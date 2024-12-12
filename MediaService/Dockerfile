FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "Common/Common.csproj"

COPY ["MediaService/MediaService.csproj", "MediaService/"]
RUN dotnet restore "MediaService/MediaService.csproj"

COPY ["Common/", "Common/"]
COPY ["MediaService/", "MediaService/"]

WORKDIR "/src/MediaService"
RUN dotnet build "MediaService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MediaService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaService.dll"]