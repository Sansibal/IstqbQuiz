#!/usr/bin/env bash
set -e

echo "🚀 Building Blazor WebAssembly App for Render..."

# Gehe ins Client-Projekt
cd Client

# Publish mit Release Config
dotnet publish -c Release

echo "✅ Build completed. Output in Client/bin/Release/net8.0/publish/wwwroot"
