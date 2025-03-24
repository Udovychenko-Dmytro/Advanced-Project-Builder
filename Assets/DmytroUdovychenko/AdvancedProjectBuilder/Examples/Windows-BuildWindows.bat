@echo off

set UNITY_PATH=D:\Unity\2021.3.43f1\Editor\Unity.exe
set PROJECT_PATH=D:\Dev\GIT\!My\advanced-project-builder
set BUILD_METHOD=AdvancedProjectBuilderCommandLineRunner.Build
set BUILD_PATH_NAME=D:\Dev\Builds\StandaloneWindows
set PRODUCT_NAME=AdvancedProjectBuilder
set BUILD_FILE_NAME=Commandline-Windows.exe

%UNITY_PATH% ^
  -batchmode ^
  -quit ^
  -projectPath "%PROJECT_PATH%" ^
  -executeMethod "%BUILD_METHOD%" ^
  -logFile "%BUILD_PATH_NAME%\\build.log" ^
  -BUILD_CONFIGURATION_NAME="" ^
  -BUILD_PATH="%BUILD_PATH_NAME%\\%BUILD_FILE_NAME%" ^
  -BUILD_PLATFORM="StandaloneWindows" ^
  -BUILD_OPTION="Development;CompressWithLz4HC" ^
  -BUILD_PRODUCT_NAME="%PRODUCT_NAME%" ^
  -BUNDLE_ID="com.commandline.ios" ^
  -BUILD_VERSION="333" ^
  -BUILD_VERSION_NUMBER="444" ^
