FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/AI.Storage.Migrations/AI.Storage.Migrations.csproj", "Src/AI.Storage.Migrations/"]
COPY ["Src/AI.Storage.Entities/AI.Storage.Entities.csproj", "Src/AI.Storage.Entities/"]
RUN dotnet restore "Src/AI.Storage.Migrations/AI.Storage.Migrations.csproj"
COPY . .
WORKDIR "/src/Src/AI.Storage.Migrations"
RUN dotnet build "AI.Storage.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AI.Storage.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AI.Storage.Migrations.dll"]
