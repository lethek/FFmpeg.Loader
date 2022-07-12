FROM mcr.microsoft.com/dotnet/runtime:6.0-jammy AS base
WORKDIR /app

RUN apt-get update
RUN apt-get install -y ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["samples/DockerBundledFFmpeg4Sample/DockerBundledFFmpeg4Sample.csproj", "samples/DockerBundledFFmpeg4Sample/"]
RUN dotnet restore "samples/DockerBundledFFmpeg4Sample/DockerBundledFFmpeg4Sample.csproj"
COPY . .
WORKDIR "/src/samples/DockerBundledFFmpeg4Sample"
RUN dotnet build "DockerBundledFFmpeg4Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerBundledFFmpeg4Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerBundledFFmpeg4Sample.dll"]