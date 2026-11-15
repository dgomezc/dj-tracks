FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /source
COPY NuGet.Config ./
COPY src/DjTracks.Domain/ src/DjTracks.Domain/
COPY src/DjTracks.UI/ src/DjTracks.UI/
# Restaurar con los .razor presentes para incluir los recursos de Blazor.
RUN dotnet publish src/DjTracks.UI/DjTracks.UI.csproj -c Release -o /out -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS final
USER root
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys \
    && chmod 700 /home/app/.aspnet/DataProtection-Keys \
    && chown -R app:app /home/app/.aspnet
WORKDIR /app
COPY --from=build /out/ ./
ENV ASPNETCORE_HTTP_PORTS=8080 HOME=/home/app
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "DjTracks.UI.dll"]
