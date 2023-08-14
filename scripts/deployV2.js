#!/usr/bin/env node
const Client = require("ssh2-sftp-client");
const fs = require("fs");
const fsp = require("fs/promises");
require("dotenv").config();

const latestDir =
  "/home/torn-lasersports/htdocs/torn.lasersports.au/downloads/latestV2";
const versionsDir =
  "/home/torn-lasersports/htdocs/torn.lasersports.au/downloads/versions/files";

class SFTPClient {
  constructor() {
    this.client = new Client();
  }

  async connect(options) {
    console.log(`Connecting to ${options.host}:${options.port}`);
    try {
      await this.client.connect(options);
    } catch (err) {
      console.log("Failed to connect:", err);
    }
  }

  async disconnect() {
    await this.client.end();
  }

  async listFiles(remoteDir, fileGlob) {
    console.log(`Listing ${remoteDir} ...`);
    let fileObjects;
    try {
      fileObjects = await this.client.list(remoteDir, fileGlob);
    } catch (err) {
      console.log("Listing failed:", err);
    }

    const fileNames = [];

    for (const file of fileObjects) {
      if (file.type === "d") {
        console.log(
          `${new Date(file.modifyTime).toISOString()} PRE ${file.name}`
        );
      } else {
        console.log(
          `${new Date(file.modifyTime).toISOString()} ${file.size} ${file.name}`
        );
      }

      fileNames.push(file.name);
    }

    return fileNames;
  }
  async downloadFile(remoteFile, localFile) {
    console.log(`Downloading ${remoteFile} to ${localFile} ...`);
    try {
      await this.client.get(remoteFile, localFile);
    } catch (err) {
      console.error("Downloading failed:", err);
    }
  }
  async uploadFile(localFile, remoteFile) {
    console.log(`Uploading ${localFile} to ${remoteFile} ...`);
    try {
      await this.client.put(localFile, remoteFile);
    } catch (err) {
      console.error("Uploading failed:", err);
    }
  }
}

const checkIfSymanticVersionIsGreater = (version1, version2) => {
  const version1Array = version1.split(".");
  const version2Array = version2.split(".");
  for (let i = 0; i < version1Array.length; i++) {
    if (version1Array[i] > version2Array[i]) {
      return true;
    } else if (version1Array[i] < version2Array[i]) {
      return false;
    }
  }
  return false;
};

const run = async () => {
  const client = new SFTPClient();
  await client.connect({
    host: process.env.SFTP_HOST,
    port: "22",
    username: process.env.SFTP_USERNAME,
    password: process.env.SFTP_PASSWORD,
  });
  await client.downloadFile(`${latestDir}/version.txt`, "serverVersion.txt");

  const version = await fsp.readFile("serverVersion.txt", {
    encoding: "utf-8",
  });
  console.log("latest version on server: ", version);
  const localVersion = await fsp.readFile("version.txt", {
    encoding: "utf-8",
  });
  console.log("local version: ", localVersion);

  const isLocalVersionGreater = checkIfSymanticVersionIsGreater(
    localVersion,
    version
  );

  if (isLocalVersionGreater) {
    await client.downloadFile(
      `${latestDir}/Torn Installer.msi`,
      "oldInstaller.msi"
    );

    await client.uploadFile(
      "oldInstaller.msi",
      `${versionsDir}/Torn Installer_${version}.msi`
    );

    await client.uploadFile(
      "Torn Installer/bin/x64/Release/en-US/Torn Installer.msi",
      `${latestDir}/Torn Installer.msi`
    );
    await client.uploadFile("version.txt", `${latestDir}/version.txt`);

    fs.unlinkSync("oldInstaller.msi");
  } else {
    console.log("Server version is greater than local version");
    console.log("Bump Local version and try again");
  }

  fs.unlinkSync("serverVersion.txt");
  await client.disconnect();
};

run();
