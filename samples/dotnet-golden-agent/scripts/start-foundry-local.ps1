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

function Resolve-LoadedModelId {
    param(
        [string]$BaseUrl
    )

    $candidates = @(
        "$BaseUrl/v1/models",
        "$BaseUrl/openai/loadedmodels",
        "$BaseUrl/openai/models"
    )

    foreach ($uri in $candidates) {
        try {
            $response = Invoke-RestMethod -Uri $uri -Method Get -TimeoutSec 10

            if ($response.data -and $response.data.Count -gt 0 -and $response.data[0].id) {
                return [string]$response.data[0].id
            }

            if ($response -is [System.Array] -and $response.Count -gt 0) {
                if ($response[0] -is [string]) {
                    return [string]$response[0]
                }

                if ($response[0].id) {
                    return [string]$response[0].id
                }

                if ($response[0].name) {
                    return [string]$response[0].name
                }
            }

            if ($response.models -and $response.models.Count -gt 0) {
                $candidate = $response.models[0]
                if ($candidate -is [string]) {
                    return [string]$candidate
                }

                if ($candidate.id) {
                    return [string]$candidate.id
                }

                if ($candidate.name) {
                    return [string]$candidate.name
                }
            }
        } catch {
            Write-Verbose "Model discovery endpoint failed: $uri :: $($_.Exception.Message)"
        }
    }

    throw "Foundry Local is running, but no loaded model ID could be resolved from the documented model endpoints."
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
$modelId = Resolve-LoadedModelId -BaseUrl $baseUrl

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
