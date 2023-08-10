FROM mcr.microsoft.com/dotnet/runtime:6.0-jammy AS base
WORKDIR /app

RUN apt-get update
RUN apt-get install -y ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["demos/DockerBundledFFmpeg44Sample/DockerBundledFFmpeg44Sample.csproj", "demos/DockerBundledFFmpeg44Sample/"]
RUN dotnet restore "demos/DockerBundledFFmpeg44Sample/DockerBundledFFmpeg44Sample.csproj"
COPY . .
WORKDIR "/src/demos/DockerBundledFFmpeg44Sample"
RUN dotnet build "DockerBundledFFmpeg44Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerBundledFFmpeg44Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerBundledFFmpeg44Sample.dll"]