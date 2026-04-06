# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution and project files
COPY ["BookLAB.sln", "."]
COPY ["src/BookLAB.API/BookLAB.API.csproj", "src/BookLAB.API/"]
COPY ["src/BookLAB.Application/BookLAB.Application.csproj", "src/BookLAB.Application/"]
COPY ["src/BookLAB.Domain/BookLAB.Domain.csproj", "src/BookLAB.Domain/"]
COPY ["src/BookLAB.Infrastructure/BookLAB.Infrastructure.csproj", "src/BookLAB.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "BookLAB.sln"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/src/BookLAB.API"
RUN dotnet build "BookLAB.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "BookLAB.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

RUN mkdir -p /app/credentials

# Install tzdata for timezone support
RUN apt-get update && apt-get install -y tzdata && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

EXPOSE 8080
EXPOSE 8443

# Set timezone
ENV TZ=Asia/Ho_Chi_Minh

# Run the app
ENTRYPOINT ["dotnet", "BookLAB.API.dll"]
