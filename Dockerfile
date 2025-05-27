FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PlanyApp.API/PlanyApp.API.csproj", "PlanyApp.API/"]
COPY ["PlanyApp.Service/PlanyApp.Service.csproj", "PlanyApp.Service/"]
COPY ["PlanyApp.Repository/PlanyApp.Repository.csproj", "PlanyApp.Repository/"]
RUN dotnet restore "PlanyApp.API/PlanyApp.API.csproj"
COPY . .
WORKDIR "/src/PlanyApp.API"
RUN dotnet build "PlanyApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlanyApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlanyApp.API.dll"] 