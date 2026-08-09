param (
    [Parameter(Mandatory=$true, Position=0)]
    [string]$Name
)

Set-Location "$PSScriptRoot\.."

Write-Host "Creating migration: $Name..."
dotnet ef migrations add $Name -p ..\Telemetry.UserManagement.Infrastructure
