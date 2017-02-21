
# Metadefender Core Client (C#)
> C# library for the Metadefender Core v4 REST API.


## Features

This library makes it easy to:
* Connect to a Metadefender Core API point
* Scan files for threats
* Retrieve previous file scan results by file hash, or data_id
* Login / Logout from the REST point
* Fetching Available Scan Rules
* Fetching Engine / Database Versions
* Get information about the currently active license
* Get the version of the Metadefender Core

## Building
* Open "C Sharp Sample.sln" in Visual Studio
* In the upper menu click on "BUILD/Build Solution"
* In the upper menu click on "TEST/Run/All Tests"

## Example usages

Open the Command Prompt and change the directory where the binary is (there is a pre-built exe in the build directory). After that the following commands should work:
* "MetadefenderCoreClient.exe" -h http://localhost:8008/ -u admin -p admin -a info
* "MetadefenderCoreClient.exe" -h http://localhost:8008/ -u admin -p admin -a scan -f fileToScan.ext
* "MetadefenderCoreClient.exe" -h http://localhost:8008/ -u admin -p admin -a scan_sync -f fileToScan.ext
