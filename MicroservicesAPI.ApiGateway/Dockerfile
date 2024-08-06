FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicroservicesAPI.ApiGateway/MicroservicesAPI.ApiGateway.csproj", "MicroservicesAPI.ApiGateway/"]
RUN dotnet restore "MicroservicesAPI.ApiGateway/MicroservicesAPI.ApiGateway.csproj"
COPY . .
WORKDIR "/src/MicroservicesAPI.ApiGateway"
RUN dotnet build "MicroservicesAPI.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroservicesAPI.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroservicesAPI.ApiGateway.dll"]
