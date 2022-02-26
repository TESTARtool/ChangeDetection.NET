FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV JwTokenGenerator__JwtSecurityKey="Eo8ryfwp9n8rcyi8yrilhwetycowiruyfo8DSHIvj"
ENV JwTokenGenerator__JwtIssuer="http://localhost"
ENV JwTokenGenerator__JwtAudience="http://localhost"
ENV JwTokenGenerator__JwtExpiryInSeconds=3600

ENV OrientDb__OrientDbServerUrl="http://192.168.188.28:2480"
ENV OrientDb__StateDatabaseName="testar"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/ChangeDetection.Server/ChangeDetection.Server.csproj", "src/ChangeDetection.Server/"]
RUN dotnet restore "src/ChangeDetection.Server/ChangeDetection.Server.csproj"
COPY . .
WORKDIR "/src/src/ChangeDetection.Server"
RUN dotnet build "ChangeDetection.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChangeDetection.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Testar.ChangeDetection.Server.dll"]

HEALTHCHECK CMD curl --fail http://localhost:5000/healthz || exit