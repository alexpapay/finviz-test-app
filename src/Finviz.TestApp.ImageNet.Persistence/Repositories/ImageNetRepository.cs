using Dapper;
using Finviz.TestApp.ImageNet.Domain.Entries;
using Finviz.TestApp.ImageNet.Persistence.Contexts;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace Finviz.TestApp.ImageNet.Persistence.Repositories;

// ReSharper disable once ClassNeverInstantiated.Global
public class ImageNetRepository(IConfiguration configuration) : IImageNetRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("Default")!;

    public async Task<List<ImageNetEntry>> GetAllAsync()
    {
        const string sql =
            $"SELECT \"{nameof(ImageNetEntry.Id)}\", \"{nameof(ImageNetEntry.Name)}\", \"{nameof(ImageNetEntry.Size)}\", \"{nameof(ImageNetEntry.ParentId)}\" FROM \"{nameof(ApplicationDbContext.ImageNetEntries)}\"";

        await using var connection = new NpgsqlConnection(_connectionString);
        return (await connection.QueryAsync<ImageNetEntry>(sql)).ToList();
    }

    public async Task<List<ImageNetEntry>> GetRootAsync()
    {
        const string sql = $"""
                                SELECT "{nameof(ImageNetEntry.Id)}", "{nameof(ImageNetEntry.Name)}", "{nameof(ImageNetEntry.Size)}",
                                       EXISTS (SELECT 1 FROM "{nameof(ApplicationDbContext.ImageNetEntries)}" c WHERE c."{nameof(ImageNetEntry.ParentId)}" = e."{nameof(ImageNetEntry.Id)}") AS "HasChildren"
                                FROM "{nameof(ApplicationDbContext.ImageNetEntries)}" e
                                WHERE "{nameof(ImageNetEntry.ParentId)}" IS NULL
                            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        return (await connection.QueryAsync<ImageNetEntry>(sql)).ToList();
    }

    public async Task<List<ImageNetEntry>> GetChildrenAsync(int parentId)
    {
        const string sql = $"""
                                SELECT "{nameof(ImageNetEntry.Id)}", "{nameof(ImageNetEntry.Name)}", "{nameof(ImageNetEntry.Size)}", "{nameof(ImageNetEntry.ParentId)}",
                                       EXISTS (SELECT 1 FROM "{nameof(ApplicationDbContext.ImageNetEntries)}" c WHERE c."{nameof(ImageNetEntry.ParentId)}" = e."{nameof(ImageNetEntry.Id)}") AS "HasChildren"
                                FROM "{nameof(ApplicationDbContext.ImageNetEntries)}" e
                                WHERE "{nameof(ImageNetEntry.ParentId)}" = @ParentId
                                ORDER BY "{nameof(ImageNetEntry.Name)}"
                            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        return (await connection.QueryAsync<ImageNetEntry>(sql, new {ParentId = parentId})).ToList();
    }

    public async Task<List<ImageNetEntry>> SearchAsync(string query)
    {
        const string sql = $"""
                                SELECT * FROM "{nameof(ApplicationDbContext.ImageNetEntries)}" 
                                WHERE "{nameof(ImageNetEntry.FullPath)}" ILIKE '%' || @Query || '%'
                                LIMIT 200
                            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        return (await connection.QueryAsync<ImageNetEntry>(sql, new {Query = query})).ToList();
    }

    public async Task BulkInsertAsync(IEnumerable<ImageNetEntry> entries)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        const string tableName = nameof(ApplicationDbContext.ImageNetEntries);

        try
        {
            await using (var clearCommand =
                         new NpgsqlCommand($"TRUNCATE TABLE \"{tableName}\" RESTART IDENTITY CASCADE", connection))
            {
                await clearCommand.ExecuteNonQueryAsync();
            }

            await using (var writer = await connection.BeginBinaryImportAsync(
                             $"COPY \"{tableName}\" (\"{nameof(ImageNetEntry.Name)}\", \"{nameof(ImageNetEntry.FullPath)}\", \"{nameof(ImageNetEntry.Size)}\", \"{nameof(ImageNetEntry.ParentId)}\") FROM STDIN (FORMAT BINARY)"))
            {
                foreach (var entry in entries)
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(entry.Name, NpgsqlDbType.Text);
                    await writer.WriteAsync(entry.FullPath, NpgsqlDbType.Text);
                    await writer.WriteAsync(entry.Size, NpgsqlDbType.Integer);
                    await writer.WriteAsync(entry.ParentId, NpgsqlDbType.Integer);
                }

                await writer.CompleteAsync();
            }

            const string updateSql = $"""
                                          UPDATE "{tableName}" AS child
                                          SET "{nameof(ImageNetEntry.ParentId)}" = parent."{nameof(ImageNetEntry.Id)}"
                                          FROM "{tableName}" AS parent
                                          WHERE parent."{nameof(ImageNetEntry.FullPath)}" = substring(
                                              child."{nameof(ImageNetEntry.FullPath)}" FROM 1 FOR length(child."{nameof(ImageNetEntry.FullPath)}") - position(' > ' in reverse(child."{nameof(ImageNetEntry.FullPath)}")) - 2
                                          );
                                      """;

            await using (var updateCommand = new NpgsqlCommand(updateSql, connection))
            {
                await updateCommand.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            throw new InvalidOperationException("Failed to import ImageNet data", exception);
        }
    }
}