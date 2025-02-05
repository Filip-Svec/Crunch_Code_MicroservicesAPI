FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicroservicesAPI.Python/MicroservicesAPI.Python.csproj", "MicroservicesAPI.Python/"]
COPY ["MicroservicesAPI.Shared/MicroservicesAPI.Shared.csproj", "MicroservicesAPI.Shared/"]
RUN dotnet restore "MicroservicesAPI.Python/MicroservicesAPI.Python.csproj"

RUN apt-get update && \
    apt-get install -y python3 python3-pip && \
    python3 --version && \
    which python3
COPY . .
WORKDIR "/src/MicroservicesAPI.Python"
RUN dotnet build "MicroservicesAPI.Python.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroservicesAPI.Python.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copy Python installation from build stage
COPY --from=build /usr/bin/python3 /usr/bin/python3
COPY --from=build /usr/lib/python3.11 /usr/lib/python3.11

# Install missing shared libraries
RUN apt-get update && \
    apt-get install -y libexpat1 libssl3

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroservicesAPI.Python.dll"]
