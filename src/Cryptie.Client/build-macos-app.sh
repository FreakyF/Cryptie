#!/bin/bash
set -e

PROJECT_PATH="$(dirname "$0")"
PUBLISH_DIR="$PROJECT_PATH/bin/Release/net9.0/osx-x64/publish"
APP_NAME="Cryptie"
APP_BUNDLE="$PROJECT_PATH/${APP_NAME}.app"

dotnet publish -c Release -r osx-x64

rm -rf "$APP_BUNDLE"

mkdir -p "$APP_BUNDLE/Contents/MacOS"
mkdir -p "$APP_BUNDLE/Contents/Resources"

cp "$PUBLISH_DIR/$APP_NAME" "$APP_BUNDLE/Contents/MacOS/"

cp "$PUBLISH_DIR"/*.dylib "$APP_BUNDLE/Contents/MacOS/" 2>/dev/null || true

cp "$PROJECT_PATH/Info.plist" "$APP_BUNDLE/Contents/Info.plist"

cp "$PROJECT_PATH/Assets/AppIcon.icns" "$APP_BUNDLE/Contents/Resources/" 2>/dev/null || true

cp "$PROJECT_PATH/appsettings.json" "$APP_BUNDLE/Contents/MacOS/"

chmod +x "$APP_BUNDLE/Contents/MacOS/$APP_NAME"

echo "Gotowa paczka: $APP_BUNDLE"
open "$APP_BUNDLE"
