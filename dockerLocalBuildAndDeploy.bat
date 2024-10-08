dotnet clean "./ChangeDetection.sln"

dotnet restore "./ChangeDetection.sln"

@rem build both docker images
docker build -t change_detection/testar-net-ui:latest -f src/BlazorApp/Dockerfile .

docker build -t change_detection/testar-net-server:latest -f src/ChangeDetection.Server/Dockerfile .

@rem deploy containers
docker run -i -dp 5000:80 --name="Testar-Server" -e JwTokenGenerator__JwtSecurityKey="hfjdhfdifngyuernguinfdfs4235tfmguh" -e JwTokenGenerator__JwtIssuer="http://localhost" -e JwTokenGenerator__JwtAudience="http://localhost" -e JwTokenGenerator__JwtExpiryInSeconds=3600 -e OrientDb__OrientDbServerUrl="http://host.docker.internal:2480" -e OrientDb__StateDatabaseName="testar" change_detection/testar-net-server

docker run -i -dp 80:80 --name="Testar-UI" change_detection/testar-net-ui

@rem launch chrome with the running port
start chrome http://localhost/Login