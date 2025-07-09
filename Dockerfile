# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# Copy solution and project files
COPY IoTDeviceWebAPI.sln ./
COPY IoTDeviceWebAPI/IoTDeviceWebAPI.csproj ./IoTDeviceWebAPI/

# Restore dependencies
RUN dotnet restore

# Copy the entire project and build
COPY IoTDeviceWebAPI/. ./IoTDeviceWebAPI/
WORKDIR /source/IoTDeviceWebAPI
RUN dotnet publish -c Release -o /app --no-restore

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "IoTDeviceWebAPI.dll"]
