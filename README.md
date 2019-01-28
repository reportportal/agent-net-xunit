[![Build status](https://ci.appveyor.com/api/projects/status/2ltljpbft1ofxr32/branch/master?svg=true)](https://ci.appveyor.com/project/nvborisenko/agent-net-xunit/branch/master)

# Installation
Download zip archive from the `Releases` tab and extract it into the folder with `xunit.console.exe`. 2 files should be placed in the same folder:
- xunit.console.exe
- ReportPortal.XUnitReporter.dll

> Note: Supports only xUnit v2.4. Awaiting [issue](https://github.com/xunit/xunit/issues/1874) with ability to use custom reporters.

# Configuration
Configure connection with Report Portal server in `ReportPortal.config.json` file.

# Usage
Just execute your tests as you do it usually. Test results are automatically will be sent during execution.
