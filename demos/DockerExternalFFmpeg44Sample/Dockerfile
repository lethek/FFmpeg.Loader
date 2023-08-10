FROM mcr.microsoft.com/dotnet/runtime:6.0-jammy AS base
WORKDIR /app

RUN apt-get update
RUN apt-get install -y ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["demos/DockerExternalFFmpeg44Sample/DockerExternalFFmpeg44Sample.csproj", "demos/DockerExternalFFmpeg44Sample/"]
RUN dotnet restore "demos/DockerExternalFFmpeg44Sample/DockerExternalFFmpeg44Sample.csproj"
COPY . .
WORKDIR "/src/demos/DockerExternalFFmpeg44Sample"
RUN dotnet build "DockerExternalFFmpeg44Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerExternalFFmpeg44Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerExternalFFmpeg44Sample.dll"]