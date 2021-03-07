# SimpleS3.Extensions.ProfileManager
This extension adds persistent and secure profile handling to SimpleS3. It has a few important components:
* ProfileManager - Used to add and remove profiles
* ConsoleProfileSetup - A console based setup wizard that helps you create a profile

### Using Microsoft.Extensions.DependencyInjection
If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you simply add the extension like this

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);

coreBuilder.UseProfileManager() //Adds the profile system
           .BindConfigToDefaultProfile() //Ask SimpleS3 to use credentials using a profile called "Default"
           .UseConsoleSetup(); //Add the ConsoleSetup service to the service collection

IServiceProvider serviceProvider = services.BuildServiceProvider();

//Use the profile manager to get the "Default" profile. Returns null if it does not exist.

IProfileManager profileManager = serviceProvider.GetRequiredService<IProfileManager>();
IProfile? profile = profileManager.GetDefaultProfile();

if (profile == null)
{
    //When the profile does not exist, we use the ConsoleProfileSetup class to create it.
    ConsoleProfileSetup? setup = serviceProvider.GetRequiredService<ConsoleProfileSetup>();
    setup.SetupDefaultProfile();
}
```