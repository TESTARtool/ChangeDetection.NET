# Testar .NET Server
The testar .NET server Proxy requests and handle authorisation to the Orient DB server. You need to host your own OrientDB server. 

## Open API (Swagger)
The restful web service are explained in an OpenAPI.json file (previously known as Swagger specifications). After hosting the server, you can locate the openapi.json at: `/v1/swagger.json`

## Health check
The server is provided with a health endpoint, reachable at /healthz. It will return either `healty` or `unhealthy`. The service is unhealthy when something is wrong with the environment variables or orientDB is not reachable. 

## Docker
A docker image can be run with the following settings
```
docker run -i -dp 5000:80 --name="Testar-Server" 5cabdc97a914 -e JwTokenGenerator__JwtSecurityKey="hfjdhfdifngyuernguinfdfs4235tfmguh" -e JwTokenGenerator__JwtIssuer="http://localhost" -e JwTokenGenerator__JwtAudience="http://localhost" -e JwTokenGenerator__JwtExpiryInSeconds=3600 -e OrientDb__OrientDbServerUrl="http://192.168.188.28:2480" -e OrientDb__StateDatabaseName="testar" 
```