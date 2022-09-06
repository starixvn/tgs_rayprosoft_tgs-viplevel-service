#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.VipLevelService/Tgs.VipLevelService.csproj", "Tgs.VipLevelService/"]
COPY ["Tgs.VipLevelService.Core/Tgs.VipLevelService.Core.csproj", "Tgs.VipLevelService.Core/"]
COPY ["Tgs.VipLevelService.Repositories/Tgs.VipLevelService.Repositories.csproj", "Tgs.VipLevelService.Repositories/"]
RUN dotnet restore "Tgs.VipLevelService/Tgs.VipLevelService.csproj"
COPY . .
WORKDIR "/src/Tgs.VipLevelService"
RUN dotnet build "Tgs.VipLevelService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.VipLevelService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.VipLevelService.dll"]