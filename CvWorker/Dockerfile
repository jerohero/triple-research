FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CvWorker/CvWorker.csproj", "CvWorker/"]
RUN dotnet restore "CvWorker/CvWorker.csproj"
COPY . .
WORKDIR "/src/CvWorker"
RUN dotnet build "CvWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CvWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CvWorker.dll"]
