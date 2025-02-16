# The base image (runtime environment) - means that it comes with pre-existing stuff installed already
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base    
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Sdk for compilation of code 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copies .csproj file before dotnet restore to utilize Docker layer caching (avoids unnecessary restores)
COPY ["MicroservicesAPI.Python/MicroservicesAPI.Python.csproj", "MicroservicesAPI.Python/"]
COPY ["MicroservicesAPI.Shared/MicroservicesAPI.Shared.csproj", "MicroservicesAPI.Shared/"]

# Restore dependencies
RUN dotnet restore "MicroservicesAPI.Python/MicroservicesAPI.Python.csproj"

RUN apt-get update && \
    apt-get install -y python3 python3-pip && \
    python3 --version && \
    which python3 

# Copies the whole project from current dir (host) to working dir in container
COPY . .
WORKDIR "/src/MicroservicesAPI.Python"
RUN dotnet build "MicroservicesAPI.Python.csproj" -c Release -o /app/build

# Publishes the build
FROM build AS publish
RUN dotnet publish "MicroservicesAPI.Python.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Based on base img, runtime only (no sdk)
FROM base AS final
WORKDIR /app

# Copy Python installation from build stage
COPY --from=build /usr/bin/python3 /usr/bin/python3
COPY --from=build /usr/lib/python3.11 /usr/lib/python3.11

# Install missing shared libraries
RUN apt-get update && \
    apt-get install -y libexpat1 libssl3

# Copy built app from publish
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroservicesAPI.Python.dll"]
