# DB-PROJECT-WMS

## Database Setup

Copy data to the docker container:
```bash
docker cp Database/data/. wms-db:/var/opt/mssql/
```

Run the following command:

```bash
sqlcmd -S localhost,1433 -U sa -P password_goes_here -d WheresMyScheduleDB -i "00_RUN_ALL.sql" -b
```

Make sure to replace `password_goes_here` with your actual SQL Server password.