param(
    [string]$ModelAlias = "phi-4-mini",
    [int]$FoundryPort = 39839,
    [int]$AgentPort = 5080,
    [switch]$KeepRunning,
    [switch]$StopFoundryOnExit
)

$ErrorActionPreference = "Stop"

$root = Resolve-Path (Join-Path $PSScriptRoot "..")
$project = Join-Path $root "Agent365.GoldenAgent.csproj"
$outputDir = Join-Path $root "lab-output"
$agentUrl = "http://127.0.0.1:$AgentPort"

New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

function Wait-HttpOk {
    param(
        [string]$Uri,
        [int]$Attempts = 40,
        [int]$DelaySeconds = 1
    )

    for ($i = 1; $i -le $Attempts; $i++) {
        try {
            $response = Invoke-WebRequest -Uri $Uri -UseBasicParsing -TimeoutSec 5
            if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 300) {
                return
            }
        } catch {
            if ($i -eq $Attempts) {
                throw "Endpoint did not become ready: $Uri :: $($_.Exception.Message)"
            }
        }

        Start-Sleep -Seconds $DelaySeconds
    }
}

function Invoke-JsonPost {
    param(
        [string]$Uri,
        [hashtable]$Body
    )

    Invoke-RestMethod         -Uri $Uri         -Method Post         -ContentType "application/json"         -Body ($Body | ConvertTo-Json -Depth 10)         -TimeoutSec 120
}

$startedAt = Get-Date
$agentProcess = $null
$report = [ordered]@{
    startedAt = $startedAt.ToString("o")
    machine = $env:COMPUTERNAME
    modelAlias = $ModelAlias
    foundryPort = $FoundryPort
    agentPort = $AgentPort
    provider = "foundry-local"
    modelId = $null
    foundryEndpoint = $null
    checks = [ordered]@{}
    responses = [ordered]@{}
    success = $false
}

try {
    Write-Host ""
    Write-Host "=== 1. Prepare Foundry Local ==="
    . (Join-Path $PSScriptRoot "start-foundry-local.ps1")         -ModelAlias $ModelAlias         -Port $FoundryPort

    $report.modelId = $env:Agent__Model
    $report.foundryEndpoint = $env:Agent__FoundryLocalEndpoint
    $report.checks.foundryPrepared = $true

    Write-Host ""
    Write-Host "=== 2. Direct local model inference ==="
    $rawCompletion = Invoke-JsonPost         -Uri "$($env:Agent__FoundryLocalEndpoint)/chat/completions"         -Body @{
            model = $env:Agent__Model
            messages = @(
                @{
                    role = "user"
                    content = "Reply with one short sentence explaining why an AI agent inventory is useful."
                }
            )
            temperature = 0.2
        }

    $rawText = [string]$rawCompletion.choices[0].message.content
    if ([string]::IsNullOrWhiteSpace($rawText)) {
        throw "Foundry Local returned an empty chat completion."
    }

    $report.responses.directModel = $rawText
    $report.checks.directInference = $true
    Write-Host "Foundry Local response: $rawText"

    Write-Host ""
    Write-Host "=== 3. Start Golden Agent API ==="
    $env:ASPNETCORE_URLS = $agentUrl
    $env:Agent365__ExportToConsole = "true"
    $env:Agent365__ExportToAgent365 = "false"

    $stdout = Join-Path $outputDir "agent-stdout.log"
    $stderr = Join-Path $outputDir "agent-stderr.log"

    $agentProcess = Start-Process         -FilePath "dotnet"         -ArgumentList @("run", "--project", $project, "--no-launch-profile")         -WorkingDirectory $root         -RedirectStandardOutput $stdout         -RedirectStandardError $stderr         -PassThru

    Wait-HttpOk -Uri "$agentUrl/health"
    $report.checks.agentHealth = $true

    $config = Invoke-RestMethod -Uri "$agentUrl/api/config" -Method Get
    $report.responses.safeConfig = $config
    $report.checks.safeConfig = (
        $config.agent.provider -eq "foundry-local" -and
        $config.agent.model -eq $env:Agent__Model
    )

    if (-not $report.checks.safeConfig) {
        throw "Golden Agent safe config does not match the Foundry Local runtime selected by the lab."
    }

    Write-Host ""
    Write-Host "=== 4. First Agent Framework turn ==="
    $first = Invoke-JsonPost         -Uri "$agentUrl/api/chat"         -Body @{
            message = "In one short sentence, explain why ownership matters for an enterprise AI agent."
        }

    if ([string]::IsNullOrWhiteSpace([string]$first.output)) {
        throw "Golden Agent first turn returned an empty output."
    }

    $report.responses.firstTurn = $first
    $report.checks.firstTurn = $true
    Write-Host "Agent response: $($first.output)"

    Write-Host ""
    Write-Host "=== 5. Multi-turn session ==="
    $second = Invoke-JsonPost         -Uri "$agentUrl/api/chat"         -Body @{
            conversationId = $first.conversationId
            message = "Now add one concise sentence about least privilege."
        }

    if ([string]::IsNullOrWhiteSpace([string]$second.output)) {
        throw "Golden Agent second turn returned an empty output."
    }

    if ($second.conversationId -ne $first.conversationId) {
        throw "Conversation ID changed between turns."
    }

    $report.responses.secondTurn = $second
    $report.checks.multiTurn = $true
    $report.success = $true

    Write-Host "Agent response: $($second.output)"
    Write-Host ""
    Write-Host "LOCAL LAB: PASS"
}
catch {
    $report.error = $_.Exception.Message
    Write-Host ""
    Write-Host "LOCAL LAB: FAIL"
    Write-Error $_
}
finally {
    $finishedAt = Get-Date
    $report.finishedAt = $finishedAt.ToString("o")
    $report.durationSeconds = [math]::Round(($finishedAt - $startedAt).TotalSeconds, 2)

    $stamp = $startedAt.ToString("yyyyMMdd-HHmmss")
    $reportPath = Join-Path $outputDir "lab-$stamp.json"
    $latestPath = Join-Path $outputDir "latest.json"

    $json = $report | ConvertTo-Json -Depth 20
    Set-Content -Path $reportPath -Value $json -Encoding UTF8
    Set-Content -Path $latestPath -Value $json -Encoding UTF8

    Write-Host ""
    Write-Host "Report: $reportPath"

    if ($agentProcess -and -not $KeepRunning) {
        if (-not $agentProcess.HasExited) {
            Stop-Process -Id $agentProcess.Id -Force
        }
    }

    if ($StopFoundryOnExit) {
        try {
            foundry server stop
        } catch {
            Write-Warning "Unable to stop Foundry Local: $($_.Exception.Message)"
        }
    }
}

if (-not $report.success) {
    exit 1
}
