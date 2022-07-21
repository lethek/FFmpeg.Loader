FROM mcr.microsoft.com/dotnet/runtime:6.0-jammy AS base
WORKDIR /app

RUN apt-get update
RUN apt-get install -y software-properties-common
RUN add-apt-repository ppa:savoury1/ffmpeg4 -y
RUN add-apt-repository ppa:savoury1/ffmpeg5 -y
RUN apt-get update
RUN apt-get upgrade -y
RUN apt-get install -y ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["samples/DockerExternalFFmpeg5Sample/DockerExternalFFmpeg5Sample.csproj", "samples/DockerExternalFFmpeg5Sample/"]
RUN dotnet restore "samples/DockerExternalFFmpeg5Sample/DockerExternalFFmpeg5Sample.csproj"
COPY . .
WORKDIR "/src/samples/DockerExternalFFmpeg5Sample"
RUN dotnet build "DockerExternalFFmpeg5Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerExternalFFmpeg5Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerExternalFFmpeg5Sample.dll"]