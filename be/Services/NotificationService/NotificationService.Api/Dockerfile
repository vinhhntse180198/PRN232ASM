FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BuildingBlocks/ BuildingBlocks/
COPY Services/NotificationService/ Services/NotificationService/

RUN dotnet restore Services/NotificationService/NotificationService.Api/NotificationService.Api.csproj
RUN dotnet publish Services/NotificationService/NotificationService.Api/NotificationService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "NotificationService.Api.dll"]
