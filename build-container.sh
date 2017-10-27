#!bin/bash
set -e
cd test/HttpService.Tests
dotnet restore
dotnet xunit -xml ${pwd}/../../testresults/out.xml
cd -
# --version-suffix=${BuildNumber}
dotnet pack src/HttpService/HttpService.csproj -c release -o ${pwd}/package 