[CmdletBinding()]
param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest
$repositoryRoot = $PSScriptRoot
$solution = Join-Path $repositoryRoot "MetaMystia.Extensions.sln"

function Invoke-DotNet {
    param([string[]]$Arguments)

    & dotnet $Arguments
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}

Invoke-DotNet @("restore", $solution)
Invoke-DotNet @("test", $solution, "--configuration", $Configuration, "--no-restore", "-p:DeployToGame=false")
Invoke-DotNet @("build", $solution, "--configuration", $Configuration, "--no-restore", "-p:DeployToGame=false")

$artifactRoot = Join-Path $repositoryRoot "artifacts/$Configuration"
$moduleArtifacts = @(
    foreach ($moduleDirectory in Get-ChildItem -LiteralPath $artifactRoot -Directory) {
        $files = @(Get-ChildItem -LiteralPath $moduleDirectory.FullName -File)
        if ($files.Count -ne 1 -or $files[0].Name -notmatch "-v.+\.dll$") {
            throw "Module artifact must be exactly one versioned DLL: $($moduleDirectory.FullName)"
        }
        $artifactText = [Text.Encoding]::Latin1.GetString(
            [IO.File]::ReadAllBytes($files[0].FullName))
        if (-not $artifactText.Contains("costura.metamystia.extensions.addonkit.dll.compressed")) {
            throw "Embedded AddonKit payload is missing: $($files[0].FullName)"
        }
        $files[0]
    }
)

if ($moduleArtifacts.Count -eq 0) {
    throw "No module artifacts were produced."
}

$moduleArtifacts | Select-Object FullName, Length
