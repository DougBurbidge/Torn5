#!/usr/bin/env bash
source .env
version=$(<version.txt)
deployedVersion=$(curl ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/version.txt --ssl)
mkdir temp

arrVersion=(${version//./ })

patch=${arrVersion[2]}
minor=${arrVersion[1]}
major=${arrVersion[0]}

arrDeployedVersion=(${deployedVersion//./ })

deployedPatch=${arrDeployedVersion[2]}
deployedMinor=${arrDeployedVersion[1]}
deployedMajor=${arrDeployedVersion[0]}

deployable=false

if [ "$major" -gt "$deployedMajor" ]
  then
    deployable=true
fi

if [ "$major" -eq "$deployedMajor" ] && [ "$minor" -gt "$deployedMinor" ]
  then
    deployable=true
fi

if [ "$major" -eq "$deployedMajor" ] && [ "$minor" -eq "$deployedMinor" ] && [ "$patch" -gt "$deployedPatch" ]
  then
    deployable=true
fi

echo $deployable

if [ $deployable == false ]
  then
    echo "No changes to deploy"
    echo "Deployed version: $deployedVersion"
    echo "Local version: $version"
    echo "Bump version and try again"
    exit 1
fi

mkdir workrepo
cd workrepo
git init
cp -r "../bin/Debug/." .
git add .
git commit -m commit
mkdir -p ../Releases/$version/
git archive -o "../Releases/$version/Torn5.zip" @
cd ..
rm -rf workrepo

# download old version
curl ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/Torn5.zip --ssl -o ./temp/Torn5.zip

# upload old version to archive folder
curl -T temp/Torn5.zip ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Archive/Torn5_$deployedVersion.zip --ssl

# upload new version to current folder
curl -T Releases/$version/Torn5.zip ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/Torn5.zip --ssl

# update version file
curl -T version.txt ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/version.txt --ssl

rm -rf temp

echo deployed Torn5 v$version


