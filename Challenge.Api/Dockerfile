#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
# Expose port 8888
EXPOSE 8888

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Challenge.Api/Challenge.Api.csproj", "Challenge.Api/"]
COPY ["Challenge.Service/Challenge.Service.csproj", "Challenge.Service/"]
RUN dotnet restore "Challenge.Api/Challenge.Api.csproj"
COPY . .
WORKDIR "/src/Challenge.Api"
RUN dotnet build "Challenge.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Challenge.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Challenge.Api.dll", "--urls", "http://0.0.0.0:8888"]

# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
