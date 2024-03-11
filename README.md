# EppoiOrleans
[![Made with .NET](https://img.shields.io/badge/Made%20with-.NET-orange)](https://dotnet.microsoft.com/en-us/)
[![Made with Orleans](https://img.shields.io/badge/Made%20with-Orleans-purple)](https://learn.microsoft.com/it-it/dotnet/orleans/)

<br />

EppoiOrleans is a simple readjustment of an existing project called Eppoi that we presented for bachelor thesis and for Complex System Design course.
**For more details take a look to [Eppoi Abstract](Eppoi-Abstract.pdf).**
Our aim is not to recreate it completely but to produce a simple skeleton, based on that project, using Microsoft Orleans.
At the moment it provides APIs for:

- *Handling Itineraries* : An itinerary can be created, updated and deleted by a User.
- *Handling POIs* : A POI can be created, updated and deleted by an Ente.

## How to run
> To start redis service for grains persistence
```bash
docker compose -f .\EppoiBackend\docker-compose.yaml up -d
```
> To start Orleans Silo (Backend WebApi)
```bash
dotnet run --project .\EppoiBackend\EppoiBackend.csproj
```

## API Reference
For details about APIs reference, once you started the application, move on  **[Swagger UI](http://localhost:5139/swagger/index.html)**.
