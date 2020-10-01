$NugetKey = Read-Host "Enter Nuget key"
dotnet nuget push --skip-duplicate -k $NugetKey -s https://api.nuget.org/v3/index.json TempRelease\*