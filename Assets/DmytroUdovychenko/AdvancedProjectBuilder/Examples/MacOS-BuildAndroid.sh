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