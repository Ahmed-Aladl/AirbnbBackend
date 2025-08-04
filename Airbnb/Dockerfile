# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY ["AirbnbBackend.sln", "."]
COPY ["Airbnb/", "Airbnb/"]
COPY ["Application/", "Application/"]
COPY ["Domain/", "Domain/"]
COPY ["Infrastructure/", "Infrastructure/"]

RUN dotnet restore "AirbnbBackend.sln"

# Build and publish
RUN dotnet publish "Airbnb/Airbnb.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS final
WORKDIR /app

# Copy .env file if needed
COPY --from=build /app/publish .

# Set environment variable loading (if using .env manually in code)
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "Airbnb.dll"]
