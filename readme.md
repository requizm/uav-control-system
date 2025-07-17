# Local DB

```shell
docker run --name uav-postgres -e POSTGRES_PASSWORD=your_strong_password -p 5432:5432 -v uav-db-data:/var/lib/postgresql/data -d postgres
```

# Migration

Initial migration:
```
dotnet ef migrations add InitialCreate --project src/Uav.Infrastructure --startup-project src/Uav.Control.Api
```

Update DB
```
dotnet ef database update --startup-project src/Uav.Control.Api
```
