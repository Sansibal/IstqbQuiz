# ==========================
# 1. Build Stage
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy sln and csproj files first for better layer caching
COPY ["IstqbQuiz.sln", "."]
COPY ["Server/IstqbQuiz.Server.csproj", "Server/"]
COPY ["Client/IstqbQuiz.Client.csproj", "Client/"]
COPY ["Shared/IstqbQuiz.Shared.csproj", "Shared/"]

RUN dotnet restore "IstqbQuiz.sln"

# Copy everything else and build
COPY . .
RUN dotnet publish "Server/IstqbQuiz.Server.csproj" -c Release -o /app/publish

# ==========================
# 2. Runtime Stage
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "IstqbQuiz.Server.dll"]
