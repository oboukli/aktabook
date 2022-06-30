FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Aktabook.PublicApi/Aktabook.PublicApi.csproj", "Aktabook.PublicApi/"]
RUN dotnet restore "src/Aktabook.PublicApi/Aktabook.PublicApi.csproj"
COPY . .
WORKDIR "/src/Aktabook.PublicApi"
RUN dotnet build "Aktabook.PublicApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aktabook.PublicApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aktabook.PublicApi.dll"]
