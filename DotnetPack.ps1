Remove-Item TempRelease\* -ErrorAction Ignore
dotnet pack src\SimpleS3.sln -c release -o TempRelease