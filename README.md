[![Build status](https://ci.appveyor.com/api/projects/status/2ltljpbft1ofxr32/branch/master?svg=true)](https://ci.appveyor.com/project/nvborisenko/agent-net-xunit/branch/master)

# Installation
Download zip archive from the `Releases` tab and extract it into the folder with `xunit.console.exe`. After downloading zip file, make sure Windows didn't block it: right click on zip -> Properties -> Unblock.

2 files should be placed in the same folder:
- xunit.console.exe
- ReportPortal.XUnitReporter.dll

> Note: Supports only xUnit v2.4. Awaiting [issue](https://github.com/xunit/xunit/issues/1874) with ability to use custom reporters.

To verify whether reporter is available, execute `xunit.console.exe` without parameters. `-reportportal` should be listed in Reporters section.

# Configuration
Configure connection with Report Portal server in `ReportPortal.config.json` file.

# Usage
Just execute your tests as you do it usually. Test results are automatically will be sent during execution.
