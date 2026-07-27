FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Services/InventoryService/ Services/InventoryService/
RUN dotnet restore Services/InventoryService/InventoryService.Api.csproj
RUN dotnet publish Services/InventoryService/InventoryService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_Kestrel__EndpointDefaults__Protocols=Http2
EXPOSE 8080
ENTRYPOINT ["dotnet", "InventoryService.Api.dll"]
