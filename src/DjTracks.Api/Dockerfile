FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /source
COPY NuGet.Config ./
COPY src/DjTracks.Domain/DjTracks.Domain.csproj src/DjTracks.Domain/
COPY src/DjTracks.Infrastructure/DjTracks.Infrastructure.csproj src/DjTracks.Infrastructure/
COPY src/DjTracks.Api/DjTracks.Api.csproj src/DjTracks.Api/
RUN dotnet restore src/DjTracks.Api/DjTracks.Api.csproj
COPY src/DjTracks.Domain/ src/DjTracks.Domain/
COPY src/DjTracks.Infrastructure/ src/DjTracks.Infrastructure/
COPY src/DjTracks.Api/ src/DjTracks.Api/
RUN dotnet publish src/DjTracks.Api/DjTracks.Api.csproj -c Release --no-restore -o /out -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS final
USER root
RUN apt-get update && apt-get install -y --no-install-recommends ffmpeg \
    && apt-get clean && rm -rf /var/lib/apt/lists \
    && mkdir -p /app/App_Data && chown app:app /app/App_Data
WORKDIR /app
COPY --from=build /out/ ./
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "DjTracks.Api.dll"]
