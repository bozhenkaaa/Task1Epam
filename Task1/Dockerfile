﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Task1/Task1.csproj", "Task1/"]
RUN dotnet restore "Task1/Task1.csproj"
COPY . .
WORKDIR "/src/Task1"
RUN dotnet build "Task1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Task1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Task1.dll"]
