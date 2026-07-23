FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BuildingBlocks/ BuildingBlocks/
COPY Services/PaperService/ Services/PaperService/
COPY Services/RecommendationService/Protos/ Services/RecommendationService/Protos/

RUN dotnet restore Services/PaperService/PaperService.Api/PaperService.Api.csproj
RUN dotnet publish Services/PaperService/PaperService.Api/PaperService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "PaperService.Api.dll"]
