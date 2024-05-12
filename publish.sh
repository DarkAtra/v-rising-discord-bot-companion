#!/bin/bash
if [ -z "$1" ]; then
    echo "Release version is not set"
    exit
fi

dotnet build --no-restore --configuration Release
#tcli publish --package-version $1 --token $THUNDERSTORE_TOKEN
