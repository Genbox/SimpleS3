Tests in this section are uploading/querying a live S3 bucket. You will need an access key to your account perform the tests.
Set the KeyId and SecretAccessKey using 'dotnet user-secrets' like this:

dotnet user-secrets set KeyId <YouKeyIdHere>
dotnet user-secrets set AccessKey <YourSecretAccessKeyHere>

Note that the default region is EUWest1 (Ireland). You can change it inside Config.json

You also need to set BucketName in the Config.json file. The bucket needs to be create beforehand and have the following:
- Public block: disabled
- Versioing: enabled
- Locking: enabled
- Lifecycle bucket policy called "AllExpire" with:
    - Current version
	- Previous versions
	- Expire current version of object: 1 day
	- Permanently delete previous versions: 1 day
	- Clean-up incomplete multipart uploads: 1 day

If you want to use Minio's play server, run the following:

dotnet user-secrets set KeyId Q3AM3UQ867SPQQA43P2F
dotnet user-secrets set AccessKey zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG

Don't forget to change Config.json to use the Minio endpoint and region
Also remember to change TestConstants.cs to the correct values of your account