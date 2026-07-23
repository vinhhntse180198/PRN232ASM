FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Services/InferenceService/ Services/InferenceService/
RUN dotnet restore Services/InferenceService/InferenceService.Api.csproj
RUN dotnet publish Services/InferenceService/InferenceService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_Kestrel__EndpointDefaults__Protocols=Http2
EXPOSE 8080
ENTRYPOINT ["dotnet", "InferenceService.Api.dll"]
