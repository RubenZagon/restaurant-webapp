# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["RestaurantApp.API/RestaurantApp.API.csproj", "RestaurantApp.API/"]
COPY ["RestaurantApp.Application/RestaurantApp.Application.csproj", "RestaurantApp.Application/"]
COPY ["RestaurantApp.Domain/RestaurantApp.Domain.csproj", "RestaurantApp.Domain/"]
COPY ["RestaurantApp.Infrastructure/RestaurantApp.Infrastructure.csproj", "RestaurantApp.Infrastructure/"]

RUN dotnet restore "RestaurantApp.API/RestaurantApp.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/RestaurantApp.API"
RUN dotnet build "RestaurantApp.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "RestaurantApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Create non-root user for security
RUN addgroup --system --gid 1000 appuser && \
    adduser --system --uid 1000 --ingroup appuser --shell /bin/sh appuser

COPY --from=publish /app/publish .

# Switch to non-root user
USER appuser

ENTRYPOINT ["dotnet", "RestaurantApp.API.dll"]
