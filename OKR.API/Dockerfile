# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/OKR.API
COPY ["OKR.API/OKR.API.csproj", "OKR.API/"]
COPY ["OKR.Models/OKR.Models.csproj", "OKR.Models/"]
COPY ["OKR.Repository/OKR.Repository.csproj", "OKR.Repository/"]
COPY ["OKR.DTO/OKR.DTO.csproj", "OKR.DTO/"]
COPY ["OKR.Service/OKR.Service.csproj", "OKR.Service/"]
COPY ["../Maynghien.Infrastructure/Maynghien.Infrastructure.csproj", "Maynghien.Infrastructure/"]
COPY ["../OKR.Infrastructure/OKR.Infrastructure.csproj", "OKR.Infrastructure/"]
RUN dotnet restore "OKR.API/OKR.API.csproj"
COPY . .
WORKDIR "/src/OKR.API"
RUN dotnet build "OKR.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OKR.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OKR.API.dll"]
