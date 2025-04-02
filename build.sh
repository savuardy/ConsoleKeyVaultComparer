#!/bin/bash

# Create release directory
RELEASE_DIR="releases/ConsoleKeyVaultComparer"
mkdir -p "$RELEASE_DIR"

# Build the project
echo "Building project..."
dotnet publish -c Release

# Copy executable and script
echo "Copying files..."
cp "ConsoleKeyVaultComparer/bin/Release/net9.0/osx-x64/publish/ConsoleKeyVaultComparer" "$RELEASE_DIR/"
cp "run.sh" "$RELEASE_DIR/"
cp "README.md" "$RELEASE_DIR/"
cp "STEP_BY_STEP.md" "$RELEASE_DIR/"

# Make script executable
chmod +x "$RELEASE_DIR/run.sh"

# Create ZIP file
echo "Creating ZIP file..."
cd releases
zip -r "ConsoleKeyVaultComparer.zip" "ConsoleKeyVaultComparer"

echo "Release created successfully!"
echo "You can find the release in: releases/ConsoleKeyVaultComparer.zip" 