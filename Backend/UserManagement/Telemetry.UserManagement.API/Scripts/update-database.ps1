Set-Location "$PSScriptRoot\.."

Write-Host "Applying migrations to database..."
dotnet ef database update
