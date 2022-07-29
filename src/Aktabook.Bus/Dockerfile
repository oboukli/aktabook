FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Aktabook.RequestProcessor.BusEndpoint/Aktabook.RequestProcessor.BusEndpoint.csproj", "Aktabook.RequestProcessor.BusEndpoint/"]
RUN dotnet restore "src/Aktabook.RequestProcessor.BusEndpoint/Aktabook.RequestProcessor.BusEndpoint.csproj"
COPY . .
WORKDIR "/src/Aktabook.RequestProcessor.BusEndpoint"
RUN dotnet build "Aktabook.RequestProcessor.BusEndpoint.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aktabook.RequestProcessor.BusEndpoint.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aktabook.RequestProcessor.BusEndpoint.dll"]
