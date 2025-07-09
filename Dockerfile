# dokcerfile for Dokcer ACR deployment...

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# Copy solution and project files
COPY *.sln .
COPY WebApp/*.csproj ./WebApp/
RUN dotnet restore

# Copy the rest of the source code and publish
COPY WebApp/. ./WebApp/
WORKDIR /source/WebApp
RUN dotnet publish -c Release -o /app --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "WebApp.dll"]
