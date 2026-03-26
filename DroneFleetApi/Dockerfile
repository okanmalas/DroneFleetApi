# 1. Aşama: İnşa etme (Build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["DroneFleetApi/DroneFleetApi.csproj", "DroneFleetApi/"]
RUN dotnet restore "DroneFleetApi/DroneFleetApi.csproj"
COPY . .
WORKDIR "/src/DroneFleetApi"
RUN dotnet publish "DroneFleetApi.csproj" -c Release -o /app

# 2. Aşama: Çalıştırma (Run)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# Uygulamanın 8080 portundan dışarı açılmasını sağlar (.NET 8 standardı)
ENV ASPNETCORE_HTTP_PORTS=8080 
ENTRYPOINT ["dotnet", "DroneFleetApi.dll"]