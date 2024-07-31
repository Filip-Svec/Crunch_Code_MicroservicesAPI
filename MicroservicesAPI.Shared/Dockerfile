FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicroservicesAPI.Shared/MicroservicesAPI.Shared.csproj", "MicroservicesAPI.Shared/"]
RUN dotnet restore "MicroservicesAPI.Shared/MicroservicesAPI.Shared.csproj"
COPY . .
WORKDIR "/src/MicroservicesAPI.Shared"
RUN dotnet build "MicroservicesAPI.Shared.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroservicesAPI.Shared.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroservicesAPI.Shared.dll"]
