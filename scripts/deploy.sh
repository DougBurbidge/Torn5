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

# download old version
curl ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/Torn%20Installer.msi --ssl -o ./temp/"Torn Installer.msi"

# upload old version to archive folder
curl -T temp/"Torn Installer.msi" ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Archive/Torn%20Installer_$deployedVersion.msi --ssl

# upload new version to current folder
curl -T "Torn Installer"/bin/x64/Release/en-US/"Torn Installer.msi" ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/Torn%20Installer.msi --ssl

# update version file
curl -T version.txt ftp://$FTP_USERNAME:$FTP_PASSWORD@$FTP_HOST:21/Torn5/Current/version.txt --ssl

rm -rf temp

echo deployed Torn5 v$version


