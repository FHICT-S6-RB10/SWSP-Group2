#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["sensor-data-service.csproj", "."]
RUN dotnet restore "./sensor-data-service.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "sensor-data-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "sensor-data-service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sensor-data-service.dll"]