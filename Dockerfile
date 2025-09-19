# Use the official ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SharpHooks.csproj", "."]
RUN dotnet restore "SharpHooks.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "SharpHooks.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "SharpHooks.csproj" -c Release -o /app/publish

# Create the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directories for hooks configuration and scripts
RUN mkdir -p /app/Scripts

# Copy hooks configuration
COPY hooks.json .
COPY Scripts/ Scripts/

ENTRYPOINT ["dotnet", "SharpHooks.dll"]