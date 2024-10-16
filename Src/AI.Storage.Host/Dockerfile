FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/AI.Storage.Host/AI.Storage.Host.csproj", "Src/AI.Storage.Host/"]
COPY ["Src/AI.Storage/AI.Storage.csproj", "Src/AI.Storage/"]
COPY ["Src/AI.Storage.Http.Contracts/AI.Storage.Http.Contracts.csproj", "Src/AI.Storage.Http.Contracts/"]
COPY ["Src/AI.Storage.Entities/AI.Storage.Entities.csproj", "Src/AI.Storage.Entities/"]
RUN dotnet restore "Src/AI.Storage.Host/AI.Storage.Host.csproj"
COPY . .
WORKDIR "/src/Src/AI.Storage.Host"
RUN dotnet build "AI.Storage.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AI.Storage.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AI.Storage.Host.dll"]
