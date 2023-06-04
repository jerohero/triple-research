FROM mcr.microsoft.com/dotnet/sdk:7.0 AS installer-env

RUN dotnet_sdk_version=7.0.100 && curl -SL --output dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Sdk/$dotnet_sdk_version/dotnet-sdk-$dotnet_sdk_version-linux-x64.tar.gz && mkdir -p /usr/share/dotnet && tar -ozxf dotnet.tar.gz -C /usr/share/dotnet && rm dotnet.tar.gz

COPY . /src/dotnet-function-app/

RUN mkdir -p /home/site/wwwroot

WORKDIR /src/dotnet-function-app

RUN dotnet publish --output /home/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated7.0

ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
  AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]