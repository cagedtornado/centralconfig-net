# centralconfig-net [![Build status](https://ci.appveyor.com/api/projects/status/ebtl0q21f2t4tjcq?svg=true)](https://ci.appveyor.com/project/danesparza/centralconfig-net)
.NET [centralconfig](https://github.com/cagedtornado/centralconfig) client

### Quick Start

Install the [NuGet package](https://www.nuget.org/packages/CentralConfigClient/) from the package manager console:

```powershell
Install-Package CentralConfigClient
```

In your application:

```CSharp
//  Connect to the service
var config = new CentralConfigManager("http://centralconfig-service:3000", "YourAppName");

//  Call 'get' to get your configs:
var stringVal = config.Get<string>("SomeApplicationSetting");

//  You can even set a default value to indicate
//  what should be returned if your config item 
//  can't be found
var retval = config.Get<int>("SomethingNotThere", 42);

```
