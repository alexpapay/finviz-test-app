using Dapper;
using Finviz.TestApp.ImageNet.Domain.Entries;
using Finviz.TestApp.ImageNet.Persistence.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace Finviz.TestApp.ImageNet.Persistence.Repositories;

// ReSharper disable once ClassNeverInstantiated.Global
public class ImageNetRepository(
    ILogger<ImageNetRepository> logger,
    IConfiguration configuration) :
    IImageNetRepository
{
    private const string BaseTable = nameof(ApplicationDbContext.ImageNetEntries);
    private readonly string _connectionString = configuration.GetConnectionString("Default")!;

    public async Task<List<ImageNetEntry>> GetAllAsync()
    {
        const string sql =
            $"SELECT \"{nameof(ImageNetEntry.Id)}\", \"{nameof(ImageNetEntry.Name)}\", \"{nameof(ImageNetEntry.Size)}\", \"{nameof(ImageNetEntry.ParentId)}\" FROM \"{BaseTable}\"";

        await using var connection = new NpgsqlConnection(_connectionString);
        return (await connection.QueryAsync<ImageNetEntry>(sql)).ToList();
    }

    public async Task<List<ImageNetEntry>> GetRootAsync()
    {
        const string sql = $"""
                                SELECT "{nameof(ImageNetEntry.Id)}", "{nameof(ImageNetEntry.Name)}", "{nameof(ImageNetEntry.Size)}",
                                       EXISTS (SELECT 1 FROM "{BaseTable}" c WHERE c."{nameof(ImageNetEntry.ParentId)}" = e."{nameof(ImageNetEntry.Id)}") AS "HasChildren"
                                FROM "{BaseTable}" e
                                WHERE "{nameof(ImageNetEntry.ParentId)}" IS NULL
                            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        return (await connection.QueryAsync<ImageNetEntry>(sql)).ToList();
    }

    public async Task<List<ImageNetEntry>> GetPathAsync(int id)
    {
        const string sql = $"""
                                WITH RECURSIVE path AS (
                                    SELECT * FROM "{BaseTable}" WHERE "{nameof(ImageNetEntry.Id)}" = @Id
                                    UNION ALL
                                    SELECT parent.* FROM "{BaseTable}" parent
                                    INNER JOIN path child ON parent."{nameof(ImageNetEntry.Id)}" = child."{nameof(ImageNetEntry.ParentId)}"
                                )
                                SELECT * FROM path ORDER BY "{nameof(ImageNetEntry.ParentId)}" NULLS FIRST;
                            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        var entries = await connection.QueryAsync<ImageNetEntry>(sql, new { Id = id });
        return entries.ToList();
    }

    public async Task<List<ImageNetEntry>> GetChildrenAsync(int parentId)
    {
        const string sql = $"""
                                SELECT "{nameof(ImageNetEntry.Id)}", "{nameof(ImageNetEntry.Name)}", "{nameof(ImageNetEntry.Size)}", "{nameof(ImageNetEntry.ParentId)}",
                                       EXISTS (SELECT 1 FROM "{BaseTable}" c WHERE c."{nameof(ImageNetEntry.ParentId)}" = e."{nameof(ImageNetEntry.Id)}") AS "HasChildren"
                                FROM "{BaseTable}" e
                                WHERE "{nameof(ImageNetEntry.ParentId)}" = @ParentId
                                ORDER BY "{nameof(ImageNetEntry.Name)}"
                            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        return (await connection.QueryAsync<ImageNetEntry>(sql, new { ParentId = parentId })).ToList();
    }

    public async Task<(List<ImageNetEntry> Items, int Total)> SearchAsync(string query, int skip = 0, int take = 100)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string countSql = $"""
                                     SELECT COUNT(*)
                                     FROM "{BaseTable}"
                                     WHERE "{nameof(ImageNetEntry.FullPath)}" ILIKE '%' || @Query || '%';
                                 """;

        var total = await connection.ExecuteScalarAsync<int>(countSql,
            new
            {
                Query = query,
            });

        const string sql = $"""
                                SELECT *
                                FROM "{BaseTable}"
                                WHERE "{nameof(ImageNetEntry.FullPath)}" ILIKE '%' || @Query || '%'
                                ORDER BY "{nameof(ImageNetEntry.Id)}"
                                OFFSET @Skip LIMIT @Take;
                            """;

        var items = await connection.QueryAsync<ImageNetEntry>(sql,
            new
            {
                Query = query,
                Skip = skip,
                Take = take,
            });

        return (items.ToList(), total);
    }

    public async Task BulkInsertAsync(IEnumerable<ImageNetEntry> entries)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await using (var clearCommand =
                         new NpgsqlCommand($"TRUNCATE TABLE \"{BaseTable}\" RESTART IDENTITY CASCADE", connection))
            {
                await clearCommand.ExecuteNonQueryAsync();
            }

            await using (var writer = await connection.BeginBinaryImportAsync(
                             $"COPY \"{BaseTable}\" (\"{nameof(ImageNetEntry.Name)}\", \"{nameof(ImageNetEntry.FullPath)}\", \"{nameof(ImageNetEntry.Size)}\", \"{nameof(ImageNetEntry.ParentId)}\") FROM STDIN (FORMAT BINARY)"))
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
                                          UPDATE "{BaseTable}" AS child
                                          SET "{nameof(ImageNetEntry.ParentId)}" = parent."{nameof(ImageNetEntry.Id)}"
                                          FROM "{BaseTable}" AS parent
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
            logger.LogError(exception, "Failed to import ImageNet data");
        }
    }
}
