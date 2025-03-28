#!/usr/bin/env bash

UNITY_PATH="/Applications/Unity/Hub/Editor/2021.3.34f1/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="/Users/dmytroudovychenko/Dev/My/advanced-project-builder"
BUILD_METHOD="AdvancedProjectBuilderCommandLineRunner.Build"
BUILD_PATH_NAME="/Users/dmytroudovychenko/Dev/Builds"
BUILD_FILE_NAME="Commandline-MacOS"

"$UNITY_PATH" \
  -batchmode \
  -quit \
  -projectPath "$PROJECT_PATH" \
  -executeMethod "$BUILD_METHOD" \
  -logFile "$BUILD_PATH_NAME/build.log" \
  \
  -BUILD_CONFIGURATION_NAME="" \
  -BUILD_PATH="$BUILD_PATH_NAME/$BUILD_FILE_NAME" \
  -BUILD_PLATFORM="StandaloneOSX" \
  -BUILD_OPTION="Development;CompressWithLz4HC" \
  \
  -BUNDLE_ID="com.commandline.StandaloneOSX" \
  -BUILD_VERSION="333" \
  -BUILD_VERSION_NUMBER="444" \
  \
  -APPLE_DEVELOPER_TEAM_ID="TeamId"
