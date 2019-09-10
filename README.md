[![Build status](https://ci.appveyor.com/api/projects/status/2ltljpbft1ofxr32/branch/master?svg=true)](https://ci.appveyor.com/project/nvborisenko/agent-net-xunit/branch/master)

There are 2 ways to use ReportPortal with xUnit framework, it's depent on runner.

- [Visual Studio Runner](#visual-studio-runner)
- [xunit.console.exe](#xunitconsoleexe)

# Visual Studio Runner

This way is applicable if you use xunit with `xunit.runner.visualstudio` nuget package.

## Installation
Install `ReportPortal.XUnit` nuget package in project with xunit tests.

[![NuGet Badge](https://buildstats.info/nuget/reportportal.xunit)](https://www.nuget.org/packages/reportportal.xunit)

## Configuration
Add `ReportPortal.config.json` file to the test project.

```json
{
  "$schema": "https://raw.githubusercontent.com/reportportal/agent-net-xunit/master/src/ReportPortal.XUnitReporter/ReportPortal.config.schema",
  "enabled": true,
  "server": {
    "url": "https://rp.epam.com/api/v1/",
    "project": "default_project",
    "authentication": {
      "uuid": "aa19555c-c9ce-42eb-bb11-87757225d535"
    }
  },
  "launch": {
    "name": "XUnit Demo Launch",
    "description": "this is description",
    "debugMode": false,
    "tags": [ "t1", "t2" ]
  }
}
```

## Run tests
Now if you execute tests via `dotnet test`, or `dotnet vstest`, or `vstest.console.exe`, you should see real-time report.

# xunit.console.exe

If you execute tests with `xunit.console.exe` runner.

## Installation
Download zip archive from the `Releases` tab and extract it into the folder with `xunit.console.exe`. After downloading zip file, make sure Windows didn't block it: right click on zip -> Properties -> Unblock.

2 files should be placed in the same folder:
- xunit.console.exe
- ReportPortal.XUnitReporter.dll

> Note: Supports only xUnit v2.4. Awaiting [issue](https://github.com/xunit/xunit/issues/1874) with ability to use custom reporters.

To verify whether reporter is available, execute `xunit.console.exe` without parameters. `-reportportal` should be listed in Reporters section.

## Configuration
Configure connection with Report Portal server in `ReportPortal.config.json` file. Sample is already included in zip archive.

## Run tests
Just execute your tests as you do it usually. Test results are automatically will be sent during execution.

# Environment variables
It's possible to override parameters via environment variables.
```cmd
set reportportal_launch_name="My new launch name"
# execute tests
```

`reportportal_` prefix is used for naming variables, and `_` is used as delimeter. For example to override `Server.Authentication.Uuid` parameter, we need specify `ReportPortal_Server_Authentication_Uuid` in environment variables. To override launch tags we need specify `ReportPortal_Launch_Tags` with `tag1;tag2` value (`;` used as separator for list of values).
