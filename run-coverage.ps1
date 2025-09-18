# run-coverage.ps1
Write-Host "==> Starte Tests mit Coverage..."

# Lösche alte Coverage-Dateien
Remove-Item -Recurse -Force "coverage.xml", "coverage-report" -ErrorAction SilentlyContinue

# 1. Coverage sammeln
dotnet-coverage collect "dotnet test" -f cobertura -o coverage.xml

# 2. Report erzeugen
reportgenerator -reports:coverage.xml -targetdir:coverage-report -reporttypes:Html

Write-Host "==> Coverage abgeschlossen!"
Write-Host "==> Öffne coverage-report/index.html für das Ergebnis."