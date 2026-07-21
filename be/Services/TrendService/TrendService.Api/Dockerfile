FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BuildingBlocks/ BuildingBlocks/
COPY Services/TrendService/ Services/TrendService/

RUN dotnet restore Services/TrendService/TrendService.Api/TrendService.Api.csproj
RUN dotnet publish Services/TrendService/TrendService.Api/TrendService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "TrendService.Api.dll"]
