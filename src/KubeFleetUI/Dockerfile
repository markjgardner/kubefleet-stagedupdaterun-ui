FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["src/KubeFleetUI/KubeFleetUI.csproj", "src/KubeFleetUI/"]
RUN dotnet restore "src/KubeFleetUI/KubeFleetUI.csproj"
COPY . .
WORKDIR "/src/src/KubeFleetUI"
RUN dotnet build "KubeFleetUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KubeFleetUI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KubeFleetUI.dll"]
