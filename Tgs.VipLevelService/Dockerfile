#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.VipLevelService/Tgs.VipLevelService.csproj", "Tgs.VipLevelService/"]
COPY ["Tgs.VipLevelService.Core/Tgs.VipLevelService.Core.csproj", "Tgs.VipLevelService.Core/"]
COPY ["Tgs.VipLevelService.Repositories/Tgs.VipLevelService.Repositories.csproj", "Tgs.VipLevelService.Repositories/"]
RUN dotnet restore "./Tgs.VipLevelService/./Tgs.VipLevelService.csproj"
COPY . .
WORKDIR "/src/Tgs.VipLevelService"
RUN dotnet build "./Tgs.VipLevelService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.VipLevelService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.VipLevelService.dll"]