#!bin/bash
set -e
cd test/HttpService.Tests
dotnet restore
dotnet xunit -xml ${pwd}/../../testresults/out.xml
cd -

dotnet pack src/HttpService/HttpService.csproj -c release -o ${pwd}/package --version-suffix=${BuildNumber}