#Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish src/KrakenDB.Server/KrakenDB.Server.csproj -c Release -o out

#Run
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 5432
ENTRYPOINT ["dotnet", "KrakenDB.Server.dll"]