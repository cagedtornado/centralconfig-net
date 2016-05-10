# centralconfig-net [![Build status](https://ci.appveyor.com/api/projects/status/ebtl0q21f2t4tjcq?svg=true)](https://ci.appveyor.com/project/danesparza/centralconfig-net)
.NET centralconfig client

### Quick Start

Install the [NuGet package](https://www.nuget.org/packages/CentralConfigClient/) from the package manager console:

```powershell
Install-Package CentralConfigClient
```

In your application, call:

```CSharp
//  Connect to the service
CentralConfigManager config = new CentralConfigManager("http://your.centralconfig.service:3000", "YourAppName");

//  Call 'get' to get your configs:
var stringVal = config.Get<string>("SomeApplicationSetting");

//  You can even set a default value to indicate
//  what should be returned if your config item 
//  can't be found
var retval = config.Get<int>("SomethingNotThere", 42);

```
