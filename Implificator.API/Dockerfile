

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app

EXPOSE 5002
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5002
ENV ASPNETCORE_HTTPS_PORT=https://+:5001
ENV ASPNETCORE_ENVIRONMENT=Development


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Implificator.API/Implificator.API.csproj", "Implificator.API/"]
RUN dotnet restore "Implificator.API/Implificator.API.csproj"
COPY . .
WORKDIR "/src/Implificator.API"
RUN dotnet build "Implificator.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Implificator.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Implificator.API.dll"]