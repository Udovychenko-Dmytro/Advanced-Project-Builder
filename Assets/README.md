[![license](https://img.shields.io/badge/license-MIT-red.svg)](LICENSE)
[![version](https://img.shields.io/badge/package-download-brightgreen.svg)](AdvancedProjectBuilder.unitypackage)

# Advanced Project Builder

`version 1.0`

## Overview

**Advanced Project Builder** is a Unity tool designed to simplify and automate the build process for your projects directly from the editor (via a dedicated window) and through the command line, making it ideal for continuous integration and delivery (CI/CD) pipelines.

You can create, manage, and save multiple build configurations tailored for different platforms, allowing streamlined local and remote builds.

---

## Features

- **Multiple Configuration Support:** Create, duplicate, and manage build configurations for various platforms.
- **Editor Integration:** Easy access through a dedicated Unity Editor window.
- **Command Line Integration:** Fully compatible with command-line builds, enhancing your CI/CD workflows.
- **Cross-Platform Builds:** Supports Android, iOS, Windows, macOS, and other Unity-supported platforms.
- **Flexible Overrides:** Easily override any configuration setting via command line parameters.

---

## Installation

### 1. **Download the Tool**

Clone the repository or download the latest release:

```bash
git clone https://github.com/Udovychenko-Dmytro/Advanced-Project-Builder.git
```

OR

1. Download the `AdvancedProjectBuilder.unitypackage` file.
2. Import it into your Unity project via `Assets > Import Package > Custom Package`.
3. Follow the setup instructions.

---

## Usage

To open Advanced Project Builder in Unity:

- Navigate to `Tools > DmytroUdovychenko > AdvancedProjectBuilder`

The window displays your build configurations and provides four main actions:

- **Build** – Initiate the build process with the selected configuration.
- **Open** – View and edit the selected configuration.
- **Duplicate** – Copy an existing configuration.
- **Delete** – Remove an existing configuration.

---

## Configuration Settings

**General Build Settings:**

- `BUILD_PATH` – Path for the build.
- `BUILD_PLATFORM` – [BuildTarget](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/BuildTarget.html)
- `BUILD_OPTION` – [BuildOptions](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/BuildOptions.html)
- `BUILD_VERSION` – [Application.version](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-version.html)
- `BUILD_VERSION_NUMBER` – Android ([bundleVersionCode](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/PlayerSettings.Android-bundleVersionCode.html)), iOS ([buildNumber](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/PlayerSettings.iOS-buildNumber.html))
- `BUILD_PRODUCT_NAME` – [Application.productName](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-productName.html)
- `BUNDLE_ID` – [Application.identifier](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-identifier.html)

**Unity Services ID Settings:**

- `UNITY_SERVICE_ID_OVERRIDE` – "true" or "false"
- `UNITY_SERVICE_PROJECT_ID` – Your Unity Project ID
- `UNITY_SERVICE_ORGANIZATION_ID` – Your Unity Organization ID

**Android Build Settings:**

- `ANDROID_APP_BUNDLE` – [App Bundle](https://docs.unity3d.com/6000.0/Documentation/Manual/android-BuildProcess.html)
- `ANDROID_SPLIT_BINARY` – [Split Application Binary](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/PlayerSettings.Android-splitApplicationBinary.html)
- `ANDROID_USE_KEYSTORE` – "true" or "false"
- `ANDROID_KEYSTORE_PATH`
- `ANDROID_KEYSTORE_PASS`
- `ANDROID_KEYALIAS_NAME`
- `ANDROID_KEYALIAS_PASS` – [Keystore settings](https://docs.unity3d.com/6000.0/Documentation/Manual/class-PlayerSettingsAndroid.html#projectkeystore)

**iOS Build Settings:**

- `APPLE_DEVELOPER_TEAM_ID` – [Apple Developer Team ID](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/PlayerSettings.iOS-appleDeveloperTeamID.html)

---

## Command Line Builds

Use the following example format for command line builds for the Windows machine:

```bash
@echo off

set UNITY_PATH=D:\Unity\2021.3.43f1\Editor\Unity.exe
set PROJECT_PATH=D:\Dev\GIT\!My\advanced-project-builder
set BUILD_METHOD=AdvancedProjectBuilderCommandLineRunner.Build
set BUILD_PATH_NAME=D:\Dev\Builds
set PRODUCT_NAME=AdvancedProjectBuilder
set BUILD_FILE_NAME=Commandline-Android.apk

%UNITY_PATH% ^
  -batchmode ^
  -quit ^
  -projectPath "%PROJECT_PATH%" ^
  -executeMethod "%BUILD_METHOD%" ^
  -logFile "%BUILD_PATH_NAME%\\build.log" ^
  -BUILD_CONFIGURATION_NAME="" ^
  -BUILD_PATH="%BUILD_PATH_NAME%\\%BUILD_FILE_NAME%" ^
  -BUILD_PLATFORM="Android" ^
  -BUILD_OPTION="Development;CompressWithLz4HC" ^
  -UNITY_SERVICE_ID_OVERRIDE="true" ^
  -UNITY_SERVICE_PROJECT_ID="b6dd78fa-3c9f-4134-9b80-77b07dc4b0e2" ^
  -UNITY_SERVICE_ORGANIZATION_ID="blabla-studio" ^
  -BUILD_PRODUCT_NAME="%PRODUCT_NAME%" ^
  -BUNDLE_ID="com.commandline.ios" ^
  -BUILD_VERSION="333" ^
  -BUILD_VERSION_NUMBER="444" ^
  -ANDROID_APP_BUNDLE="false" ^
  -ANDROID_SPLIT_BINARY="false" ^
  -ANDROID_USE_KEYSTORE="true" ^
  -ANDROID_KEYSTORE_PATH="Assets/DmytroUdovychenko/AdvancedProjectBuilder/Examples/key.keystore" ^
  -ANDROID_KEYSTORE_PASS="12345678" ^
  -ANDROID_KEYALIAS_NAME="Advanced Project Builder" ^
  -ANDROID_KEYALIAS_PASS="qwerty" ^
```


Use the following example format for command line builds for the MacOs machine:

```bash
#!/usr/bin/env bash

UNITY_PATH="/Applications/Unity/Hub/Editor/2021.3.34f1/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="/Users/dmytroudovychenko/Dev/My/advanced-project-builder"
BUILD_METHOD="AdvancedProjectBuilderCommandLineRunner.Build"
BUILD_PATH_NAME="/Users/dmytroudovychenko/Dev/Builds"
PRODUCT_NAME="AdvancedProjectBuilder"
BUILD_FILE_NAME="test.apk"

"$UNITY_PATH" \
  -batchmode \
  -quit \
  -projectPath "$PROJECT_PATH" \
  -executeMethod "$BUILD_METHOD" \
  -logFile "$BUILD_PATH_NAME/build.log" \
  \
  -BUILD_CONFIGURATION_NAME="Android_Release" \
  -BUILD_PATH="$BUILD_PATH_NAME/$BUILD_FILE_NAME" \
  -BUILD_PLATFORM="Android" \
  -BUILD_OPTION="Development;CompressWithLz4HC" \
  \
  -BUILD_PRODUCT_NAME="$PRODUCT_NAME" \
  -BUNDLE_ID="com.commandline.Android" \
  -BUILD_VERSION="111" \
  -BUILD_VERSION_NUMBER="222" \
  \
  -ANDROID_APP_BUNDLE="false" \
  -ANDROID_SPLIT_BINARY="false" \
  -ANDROID_USE_KEYSTORE="true" \
  -ANDROID_KEYSTORE_PATH="Assets/DmytroUdovychenko/AdvancedProjectBuilder/Examples/key.keystore" \
  -ANDROID_KEYSTORE_PASS="12345678" \
  -ANDROID_KEYALIAS_NAME="Advanced Project Builder" \
  -ANDROID_KEYALIAS_PASS="qwerty"
```




Parameters can override configuration file settings or fully define builds directly from the command line.

---

## License

This project is licensed under the MIT License – see the [LICENSE](LICENSE) file for details.

## Contact

For any questions, suggestions, or feedback, please contact:

- **LinkedIn:** [https://www.linkedin.com/in/dmytro-udovychenko/](https://www.linkedin.com/in/dmytro-udovychenko/)


