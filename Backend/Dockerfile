# =====================================
# Stage 1: Build
# =====================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and project files
COPY ["VendorRisk.sln", "./"]
COPY ["src/VendorRisk.Api/VendorRisk.Api.csproj", "src/VendorRisk.Api/"]
COPY ["src/VendorRisk.Application/VendorRisk.Application.csproj", "src/VendorRisk.Application/"]
COPY ["src/VendorRisk.Domain/VendorRisk.Domain.csproj", "src/VendorRisk.Domain/"]
COPY ["src/VendorRisk.Infrastructure/VendorRisk.Infrastructure.csproj", "src/VendorRisk.Infrastructure/"]
COPY ["src/VendorRisk.Tests/VendorRisk.Tests.csproj", "src/VendorRisk.Tests/"]

# Restore dependencies
RUN dotnet restore "VendorRisk.sln"

# Copy the rest of the source code
COPY . .

# Build the API project
WORKDIR "/src/src/VendorRisk.Api"
RUN dotnet build "VendorRisk.Api.csproj" -c Release -o /app/build

# =====================================
# Stage 2: Publish
# =====================================
FROM build AS publish
RUN dotnet publish "VendorRisk.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =====================================
# Stage 3: Runtime
# =====================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Install curl for healthcheck
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN addgroup --system --gid 1001 dotnet && \
    adduser --system --uid 1001 --ingroup dotnet dotnetuser

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Create directories for logs and data
RUN mkdir -p /app/Logs /app/Data && \
    chown -R dotnetuser:dotnet /app

# Switch to non-root user
USER dotnetuser

# Expose port
EXPOSE 8080

# Set ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=5 \
  CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "VendorRisk.Api.dll"]
