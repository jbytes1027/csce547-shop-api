# PalmetoProgrammers547

## Environment Setup
### Starting Postgres
Install and start the DB by installing docker and running the following command.
```
docker run --name postgres-547 -e POSTGRES_DB=shop_dev -e POSTGRES_PASSWORD=dev -e POSTGRES_USER=dev -p 5432:5432 postgres:alpine
```

Alternativly, install and start a postgreSQL db manually.

### Setup Postgres
Run the following command to apply the latest db changes.
```
dotnet ef database update
```

### Connect to Postgres
Create a `.env` file that contains the connection string in `ShopAPI`. The format is as follows:
```
CONNECTION_STRING="Host=localhost; Database=shop_dev; Username=dev; Password=dev"
```

Alternativly, add the connection string to the environment vars.
