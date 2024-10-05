FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicroservicesAPI.Python/MicroservicesAPI.Python.csproj", "MicroservicesAPI.Python/"]
COPY ["MicroservicesAPI.Shared/MicroservicesAPI.Shared.csproj", "MicroservicesAPI.Shared/"]
RUN dotnet restore "MicroservicesAPI.Python/MicroservicesAPI.Python.csproj"
COPY . .
WORKDIR "/src/MicroservicesAPI.Python"
RUN dotnet build "MicroservicesAPI.Python.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroservicesAPI.Python.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroservicesAPI.Python.dll"]
