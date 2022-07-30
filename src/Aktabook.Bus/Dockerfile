FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Aktabook.Bus/Aktabook.Bus.csproj", "Aktabook.Bus/"]
RUN dotnet restore --locked-mode "src/Aktabook.Bus/Aktabook.Bus.csproj"
COPY . .
WORKDIR "/src/Aktabook.Bus"
RUN dotnet build "Aktabook.Bus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aktabook.Bus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aktabook.Bus.dll"]
