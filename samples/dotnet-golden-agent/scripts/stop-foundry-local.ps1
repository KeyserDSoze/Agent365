param(
    [string]$Model = ""
)

$ErrorActionPreference = "Stop"

if ($Model) {
    Write-Host "Unloading $Model..."
    foundry model unload $Model
}

Write-Host "Stopping Foundry Local server..."
foundry server stop
