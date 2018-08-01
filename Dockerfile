FROM microsoft/dotnet:2.0.0-sdk
ARG BuildNumber=local
ENV BuildNumber=${BuildNumber}
RUN mkdir app
WORKDIR /app

COPY *.sln .
COPY ./src/HttpService/HttpService.csproj /app/src/HttpService/HttpService.csproj
COPY ./test/HttpService.Tests/HttpService.Tests.csproj /app/test/HttpService.Tests/HttpService.Tests.csproj

RUN dotnet restore

COPY . .

RUN ["sh", "build-container.sh"]

