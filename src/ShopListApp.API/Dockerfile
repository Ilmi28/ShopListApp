# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5181
EXPOSE 7278


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ShopListApp.API/ShopListApp.API.csproj", "src/ShopListApp.API/"]
COPY ["src/ShopListApp.Application/ShopListApp.Application.csproj", "src/ShopListApp.Application/"]
COPY ["src/ShopListApp.Core/ShopListApp.Core.csproj", "src/ShopListApp.Core/"]
COPY ["src/ShopListApp.Infrastructure/ShopListApp.Infrastructure.csproj", "src/ShopListApp.Infrastructure/"]
RUN dotnet restore "./src/ShopListApp.API/ShopListApp.API.csproj"
COPY . .
WORKDIR "/src/src/ShopListApp.API"
RUN dotnet build "./ShopListApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShopListApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShopListApp.API.dll"]