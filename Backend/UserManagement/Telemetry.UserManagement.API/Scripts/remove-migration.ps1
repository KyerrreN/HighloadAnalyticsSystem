Set-Location "$PSScriptRoot\.."

Write-Host "Removing last migration..."

dotnet ef migrations remove -p ..\Telemetry.UserManagement.Infrastructure

Set-Location "$PSScriptRoot"