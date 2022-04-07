# Group 2 Services

## Event Bus
A messaging service that connects different microservices over a channels and stores the sent messages in it's database.

## Sensor Data Service
A microservice that receives the data from the sensors via the mobile app, converts it to normalized data and stores it in it's database.

## Raw Data Service
A microservice that retreives the normalized sensor data from it's database.

## Technical Health Dashboard
A frontend that shows the system functionality of all the microservices, including logs, warnings and errors.

## Technical Health Service
A microservice that monitors the sysem functionality of all the microservices with a hearthbeat and stores their logs, warnings and errors in it's database.

# How to use

## Run all containers using docker-compose
```
docker-compose pull && docker-compose up -d
```

## See all containers
```
docker ps
```

## See all images
```
docker images
```

## See all networks
```
docker network ls
```

## Stop all containers
```
docker-compose stop
```

## Delete all containers and networks
```
docker-compose down
```
