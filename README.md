
## Build and run the project:

```
dontnet build
// workdir: ./Demo
dotnet run
```

## Swagger

https://localhost:5001/swagger/index.html


## Run the tests:

```
// workdir: ./Demo.Test
dotnet test
```


## Create database:

```
dotnet ef migrations add InitialCreat --project demo

dotnet ef database update --project demo
```
