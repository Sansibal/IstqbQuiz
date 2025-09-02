# Basis-Image für die Laufzeit
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build-Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Alles kopieren
COPY . .

# Projekt wiederherstellen
RUN dotnet restore "./IstqbQuiz.Server/IstqbQuiz.Server.csproj"

# Projekt bauen und veröffentlichen
RUN dotnet publish "./IstqbQuiz.Server/IstqbQuiz.Server.csproj" -c Release -o /app/publish

# Finales Image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Startpunkt
ENTRYPOINT ["dotnet", "IstqbQuiz.Server.dll"]
