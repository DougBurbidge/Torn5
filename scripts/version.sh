#!/usr/bin/env bash
if [ -z "$1" ] || [ "$1" != "major" ] && [ "$1" != "minor" ] && [ "$1" != "patch" ]
  then
    echo "Format should match: ./scripts/version.sh <major | minor | patch>"
    exit 1
fi


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


echo $currentVersion \> $newVersion

