#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR .
COPY ["src/OSItemIndex.Aggregator/OSItemIndex.Aggregator.csproj", "src/OSItemIndex.Aggregator/"]
COPY ["src/OSItemIndex.Data/OSItemIndex.Data.csproj", "src/OSItemIndex.Data/"]
RUN dotnet restore "src/OSItemIndex.Aggregator/OSItemIndex.Aggregator.csproj"
COPY . .
WORKDIR "/src/OSItemIndex.Aggregator"
RUN dotnet build "OSItemIndex.Aggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OSItemIndex.Aggregator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OSItemIndex.Aggregator.dll"]
