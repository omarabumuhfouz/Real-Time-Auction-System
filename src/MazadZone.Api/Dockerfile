# --- STAGE 1: Base Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- STAGE 2: Build and Restore ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files using the local src/ layout
COPY ["src/MazadZone.Api/MazadZone.Api.csproj", "MazadZone.Api/"]
COPY ["src/MazadZone.Application/MazadZone.Application.csproj", "MazadZone.Application/"]
COPY ["src/MazadZone.Domain/MazadZone.Domain.csproj", "MazadZone.Domain/"]
COPY ["src/MazadZone.Infrastructure/MazadZone.Infrastructure.csproj", "MazadZone.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "MazadZone.Api/MazadZone.Api.csproj"

# Copy all repository source files
COPY . .

# Move into the actual API folder (Notice the double src/ layout inside the container)
WORKDIR "/src/src/MazadZone.Api"

# Build the application
RUN dotnet build "MazadZone.Api.csproj" -c Release -o /app/build

# --- STAGE 3: Publish ---
FROM build AS publish
RUN dotnet publish "MazadZone.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- STAGE 4: Final Image ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MazadZone.Api.dll"]