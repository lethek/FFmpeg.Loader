FROM mcr.microsoft.com/dotnet/runtime:6.0-jammy AS base
WORKDIR /app

RUN apt-get update
RUN apt-get install -y ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["samples/DockerExternalFFmpeg4Sample/DockerExternalFFmpeg4Sample.csproj", "samples/DockerExternalFFmpeg4Sample/"]
RUN dotnet restore "samples/DockerExternalFFmpeg4Sample/DockerExternalFFmpeg4Sample.csproj"
COPY . .
WORKDIR "/src/samples/DockerExternalFFmpeg4Sample"
RUN dotnet build "DockerExternalFFmpeg4Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerExternalFFmpeg4Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerExternalFFmpeg4Sample.dll"]