FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
# copy csproj and restore as distinct layers
COPY src/ChangeDetection.Server/*.csproj src/ChangeDetection.Server/
COPY src/Core/*.csproj src/Core/
COPY src/Analyser.cs src/
COPY Directory.Build.props /
RUN dotnet restore src/ChangeDetection.Server/ChangeDetection.Server.csproj

COPY . .
WORKDIR /source/src/ChangeDetection.Server
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Testar.ChangeDetection.Server.dll"]