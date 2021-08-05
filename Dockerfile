FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS publish
WORKDIR /app
COPY . .
WORKDIR /app/src/OSItemIndex.Updater
RUN dotnet publish "OSItemIndex.Updater.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OSItemIndex.Updater.dll"]
