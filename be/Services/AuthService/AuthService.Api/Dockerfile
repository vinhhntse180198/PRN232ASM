FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BuildingBlocks/ BuildingBlocks/
COPY Services/AuthService/ Services/AuthService/

RUN dotnet restore Services/AuthService/AuthService.Api/AuthService.Api.csproj
RUN dotnet publish Services/AuthService/AuthService.Api/AuthService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "AuthService.Api.dll"]
