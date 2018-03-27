# tm1kill
`tm1kill` is a small executable that cancels **ALL** running threads or disconnects **ALL** users connected to TM1.

The program is written using C# and runs on the .NET framework 2.0 or later. All communication with TM1 is through the `C API`, if you are interested in understanding how the `C API` works in TM1 this is a good example.

## Setup
For the program to run, it must know the location of tm1api.dll (64 bit), you can either:
1. Add tm1kill.exe to the TM1 64 bit bin directory, i.e. C:\Program Files\ibm\cognos\tm1_64\bin64
2. Add the TM1 64 bit bin directory (C:\Program Files\ibm\cognos\tm1_64\bin64) to your PATH system variable.
3. Add the TM1 64 bit bin directory (C:\Program Files\ibm\cognos\tm1_64\bin64) to your PATH in a batch file.

## Sample Bath File

```
set PATH=C:\Program Files\ibm\cognos\tm1_64\bin64
tm1kill.exe -adminhost "localhost" -server "CXMD" -username "admin" -password "" -cancel -disconnect
```

The command line options (case-insensitive) are as follows:
* **-USERNAME**: The user name to log into TM1.
* **-PASSWORD**: The password to use log into TM1.
* **-CAMNAMESPACE** (Optional): The CAM Namespace to use to log into TM1. This is only required when using CAM security (mode 4 and 5).
* **-SERVER**: The name of the TM1 server you are connecting to.
* **-ADMINHOST**: The name or IP address of the admin host for the TM1 server.
* **-CANCEL**: Cancels all running TM1 threads.
* **-DISCONNECT**: Disconnects all users from TM1.

You must pass **-CANCEL** or **-DISCONNECT** or both. If you pass both arguments `tm1kill` will cancel all running threads and then disconnect all users.

## Download

To download the executable go to the release page: https://code.cubewise.com/tm1kill
