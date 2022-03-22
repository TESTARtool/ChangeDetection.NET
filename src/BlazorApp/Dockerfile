FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/BlazorApp/*.csproj src/BlazorApp/
COPY src/Core/*.csproj src/Core/
COPY src/Analyser.cs src/
COPY Directory.Build.props /
RUN dotnet restore src/BlazorApp/BlazorApp.csproj

COPY . .
WORKDIR /source/src/BlazorApp
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# final stage/image
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY src/BlazorApp/nginx.conf /etc/nginx/nginx.conf