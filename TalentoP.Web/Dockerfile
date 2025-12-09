FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TalentoP.Web/TalentoP.Web.csproj", "TalentoP.Web/"]
COPY ["2-Domain/2-Domain.csproj", "2-Domain/"]
COPY ["3-Infrastructure/3-Infrastructure.csproj", "3-Infrastructure/"]
COPY ["1-Application/1-Application.csproj", "1-Application/"]
RUN dotnet restore "TalentoP.Web/TalentoP.Web.csproj"
COPY . .
WORKDIR "/src/TalentoP.Web"
RUN dotnet build "./TalentoP.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TalentoP.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoP.Web.dll"]
