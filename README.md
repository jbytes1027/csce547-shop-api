# Shop API
A shopping website API build by a team of four for an ASP.NET class project. The routes and endpoints implement a given schema.

## Postgres Setup
Install and start the database by installing docker and running the following command.

    docker run --name postgres-547 -e POSTGRES_DB=shop_dev -e POSTGRES_PASSWORD=dev -e POSTGRES_USER=dev -p 5432:5432 postgres:alpine

To start the database later, run:

    docker container start postgres-547

To apply the latest database changes, run:

    dotnet ef database update

Create a `.env` file in `ShopAPI` with the connection string formatted as follows:

    CONNECTION_STRING="Host=localhost; Database=shop_dev; Username=dev; Password=dev"

Alternatively, add the connection string to the environment variables.

## Running

To start the server, run `dotnet run` in `ShopAPI/ShopAPI`.

To test the server, run `dotnet test` in `ShopAPI/ShopAPI.Tests`.
