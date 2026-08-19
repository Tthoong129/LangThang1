# ----------------------------------------------------
# Stage 1: Build & Publish
# ----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["MiniMap.csproj", "./"]
RUN dotnet restore "./MiniMap.csproj"

# Copy all source code and publish
COPY . .
RUN dotnet publish "MiniMap.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ----------------------------------------------------
# Stage 2: Runtime
# ----------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "MiniMap.dll"]
