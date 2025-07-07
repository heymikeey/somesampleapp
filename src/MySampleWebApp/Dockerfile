# Stage 1: Build the ASP.NET Core Web Application
# Use the official .NET SDK image for building the backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the MySampleWebApp project file and restore dependencies
# This leverages Docker's layer caching for faster builds if the project file doesn't change
COPY ./MySampleWebApp.csproj MySampleWebApp/
RUN dotnet restore MySampleWebApp/MySampleWebApp.csproj

# Copy the entire MySampleWebApp directory, including the pre-built wwwroot content
# The Angular build output MUST be in MySampleWebApp/wwwroot before this step
COPY . ./MySampleWebApp
WORKDIR "/src/MySampleWebApp"

# Publish the ASP.NET Core application
# This will include the contents of the wwwroot folder (your Angular app)
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Create the final runtime image
# Use the official .NET ASP.NET runtime image, which is smaller and optimized for running apps
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Explicitly set the ASP.NET Core URLs to listen on HTTP port 80
ENV ASPNETCORE_URLS="http://+:8080"
# Set the ASP.NET Core environment to Development for local Docker runs (can be changed for production)
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port 8080
EXPOSE 8080

# Define the entry point for the container
ENTRYPOINT ["dotnet", "MySampleWebApp.dll"]
