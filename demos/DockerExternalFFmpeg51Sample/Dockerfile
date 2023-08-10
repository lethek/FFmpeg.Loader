#Get Ubuntu Kinetic Kudu since it's the only one that has FFmpeg 5.1.1 in its official universe repo
ARG UBUNTU_VERSION=22.10
FROM ubuntu:${UBUNTU_VERSION} as ubuntu-base
RUN apt-get update

#Install FFmpeg 5.1.1
FROM ubuntu-base as ffmpeg-base
ARG FFMPEG_VERSION=5.1.1-1ubuntu2.1
ENV FFMPEG_VERSION=${FFMPEG_VERSION}
RUN apt-get install -y ffmpeg=7:${FFMPEG_VERSION}

#Install .NET 6.0
FROM ffmpeg-base as dotnet-base
ARG DOTNET_VERSION=6.0
ENV DOTNET_VERSION=${DOTNET_VERSION}
RUN apt-get install -y dotnet-runtime-${DOTNET_VERSION} ca-certificates
RUN rm -rf /var/lib/apt/lists/*
ENV DOTNET_RUNNING_IN_CONTAINER=true

#Build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["demos/DockerExternalFFmpeg51Sample/DockerExternalFFmpeg51Sample.csproj", "demos/DockerExternalFFmpeg51Sample/"]
RUN dotnet restore "demos/DockerExternalFFmpeg51Sample/DockerExternalFFmpeg51Sample.csproj"
COPY . .
WORKDIR "/src/demos/DockerExternalFFmpeg51Sample"
RUN dotnet build "DockerExternalFFmpeg51Sample.csproj" -c Release -o /app/build

#Publish the app
FROM build AS publish
RUN dotnet publish "DockerExternalFFmpeg51Sample.csproj" -c Release -o /app/publish

FROM dotnet-base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerExternalFFmpeg51Sample.dll"]
