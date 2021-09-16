#!/bin/bash

# osx x64
echo "Publishing OSX x64..."
dotnet publish -c release --no-self-contained /p:PublishSingleFile=true -o published -r osx-x64 src/rest.csproj
zip -r -j published/rest-osx-x64.zip published/rest

# windows x64
echo "Publishing Windows x64..."
dotnet publish -c release --no-self-contained /p:PublishSingleFile=true -o published -r win10-x64 src/rest.csproj
zip -r -j published/rest-win10-x64.zip published/rest.exe

# linux x64
echo "Publishing Linux x64..."
dotnet publish -c release --no-self-contained /p:PublishSingleFile=true -o published -r linux-x64 src/rest.csproj
zip -r -j published/rest-linux-x64.zip published/rest
