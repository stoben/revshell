## Build
dotnet publish -r win-x64 /p:PublishSingleFile=true csshell.csproj --no-dependencies --self-contained false -c Release
