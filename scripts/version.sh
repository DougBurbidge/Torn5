#!/usr/bin/env bash
if [ $0 != "./scripts/version.sh" ]
  then
    echo "Please run this script from the root of the project (above the scripts directory)"
    exit 1
elif [ -z "$1" ] || [ "$1" != "major" ] && [ "$1" != "minor" ] && [ "$1" != "patch" ]
  then
    echo "Format should match: ./scripts/version.sh <major | minor | patch>"
    exit 1
fi

# Installer Paths
installPackPath="./Torn Installer/Torn Installer.wixproj"

currentVersion=$(<version.txt)  

arrVersion=(${currentVersion//./ })

patch=${arrVersion[2]}
minor=${arrVersion[1]}
major=${arrVersion[0]}


if [ "$1" == "major" ]
  then
    major=$((major+1))
    minor="0"
    patch="0"
fi

if [ "$1" == "minor" ]
  then
    minor=$((minor+1))
    patch="0"
fi

if [ "$1" == "patch" ]
  then
    patch=$((patch+1))
fi

newVersion="$major.$minor.$patch"

echo $newVersion > version.txt

# Update the version in the Torn Installer
installerVersion="$(grep -Eom 1 'Torn.Version=[0-9\.]+' "$installPackPath")"
if [ -z "$installerVersion" ]
  then
    echo "WARNING: installer package ($installPackPath) version not updated!"
  else
    sed -i "s/$installerVersion/Torn.Version=$newVersion/" "$installPackPath"
fi

echo $currentVersion \> $newVersion

