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
            $response = Invoke-WebRequest -Uri $Uri -TimeoutSec 5
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

    $params = @{
        Uri = $Uri
        Method = "Post"
        ContentType = "application/json"
        Body = ($Body | ConvertTo-Json -Depth 10)
        TimeoutSec = 120
    }

    return Invoke-RestMethod @params
}

$startedAt = Get-Date
$agentProcess = $null
$failureMessage = $null

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
    outputs = [ordered]@{}
    success = $false
}

try {
    Write-Host ""
    Write-Host "=== 1. Prepare Foundry Local ==="

    $bootstrap = Join-Path $PSScriptRoot "start-foundry-local.ps1"
    . $bootstrap -ModelAlias $ModelAlias -Port $FoundryPort

    $report.modelId = $env:Agent__Model
    $report.foundryEndpoint = $env:Agent__FoundryLocalEndpoint
    $report.checks.foundryPrepared = $true

    Write-Host ""
    Write-Host "=== 2. Direct local model inference ==="

    $directBody = @{
        model = $env:Agent__Model
        messages = @(
            @{
                role = "user"
                content = "Reply with one short sentence explaining why an AI agent inventory is useful."
            }
        )
        temperature = 0.2
    }

    $rawCompletion = Invoke-JsonPost -Uri "$($env:Agent__FoundryLocalEndpoint)/chat/completions" -Body $directBody

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

    $processParams = @{
        FilePath = "dotnet"
        ArgumentList = @("run", "--project", $project, "--no-launch-profile")
        WorkingDirectory = $root
        RedirectStandardOutput = $stdout
        RedirectStandardError = $stderr
        PassThru = $true
    }

    $agentProcess = Start-Process @processParams

    Wait-HttpOk -Uri "$agentUrl/health"
    $report.checks.agentHealth = $true

    Wait-HttpOk -Uri "$agentUrl/ready"
    $readiness = Invoke-RestMethod -Uri "$agentUrl/ready" -Method Get
    $report.responses.readiness = $readiness
    $report.checks.agentReady = [bool]$readiness.ready

    if (-not $report.checks.agentReady) {
        throw "Golden Agent is live but not ready for inference."
    }

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

    $firstBody = @{
        message = "In one short sentence, explain why ownership matters for an enterprise AI agent."
    }

    $first = Invoke-JsonPost -Uri "$agentUrl/api/chat" -Body $firstBody

    if ([string]::IsNullOrWhiteSpace([string]$first.output)) {
        throw "Golden Agent first turn returned an empty output."
    }

    $report.responses.firstTurn = $first
    $report.checks.firstTurn = $true
    Write-Host "Agent response: $($first.output)"

    Write-Host ""
    Write-Host "=== 5. Multi-turn session ==="

    $secondBody = @{
        conversationId = $first.conversationId
        message = "Now add one concise sentence about least privilege."
    }

    $second = Invoke-JsonPost -Uri "$agentUrl/api/chat" -Body $secondBody

    if ([string]::IsNullOrWhiteSpace([string]$second.output)) {
        throw "Golden Agent second turn returned an empty output."
    }

    if ($second.conversationId -ne $first.conversationId) {
        throw "Conversation ID changed between turns."
    }

    $report.responses.secondTurn = $second
    $report.checks.multiTurn = $true

    Write-Host ""
    Write-Host "=== 6. Run evidence correlation ==="

    $evidence = Invoke-RestMethod -Uri "$agentUrl/api/evidence/runs?conversationId=$($first.conversationId)&limit=10" -Method Get
    $report.responses.runEvidence = $evidence

    $matchingRunIds = @($evidence.items | ForEach-Object { [string]$_.runId })
    $report.checks.runEvidence = (
        $matchingRunIds.Count -ge 2 -and
        $matchingRunIds -contains [string]$first.runId -and
        $matchingRunIds -contains [string]$second.runId
    )

    if (-not $report.checks.runEvidence) {
        throw "Run evidence did not correlate both Agent Framework turns."
    }

    if ($evidence.capturesContent -ne $false) {
        throw "Run evidence unexpectedly reports content capture."
    }

    Write-Host "Evidence correlated run IDs:"
    Write-Host "  first:  $($first.runId)"
    Write-Host "  second: $($second.runId)"

    Write-Host ""
    Write-Host "=== 7. Export Operations Dashboard bundle ==="

    $operationsExport = Invoke-RestMethod -Uri "$agentUrl/api/evidence/export?limit=200" -Method Get

    if ($operationsExport.schema -ne "agent365-golden-agent-evidence/v1") {
        throw "Unexpected operations evidence schema: $($operationsExport.schema)"
    }

    if ($operationsExport.capturesContent -ne $false) {
        throw "Operations export unexpectedly reports content capture."
    }

    $operationsExportPath = Join-Path $outputDir "operations-export.json"
    $operationsExport |
        ConvertTo-Json -Depth 20 |
        Set-Content -Path $operationsExportPath -Encoding UTF8

    $report.outputs.operationsExport = $operationsExportPath
    $report.checks.operationsExport = Test-Path $operationsExportPath
    $report.success = $true

    Write-Host "Dashboard bundle: $operationsExportPath"
    Write-Host ""
    Write-Host "LOCAL LAB: PASS"
}
catch {
    $failureMessage = $_.Exception.Message
    $report.error = $failureMessage

    Write-Host ""
    Write-Host "LOCAL LAB: FAIL"
    Write-Host $failureMessage
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
    if ([string]::IsNullOrWhiteSpace($failureMessage)) {
        $failureMessage = "Local lab failed."
    }

    Write-Error $failureMessage
    exit 1
}
