# EppoiOrleans
[![Made with .NET](https://img.shields.io/badge/Made%20with-.NET-orange)](https://dotnet.microsoft.com/en-us/)
[![Made with Orleans](https://img.shields.io/badge/Made%20with-Orleans-purple)](https://learn.microsoft.com/it-it/dotnet/orleans/)

## How to run
> To start redis service for grains persistence
```bash
docker compose -f .\EppoiBackend\docker-compose.yaml up -d
```
> To start Orleans Silo (Backend WebApi)
```bash
dotnet run --project .\EppoiBackend\EppoiBackend.csproj
```
