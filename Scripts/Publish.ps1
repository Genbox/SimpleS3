param(
    [string]$NuGetKey = $env:NUGET_KEY,
    [string]$PwshGKey = $env:PWSHG_KEY,
    [string]$GitHubToken = $env:GITHUB_TOKEN
)

$Config = "Release"
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

. "$PSScriptRoot/Common.ps1"

$Root = (Resolve-Path "$PSScriptRoot/..").Path
$Solution = "$Root/SimpleS3.slnx"
$PublishRoot = "$Root/Publish"

if ([string]::IsNullOrWhiteSpace($NuGetKey)) {
    throw "NuGet key must be provided with -NuGetKey or NUGET_KEY."
}

# Create an isolated staging directory so stale packages cannot be published.
if (Test-Path -LiteralPath $PublishRoot) {
    Remove-Item -LiteralPath $PublishRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $PublishRoot | Out-Null
$PublishDir = New-Item -ItemType Directory -Path $PublishRoot -Name ([guid]::NewGuid().ToString("N"))

# Verify the package before publishing
Invoke-DotNet restore $Solution --locked-mode
Invoke-DotNet build $Solution -c $Config --no-restore "-p:ContinuousIntegrationBuild=true" "-p:PackAssemblyName=true"
Invoke-DotNet test --solution $Solution -c $Config --no-build "-p:PackAssemblyName=true"

# Pack the NuGet files
Invoke-DotNet pack $Solution "-p:ContinuousIntegrationBuild=true" "-p:PackAssemblyName=true" "-p:ValidatePackageMetadata=true" -c $Config --no-build -o $PublishDir.FullName

$packages = @(Get-ChildItem -LiteralPath $PublishDir.FullName -Filter "*.nupkg" -File)
$symbolPackages = @(Get-ChildItem -LiteralPath $PublishDir.FullName -Filter "*.snupkg" -File)

if ($packages.Count -eq 0) {
    throw "Expected at least one NuGet package, but found none."
}

if ($packages.Count -ne $symbolPackages.Count) {
    throw "Expected one symbol package per NuGet package, but found $($packages.Count) NuGet and $($symbolPackages.Count) symbol packages."
}

$packageStems = @($packages.BaseName | Sort-Object)
$symbolPackageStems = @($symbolPackages.BaseName | Sort-Object)

if (Compare-Object $packageStems $symbolPackageStems -CaseSensitive) {
    throw "NuGet and symbol package filename stems do not match."
}

if ($packages.Where({ -not $_.Name.StartsWith("Genbox.SimpleS3.", [StringComparison]::Ordinal) }).Count -ne 0) {
    throw "NuGet package filename does not use the expected project prefix."
}

# Push the package and its matching symbol package to NuGet.
foreach ($package in $packages) {
    Invoke-DotNet nuget push $package.FullName --api-key $NuGetKey --source https://api.nuget.org/v3/index.json
}