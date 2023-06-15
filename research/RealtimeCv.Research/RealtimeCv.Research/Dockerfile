FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RealtimeCv.Research/RealtimeCv.Research.csproj", "RealtimeCv.Research/"]
RUN dotnet restore "RealtimeCv.Research/RealtimeCv.Research.csproj"
COPY . .
WORKDIR "/src/RealtimeCv.Research"
RUN dotnet build "RealtimeCv.Research.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealtimeCv.Research.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealtimeCv.Research.dll"]
