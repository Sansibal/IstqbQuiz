# -------------------------
# Build Stage
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiere nur csproj-Dateien und restore
COPY ["IstqbQuiz.sln", "."]
COPY ["Server/IstqbQuiz.Server.csproj", "Server/"]
COPY ["Client/IstqbQuiz.Client.csproj", "Client/"]
COPY ["Shared/IstqbQuiz.Shared.csproj", "Shared/"]
RUN dotnet restore "IstqbQuiz.sln"

# Kopiere Rest und veröffentliche
COPY . .
RUN dotnet publish "Server/IstqbQuiz.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------------------------
# Runtime Stage
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IstqbQuiz.Server.dll"]
