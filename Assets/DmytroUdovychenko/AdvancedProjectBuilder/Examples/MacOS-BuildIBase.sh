#!/usr/bin/env bash

UNITY_PATH="/Applications/Unity/Hub/Editor/2021.3.34f1/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="/Users/dmytroudovychenko/Dev/My/advanced-project-builder"
BUILD_METHOD="AdvancedProjectBuilderCommandLineRunner.Build"
BUILD_PATH_NAME="/Users/dmytroudovychenko/Dev/Builds"
PRODUCT_NAME="AdvancedProjectBuilder"
BUILD_FILE_NAME="Commandline-ios"

"$UNITY_PATH" \
  -batchmode \
  -quit \
  -projectPath "$PROJECT_PATH" \
  -executeMethod "$BUILD_METHOD" \
  //-buildTarget Android \
  -logFile "$BUILD_PATH_NAME/build.log" \
  \
  -BUILD_CONFIGURATION_NAME="" \
  -BUILD_PATH="$BUILD_PATH_NAME/$BUILD_FILE_NAME" \
  -BUILD_PLATFORM="iOS" \
  -BUILD_OPTION="Development;CompressWithLz4HC" \
  -UNITY_SERVICE_ID_OVERRIDE="true" \
  -UNITY_SERVICE_PROJECT_ID="b6dd78fa-3c9f-4134-9b80-77b07dc4b0e2" \
  -UNITY_SERVICE_ORGANIZATION_ID="blabla-studio" \
  \
  -BUILD_PRODUCT_NAME="$PRODUCT_NAME" \
  -BUNDLE_ID="com.commandline.ios" \
  -BUILD_VERSION="333" \
  -BUILD_VERSION_NUMBER="444" \
  \
  -ANDROID_APP_BUNDLE="false" \
  -ANDROID_SPLIT_BINARY="false" \
  -ANDROID_USE_KEYSTORE="true" \
  -ANDROID_KEYSTORE_PATH="Assets/DmytroUdovychenko/AdvancedProjectBuilder/Examples/key.keystore" \
  -ANDROID_KEYSTORE_PASS="12345678" \
  -ANDROID_KEYALIAS_NAME="Advanced Project Builder" \
  -ANDROID_KEYALIAS_PASS="qwerty" \
  \
  -APPLE_DEVELOPER_TEAM_ID="TeamId"
