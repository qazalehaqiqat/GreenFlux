
## Build the project:

```
dontnet build

dotnet run
```

## Swagger

https://localhost:5001/swagger/index.html


## Run the tests:

```
// workdir: ./Demo.Test

dotnet test
```


## Create migration & database:

```
dotnet ef migrations add InitialCreate --project demo

dotnet ef database update --project demo
```
