# Use the official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore and publish the specific project
RUN dotnet restore "IoTDeviceWebAPI.sln"
RUN dotnet publish "IoTDeviceWebAPI.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
# Set the entry point to the compiled DLL
ENTRYPOINT ["dotnet", "IoTDeviceWebAPI.dll"]
