#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Clientes/Clientes.Api/Clientes.Api.csproj", "src/Clientes/Clientes.Api/"]
COPY ["src/Clientes/Clientes.Application/Clientes.Application.csproj", "src/Clientes/Clientes.Application/"]
COPY ["src/Clientes/Clientes.Domain/Clientes.Domain.csproj", "src/Clientes/Clientes.Domain/"]
COPY ["src/Clientes/Clientes.Infrastructure/Clientes.Infrastructure.csproj", "src/Clientes/Clientes.Infrastructure/"]
RUN dotnet restore "./src/Clientes/Clientes.Api/./Clientes.Api.csproj"
COPY . .
WORKDIR "/src/src/Clientes/Clientes.Api"
RUN dotnet build "./Clientes.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Clientes.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clientes.Api.dll"]