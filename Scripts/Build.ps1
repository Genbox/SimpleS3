param(
    [ValidateSet("Debug", "Release")]
    [string]$Config = "Debug"
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

. "$PSScriptRoot/Common.ps1"

$Root = (Resolve-Path "$PSScriptRoot/..").Path
$Solution = "$Root/SimpleS3.slnx"

Invoke-DotNet restore $Solution --locked-mode
Invoke-DotNet build $Solution -c $Config --no-restore
Invoke-DotNet test --solution $Solution -c $Config --no-build