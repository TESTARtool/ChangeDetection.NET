# Testar .NET Server
The testar .NET server Proxy requests and handle authorisation to the Orient DB server. You need to host your own OrientDB server. 

## Open API (Swagger)
The restful web service are explained in an OpenAPI.json file (previously known as Swagger specifications).
After hosting the server, you can locate the openapi.json at: `/v1/swagger.json`

## Health check
The server is provided with a health endpoint, reachable at /healthz. It will return either `healty` or `unhealthy`. 
The service is unhealthy when something is wrong with the environment variables or orientDB is not reachable. 

## Docker
A docker image can be run with the following example settings
```
docker run -i -dp 5000:80 --name="Testar-Server" -e JwTokenGenerator__JwtSecurityKey="hfjdhfdifngyuernguinfdfs4235tfmguh" -e JwTokenGenerator__JwtIssuer="http://localhost" -e JwTokenGenerator__JwtAudience="http://localhost" -e JwTokenGenerator__JwtExpiryInSeconds=3600 -e OrientDb__OrientDbServerUrl="http://192.168.188.28:2480" -e OrientDb__StateDatabaseName="testar" <image-name>
```

### OrientDB multi database support
Available from version 1.1.0
Optionaly the server supports Multi dabatabase approach. Setting the `OrientDb__StateDatabaseName` 
is optional and can provide a fallback value. The user must prefix the username with the name of 
the database. For example: DataName\UserName. 

Add the following setting to the `docker run` command

```
OrientDb__MultiDatabaseSupport = false
```

| Setting | Explanation |
| --- | --- 
JwTokenGenerator__JwtSecurityKey | Security Key to sign the JWT. Make this very long and unique |
JwTokenGenerator__JwtIssuer | Jwt issuer |
JwTokenGenerator__JwtAudience | JWT Audience |
JwTokenGenerator__JwtExpiryInSeconds | Set the expiry seconds of the JWT |
OrientDb__OrientDbServerUrl | Specify the OrientDB server URL. In Docker this is not local host |
OrientDb__StateDatabaseName | Specify the name of the state database for TESTAR |
OrientDb__MultiDatabaseSupport | Specify whether the server should support multi database support upon login |