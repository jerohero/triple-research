# Stage 0: Setup OpenCV
FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy as opencv-builder

ENV OPENCV=4.7.0 DEBIAN_FRONTEND=noninteractive

WORKDIR /

# BUILD
FROM  mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated7.0 AS base
WORKDIR /home/site/wwwroot

FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
WORKDIR /src
COPY ["src/RealtimeCv.Functions/RealtimeCv.Functions.csproj", "src/RealtimeCv.Functions/"]
RUN dotnet restore "src/RealtimeCv.Functions/RealtimeCv.Functions.csproj"
COPY . .
WORKDIR "/src/src/RealtimeCv.Functions"
RUN dotnet build "RealtimeCv.Functions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealtimeCv.Functions.csproj" -c Release -o /app/publish

# RUN
FROM base AS final
WORKDIR /home/site/wwwroot

COPY --from=publish /app/publish .
COPY --from=opencv-builder /usr/lib /usr/lib

ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
  AzureFunctionsJobHost__Logging__Console__IsEnabled=true
  
ENTRYPOINT ["dotnet", "RealtimeCv.Functions.dll"]
