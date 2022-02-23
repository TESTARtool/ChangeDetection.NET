FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://*:5000


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