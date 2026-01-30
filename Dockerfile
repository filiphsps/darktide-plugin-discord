# Dockerfile for cross-compiling DarktideDiscord to Windows x64 from Linux/macOS
# Usage:
#   docker build -t darktide-discord-builder .
#   docker create --name extract darktide-discord-builder
#   docker cp extract:/out/DarktideDiscord.dll ./
#   docker rm extract

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble

# Install cross-compilation toolchain for Windows x64
RUN apt-get update && apt-get install -y --no-install-recommends \
    clang \
    zlib1g-dev \
    libkrb5-dev \
    mingw-w64 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src

# Copy project file first for better layer caching
COPY DarktideDiscord.csproj ./

# Restore dependencies
RUN dotnet restore -r win-x64

# Copy source files
COPY src/ ./src/

# Build the native DLL
RUN dotnet publish -c Release -r win-x64 --no-restore -o /out

# The output DLL will be at /out/DarktideDiscord.dll
