param (
    [string]$Target = "0"
)

Set-Location "$PSScriptRoot\.."

Write-Host "Rolling back database to migration: $Target..."
dotnet ef database update $Target