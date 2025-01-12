# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . . 
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /app/out

# Use the ASP.NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port and run the application
EXPOSE 5001
ENTRYPOINT ["dotnet", "backend.dll"]
