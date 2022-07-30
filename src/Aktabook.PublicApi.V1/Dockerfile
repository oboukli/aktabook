FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Aktabook.PublicApi.V1/Aktabook.PublicApi.V1.csproj", "Aktabook.PublicApi.V1/"]
RUN dotnet restore --locked-mode "src/Aktabook.PublicApi.V1/Aktabook.PublicApi.V1.csproj"
COPY . .
WORKDIR "/src/Aktabook.PublicApi.V1"
RUN dotnet build "Aktabook.PublicApi.V1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aktabook.PublicApi.V1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aktabook.PublicApi.V1.dll"]
