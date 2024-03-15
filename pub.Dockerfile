FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./Core/.", "Core/"]

WORKDIR /src/Core
 
RUN dotnet restore "Core.csproj"
RUN dotnet build "Core.csproj" -c Release -o /app/build

WORKDIR /src
COPY ["./pub/.", "pub/"]

WORKDIR /src/pub
RUN dotnet restore "pub.csproj"
RUN dotnet build "pub.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /src/pub
RUN dotnet publish "pub.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pub.dll"]