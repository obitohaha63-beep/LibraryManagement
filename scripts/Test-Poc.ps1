param([switch]$IncludeDesktop)
$ErrorActionPreference = 'Stop'
Push-Location (Split-Path $PSScriptRoot -Parent)
try {
    & dotnet test tests/Library.Tests --logger 'trx;LogFileName=library-tests.trx' --results-directory artifacts/TestResults
    if ($LASTEXITCODE -ne 0) { throw 'Unit/integration tests failed.' }
    if ($IncludeDesktop) {
        & dotnet run --project tests/Library.Desktop.Smoke -- artifacts/screenshots
        if ($LASTEXITCODE -ne 0) { throw 'Desktop smoke test failed. Start the POC server first.' }
    }
} finally { Pop-Location }
