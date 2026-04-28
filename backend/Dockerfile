FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY UpStatus.Domain/UpStatus.Domain.csproj UpStatus.Domain/
COPY UpStatus.Application/UpStatus.Application.csproj UpStatus.Application/
COPY UpStatus.Infrastructure/UpStatus.Infrastructure.csproj UpStatus.Infrastructure/
COPY UpStatus.Api/UpStatus.Api.csproj UpStatus.Api/
RUN dotnet restore UpStatus.Api/UpStatus.Api.csproj
COPY . .
WORKDIR /src/UpStatus.Api
RUN dotnet build UpStatus.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish UpStatus.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UpStatus.Api.dll"]
