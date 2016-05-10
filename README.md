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
CentralConfigManager config = new CentralConfigManager("http://your.centralconfig.service:3000");
ConfigItem retval = new ConfigItem();

//  Call 'get' to get your config:
var response = config.Get("YourAppName", "TheConfigNameToGet").Result;
retval = response.Data;

//  retval.Value has your config value 
```
