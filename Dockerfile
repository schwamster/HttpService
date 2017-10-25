FROM microsoft/dotnet:2.0-sdk
ARG BuildNumber=local
ENV BuildNumber=${BuildNumber}
WORKDIR /app

COPY *.sln .
COPY ./src/HttpService/HttpService.csproj ./src/HttpService/HttpService.csproj
COPY ./test/HttpService.Tests/HttpService.Tests.csproj ./test/HttpService.Tests/HttpService.Tests.csproj

RUN dotnet restore

COPY . .

RUN ["sh", "build-container.sh"]

