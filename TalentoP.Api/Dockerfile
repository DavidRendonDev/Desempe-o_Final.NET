FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TalentoP.Api/TalentoP.Api.csproj", "TalentoP.Api/"]
COPY ["1-Application/1-Application.csproj", "1-Application/"]
COPY ["2-Domain/2-Domain.csproj", "2-Domain/"]
COPY ["3-Infrastructure/3-Infrastructure.csproj", "3-Infrastructure/"]
RUN dotnet restore "TalentoP.Api/TalentoP.Api.csproj"
COPY . .
WORKDIR "/src/TalentoP.Api"
RUN dotnet build "./TalentoP.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TalentoP.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoP.Api.dll"]
