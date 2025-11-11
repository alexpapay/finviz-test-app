FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

COPY ["src/Finviz.TestApp.ImageNet.Api/Finviz.TestApp.ImageNet.Api.csproj", "src/Finviz.TestApp.ImageNet.Api/"]
COPY ["src/Finviz.TestApp.ImageNet.Infrastructure/Finviz.TestApp.ImageNet.Infrastructure.csproj", "src/Finviz.TestApp.ImageNet.Infrastructure/"]
COPY ["src/Finviz.TestApp.ImageNet.Persistence/Finviz.TestApp.ImageNet.Persistence.csproj", "src/Finviz.TestApp.ImageNet.Persistence/"]
COPY ["src/Finviz.TestApp.ImageNet.Domain/Finviz.TestApp.ImageNet.Domain.csproj", "src/Finviz.TestApp.ImageNet.Domain/"]
COPY ["src/Finviz.TestApp.Domain.Shared/Finviz.TestApp.Domain.Shared.csproj", "src/Finviz.TestApp.Domain.Shared/"]

RUN dotnet restore "src/Finviz.TestApp.ImageNet.Api/Finviz.TestApp.ImageNet.Api.csproj"

COPY . .

WORKDIR "/src/src/Finviz.TestApp.ImageNet.Api"

RUN dotnet build "./Finviz.TestApp.ImageNet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Finviz.TestApp.ImageNet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Finviz.TestApp.ImageNet.Api.dll"]
