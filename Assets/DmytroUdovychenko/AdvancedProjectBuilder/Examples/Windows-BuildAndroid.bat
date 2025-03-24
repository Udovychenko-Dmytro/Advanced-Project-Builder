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
