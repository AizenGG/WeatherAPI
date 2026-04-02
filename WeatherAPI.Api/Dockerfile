FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WeatherAPI.Api/WeatherAPI.Api.csproj", "WeatherAPI.Api/"]
COPY ["src/WeatherAPI.Application/WeatherAPI.Application.csproj", "src/WeatherAPI.Application/"]
COPY ["src/WeatherAPI.Domain/WeatherAPI.Domain.csproj", "src/WeatherAPI.Domain/"]
COPY ["src/WeatherAPI.Infrastructure/WeatherAPI.Infrastructure.csproj", "src/WeatherAPI.Infrastructure/"]
RUN dotnet restore "WeatherAPI.Api/WeatherAPI.Api.csproj"
COPY . .
WORKDIR "/src/WeatherAPI.Api"
RUN dotnet build "./WeatherAPI.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WeatherAPI.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherAPI.Api.dll"]
