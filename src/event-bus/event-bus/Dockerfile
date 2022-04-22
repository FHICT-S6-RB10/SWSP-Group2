#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["event-bus/event-bus.csproj", "event-bus/"]
RUN dotnet restore "event-bus/event-bus.csproj"
COPY . .
WORKDIR "/src/event-bus"
RUN dotnet build "event-bus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "event-bus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "event-bus.dll"]