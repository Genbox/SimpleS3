# Provider tests

These tests use real Amazon S3, Backblaze B2, Google Cloud Storage, and Wasabi accounts. They create and delete objects and buckets and change bucket configurations. Use accounts and resources intended for testing.

Run the commands below from the repository root with the .NET 10 SDK.

## Set up profiles and test buckets

Run the interactive setup utility:

```powershell
dotnet run --project Src/SimpleS3.Utility.TestSetup/SimpleS3.Utility.TestSetup.csproj
```

Select the providers to configure. The utility creates or loads profiles with these names:

- `TestSetup-AmazonS3`
- `TestSetup-BackBlazeB2`
- `TestSetup-GoogleCloudStorage`
- `TestSetup-Wasabi`

Setup uses a test bucket named `testbucket-` followed by the first eight characters of the profile's key ID, converted to lowercase. It configures provider-specific settings on that bucket.

Use credentials for a dedicated test account with permission to exercise the APIs covered by the provider suite. [TestConstants.cs](Code/TestConstants.cs) contains the account username and email expected by some ACL, ownership, and listing tests; adjust those expectations when using a different account.

## Run online tests

```powershell
dotnet test --project Src/ProviderTests/ProviderTests.csproj
```

Missing profiles produce skipped provider rows. Tests also skip unsupported providers. Authentication and download failures can fail the run.

Review the passed, failed, and skipped counts and the skip reasons. A successful command with skipped providers does not establish coverage of those providers.

## Clean up temporary buckets

Run cleanup after the live tests:

```powershell
dotnet run --project Src/SimpleS3.Utility.TestCleanup/SimpleS3.Utility.TestCleanup.csproj
```

Select the providers to clean up. The utility empties and deletes buckets matching `tempbucket-{guid}` for those providers and reports the result for each bucket. It leaves the configured `testbucket-...` bucket and locally stored profiles in place.

## Offline tests and CI

To run only the offline unit-test projects:

```powershell
Get-ChildItem Src -Recurse -Filter '*.Tests.csproj' | ForEach-Object {
    dotnet test --project $_.FullName
    if ($LASTEXITCODE -ne 0) { throw "Tests failed: $($_.FullName)" }
}
```

`ProviderTests.csproj` does not match this filename pattern.

`Scripts/Build.ps1` restores in locked mode, builds, and tests the whole solution, including `ProviderTests`. It can therefore perform online operations on a developer machine or CI runner with configured profiles. A CI run without those profiles skips the corresponding online cases. For a dedicated online run, configure the test profiles for the runner's user, including access to the profile-protection keys, and check which providers actually ran.