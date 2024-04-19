# Fx.Data.SQL
A Library for Generic CRUD &amp; Various Query Execution using RepoDB

## Docker Command To build SQL API
`docker build --tag fx-data-apis-app:v0.1 -f SQLAPI-Dockerfile .`

## Docker Command to Run SQL API
`docker run --rm -it -p 8021:80/tcp -e ASPNETCORE_URLS=http://+:80 -e ASPNETCORE_HTTP_PORTS=8021 5e84b1a6b822 --name=SQL-Data-APIs`

## Test URL
1. http://localhost:8021/swagger/index.html