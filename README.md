Buils the project:

dontnet build
dotnet run
##########################

Run the tests:

cd Demo.Test
dotnet test
##########################

Swagger address:

https://localhost:5001/swagger/index.html

##########################

Create migration & database:

dotnet ef migrations add InitialCreat --project demo
dotnet ef database update --project demo