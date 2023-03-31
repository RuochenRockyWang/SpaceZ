# SpaceZ
## Description

+ Contains three programs: SpaceZ.DSN, SpaceZ.LaunchVehicle, SpaceZ.Payload
+ Uses named pipes for inter-process communication.

## How to build
+ Make sure .NET 6.0 environment is installed.
+ Right-click the SpaceZ solution and choose "Build Solution".
+ The generated program can be found in bin/Debug(Release)/net6.0-windows.

## How to Use

### Adding configuration
+ Add a Launch-Vehicle Config in JSON format to the Config file.

###  Running
+ Launch SpaceZ.DSN.exe.
+ Right-click on the Launch-Vehicle in sleep mode to select the launch command.
+ Right-click on the active Launch-Vehicle and Payload to execute the corresponding command, and view the data on the right side of the interface.


### Possible issues
+ When the program is closed, child processes may not be released correctly, resulting in named pipe occupation in the second launch. It may be necessary to manually close the process.
+ Output information can be viewed in the Log.