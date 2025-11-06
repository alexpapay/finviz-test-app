FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["src/Finviz.TestApp.Api/Finviz.TestApp.Api.csproj", "src/Finviz.TestApp.Api/"]
RUN dotnet restore "src/Finviz.TestApp.Api/Finviz.TestApp.Api.csproj"

COPY . .

WORKDIR "/src/src/Finviz.TestApp.Api"
RUN dotnet build "./Finviz.TestApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Finviz.TestApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Finviz.TestApp.Api.dll"]
