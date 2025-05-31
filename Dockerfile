# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

# Copy everything and restore
COPY . ./
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o /app/out

# Use a runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

# Copy the published app
COPY --from=build /app/out ./

# Expose the port Render will use
EXPOSE 8080

# Tell ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "ruhanBack.dll"]
