To use the OnlineTests you will need an access key from your account.
The tests are using the ProfileManager with encryption to protect your credentials, but before it works, you need to create the profile.

Run the utility 'TestSetup' to create the profile.

You also need to set BucketName in the Config.json file. The bucket needs to be created beforehand and have the following settings:
- Public block: disabled
- Versioing: enabled
- Locking: enabled
- Lifecycle bucket policy called "AllExpire" with:
    - Current version
	- Previous versions
	- Expire current version of object: 1 day
	- Permanently delete previous versions: 1 day
	- Clean-up incomplete multipart uploads: 1 day

Also remember to change TestConstants.cs to the correct values of your account