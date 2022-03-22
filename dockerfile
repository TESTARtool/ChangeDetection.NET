FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/BlazorApp/*.csproj BlazorApp/
COPY src/Core/*.csproj Core/
RUN dotnet restore BlazorApp/BlazorApp.csproj

# copy and build app and libraries
COPY src/Core/ Core/
COPY src/BlazorApp/ BlazorApp/
WORKDIR /source/BlazorApp
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# final stage/image
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY src/BlazorApp/nginx.conf /etc/nginx/nginx.conf