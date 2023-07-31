#!/bin/bash

# osx x64
echo "Publishing OSX x64..."
#dotnet publish -c release --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=true -p:TrimMode=Link -o published -r osx-x64 src/rest.csproj
dotnet publish -c release -r osx-arm64 -p:PublishSingleFile=true --self-contained true -o published src
zip -r -j published/rest-osx-arm64.zip published/rest

# windows x64
echo "Publishing Windows x64..."
#dotnet publish -c release --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=true -p:TrimMode=Link -o published -r win10-x64 src/rest.csproj
dotnet publish -c release -r win10-x64 -p:PublishSingleFile=true --self-contained true -o published src
zip -r -j published/rest-win10-x64.zip published/rest.exe

# linux x64
echo "Publishing Linux x64..."
#dotnet publish -c release --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=true -p:TrimMode=Link -o published -r linux-x64 src/rest.csproj
dotnet publish -c release -r linux-x64 -p:PublishSingleFile=true --self-contained true -o published src
zip -r -j published/rest-linux-x64.zip published/rest
