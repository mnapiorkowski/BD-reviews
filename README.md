# BD - "Tylko/Teraz Rock" reviews

Databases - 3. semester @ MIMUW

## How to run

```
psql -h localhost -p 5432 -U postgres -f db/initDB.sql
psql -h localhost -p 5432 -U postgres reviews < db/reviewsDB.sql
cd src
dotnet build && dotnet run
```
