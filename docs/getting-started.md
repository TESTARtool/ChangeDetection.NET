# Getting started
Before the application be can run, the following information is needed:

- OrientDB location (e.g. http://localhost:2480)
- OrientDB database name (e.g. Testar)
- OrientDB username
- OrientDB password
- Control Application Name (eg. simpel_model)
- Control Application Version (eg. 1.0.0)
- Test Application Name (eg. simpel_model)
- Test Application Version (eg. 2.0.0)

The application can be executed with the following parameters: 

``` 
./ConsoleApp /Compare:ControlName=simpel_model /Compare:ControlVersion=1.0.0 /Compare:TestName=simpel_model /Compare:TestVersion=2.0.0 /OrientDb:Password=**********
```

It might be easier to set the OrientDB setting in the appsettings.json
```json
{
  "OrientDb": {
    "Url": "http://localhost:2480",
    "DatabaseName": "testar2",
    "UserName": "testar",
    "Password": "**********"
  }
}
```