param(
    [string]$NuGetKey = $env:NUGET_KEY,
    [string]$PwshGKey = $env:PWSHG_KEY,
    [string]$GitHubToken = $env:GITHUB_TOKEN
)

$Config = "Release"
$Root = (Resolve-Path "$PSScriptRoot/..").Path
$PublishDir = "$Root/Publish"

# Clean up
Remove-Item -Path $PublishDir/* -Recurse -Force -ErrorAction Ignore | Out-Null

# Pack the NuGet files
dotnet pack $Root/SimpleS3.slnx -p:ContinuousIntegrationBuild=true -p:PackAssemblyName=true -c $Config -o $PublishDir

# Push to NuGet
Get-ChildItem -Path "$PublishDir/*.nupkg" | ForEach-Object {
    dotnet nuget push $_.FullName --api-key $NuGetKey --source https://api.nuget.org/v3/index.json
}

# Push symbol packages to NuGet
Get-ChildItem -Path "$PublishDir/*.snupkg" | ForEach-Object {
    dotnet nuget push $_.FullName --api-key $NuGetKey --source https://api.nuget.org/v3/index.json
}