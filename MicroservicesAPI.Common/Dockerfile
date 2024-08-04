FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicroservicesAPI.Common/MicroservicesAPI.Common.csproj", "MicroservicesAPI.Common/"]
RUN dotnet restore "MicroservicesAPI.Common/MicroservicesAPI.Common.csproj"
COPY . .
WORKDIR "/src/MicroservicesAPI.Common"
RUN dotnet build "MicroservicesAPI.Common.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroservicesAPI.Common.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroservicesAPI.Common.dll"]
