# MySampleApi/Dockerfile

# Stage 1: Build the application
# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src/MySampleApi # Set the working directory to the project folder inside the container

# Copy the project file first to leverage Docker layer caching for dotnet restore
# This copies MySampleApi.csproj from the build context (your local MySampleApi folder)
# to the current working directory in the container (/src/MySampleApi/)
COPY MySampleApi.csproj ./

# Restore NuGet packages for the project
RUN dotnet restore

# Copy the rest of the application source code from your local MySampleApi folder
# to the current working directory in the container (/src/MySampleApi/)
COPY . .

# Publish the application for release
# -c Release: Specifies the Release configuration
# -o /app/publish: Specifies the output directory for the published application within the container
# /p:UseAppHost=false: Prevents creation of a single-file executable, common for containerized web apps
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Create the final runtime image
# Use the official .NET ASP.NET runtime image, which is smaller and optimized for running apps
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app # Set the working directory for the final image

# Copy the published application from the build stage into the final image
COPY --from=build /app/publish .

# Expose port 8080, which is the default for ASP.NET Core applications in containers
EXPOSE 8080

# Define the entry point for the container
# This command will be executed when the container starts, running your compiled API
ENTRYPOINT ["dotnet", "MySampleApi.dll"]
