FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
RUN apt update
RUN apt install ffmpeg -y
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["ImmersionMirror/ImmersionMirror.csproj", "ImmersionMirror/"]
RUN dotnet restore "ImmersionMirror/ImmersionMirror.csproj"
COPY . .
WORKDIR "/src/ImmersionMirror"
RUN dotnet build "ImmersionMirror.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImmersionMirror.csproj" -c Release -o /app/publish

FROM base AS final
ENV DOTNET_USE_POLLING_FILE_WATCHER 1
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImmersionMirror.dll"]