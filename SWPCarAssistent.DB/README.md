## Starting database
Following command, using docker and yaml file, will run Postgres sql database on port 5432.
```sh
docker-compose -f postgres.yaml up
```
Database name:
```
car_assistent
```
Login:password
```sh
car_assistent:car_assistent
```

## Starting pgAdmin
pgAdmin is tool that you can use to manage database using browser. In order to start it, run following command.
```sh
docker-compose -f pgadmin.yaml up
```

## Init database files
Any files in ```dbinit``` folder, will be used to initialize database, when container is started for the first time.
