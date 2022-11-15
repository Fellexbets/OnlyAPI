#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Igor_AIS_Proj/Igor_AIS_Proj.csproj", "Igor_AIS_Proj/"]
RUN dotnet restore "Igor_AIS_Proj/Igor_AIS_Proj.csproj"
COPY . .
WORKDIR "/src/Igor_AIS_Proj"
RUN dotnet build "Igor_AIS_Proj.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Igor_AIS_Proj.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Igor_AIS_Proj.dll"]