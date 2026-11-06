param([switch]$SkipBuild, [switch]$WebOnly)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
Push-Location $projectRoot
try {
    if (-not $SkipBuild) { & dotnet build ThuVien.sln; if ($LASTEXITCODE -ne 0) { throw 'Build failed.' } }
    $running = $false
    try { $running = (Invoke-RestMethod 'http://localhost:5080/health' -TimeoutSec 2).status -eq 'ok' } catch { }
    if (-not $running) {
        New-Item -ItemType Directory -Force artifacts | Out-Null
        $serverProject = Join-Path $projectRoot 'src/Library.Server/Library.Server.csproj'
        Start-Process dotnet -ArgumentList @('run','--project',('"' + $serverProject + '"'),'--no-build','--launch-profile','http') -WorkingDirectory $projectRoot -WindowStyle Hidden -RedirectStandardOutput (Join-Path $projectRoot 'artifacts/server.log') -RedirectStandardError (Join-Path $projectRoot 'artifacts/server-error.log') | Out-Null
        for ($attempt=0; $attempt -lt 30; $attempt++) {
            Start-Sleep -Milliseconds 500
            try { if ((Invoke-RestMethod 'http://localhost:5080/health' -TimeoutSec 1).status -eq 'ok') { $running=$true; break } } catch { }
        }
        if (-not $running) { throw 'Server did not start. Check artifacts/server-error.log and the SQL Server connection.' }
    }
    Write-Host 'Web: http://localhost:5080 | OpenAPI: http://localhost:5080/openapi/v1.json'
    if (-not $WebOnly) { Start-Process (Join-Path $projectRoot 'src/Library.Desktop/bin/Debug/net9.0-windows/Library.Desktop.exe') }
} finally { Pop-Location }
