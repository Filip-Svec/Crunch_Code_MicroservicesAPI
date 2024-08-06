FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicroservicesAPI.JavaScript/MicroservicesAPI.JavaScript.csproj", "MicroservicesAPI.JavaScript/"]
RUN dotnet restore "MicroservicesAPI.JavaScript/MicroservicesAPI.JavaScript.csproj"
COPY . .
WORKDIR "/src/MicroservicesAPI.JavaScript"
RUN dotnet build "MicroservicesAPI.JavaScript.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroservicesAPI.JavaScript.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroservicesAPI.JavaScript.dll"]
