param(
    [string]$ModelAlias = "phi-4-mini",
    [int]$Port = 39839,
    [switch]$RunAgent
)

$ErrorActionPreference = "Stop"

function Resolve-FoundryCommand {
    $cmd = Get-Command foundry -ErrorAction SilentlyContinue
    if ($cmd) {
        return $cmd.Source
    }

    $wingetLink = Join-Path $env:LOCALAPPDATA "Microsoft\WinGet\Links\foundry.exe"
    if (Test-Path $wingetLink) {
        return $wingetLink
    }

    return $null
}

$foundry = Resolve-FoundryCommand

if (-not $foundry) {
    Write-Host "Foundry Local CLI not found. Installing Microsoft.FoundryLocal with winget..."
    winget install --id Microsoft.FoundryLocal -e --accept-package-agreements --accept-source-agreements

    $foundry = Resolve-FoundryCommand
    if (-not $foundry) {
        throw "Foundry Local was installed but foundry.exe is not visible in this PowerShell session. Open a new terminal and run this script again."
    }
}

Write-Host "Foundry Local: $foundry"
& $foundry --version

Write-Host "Starting OpenAI-compatible local server on port $Port..."
& $foundry server restart --port $Port --idle-timeout 0

Write-Host "Preparing model alias '$ModelAlias'..."
& $foundry model download $ModelAlias
& $foundry model load $ModelAlias

$baseUrl = "http://127.0.0.1:$Port"
$models = Invoke-RestMethod -Uri "$baseUrl/v1/models" -Method Get

$modelId = $null
if ($models.data -and $models.data.Count -gt 0) {
    $modelId = $models.data[0].id
} elseif ($models -is [System.Array] -and $models.Count -gt 0) {
    $modelId = [string]$models[0]
}

if (-not $modelId) {
    throw "Foundry Local is running, but no loaded model was returned by /v1/models."
}

$env:Agent__Provider = "foundry-local"
$env:Agent__FoundryLocalEndpoint = "$baseUrl/v1"
$env:Agent__Model = $modelId

Write-Host ""
Write-Host "Foundry Local is ready."
Write-Host "  Alias:    $ModelAlias"
Write-Host "  Model ID: $modelId"
Write-Host "  Endpoint: $env:Agent__FoundryLocalEndpoint"
Write-Host ""
Write-Host "Environment configured for this PowerShell process:"
Write-Host "  Agent__Provider=$env:Agent__Provider"
Write-Host "  Agent__FoundryLocalEndpoint=$env:Agent__FoundryLocalEndpoint"
Write-Host "  Agent__Model=$env:Agent__Model"

if ($RunAgent) {
    $project = Join-Path $PSScriptRoot "..\Agent365.GoldenAgent.csproj"
    Write-Host ""
    Write-Host "Starting AGIC Agent 365 Golden Agent..."
    dotnet run --project $project
} else {
    Write-Host ""
    Write-Host "Tip: run '.\scripts\start-foundry-local.ps1 -RunAgent' to start the API in the same process context."
}
