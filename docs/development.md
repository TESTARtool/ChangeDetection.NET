# ChangeDetection.NET Development

The ChangeDetection.NET application is created with C# in .NET core 6. To start developing, you can use Visual Studio **2022**. A free community version can be downloaded at [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) for Windows and macOS.

When installing Visual Studio, make sure the following workloads are installed: 
- ASP.NET and web development
- .NET desktop development

Git clone the source via:

`git clone https://github.com/TESTARtool/ChangeDetection.NET.git`

To prevent committing the orientDB password, a user secret can be set up. Navigate to the folder of the repository and execute the commands: 
```
cd src\ConsoleApp\
dotnet user-secrets set "OrientDB:Password" "**********"
```

## OrientDB database
*Optional. An existing OrientDB server can be used.*

The .NET version makes use of the REST API of OrientDB. Make sure the API can be reached from your development machine. The easiest way to get OrientDB setup is by making use of Docker. More information about setting up Docker and WSL can be found here: https://docs.microsoft.com/en-us/windows/wsl/tutorials/wsl-containers. OrientDB information can be found at: http://orientdb.com/docs/3.0.x/admin/Docker-Home.html 

## Testing on Ubuntu
*Optional. Not needed when only using Windows*

To test the code on Ubuntu, WSL 2 can be used. More information about setting up WSL can be found here: https://docs.microsoft.com/en-us/windows/wsl/install