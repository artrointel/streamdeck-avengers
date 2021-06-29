Rem yourPluginID
SET PLUGIN_NAME=com.artrointel.avengerskey



SET CURR_DIR=%~dp0
SET SDTOOLS_RELEASE_DIR=%CURR_DIR%\barraider-sdtools\bin\Release\netstandard2.0
SET PLUGIN_DIR=%PLUGIN_NAME%.sdPlugin

Rem packging all resource files
SET AVGKEY_DIR=%CURR_DIR%\ArtrointelPlugin
SET AVGKEY_RELEASE_DIR=%CURR_DIR%\ArtrointelPlugin\bin\Release\%PLUGIN_DIR%
SET AVGKEY_IMG_DIR=%AVGKEY_DIR%\Images
SET AVGKEY_RES_DIR=%AVGKEY_DIR%\Res
SET AVGKEY_PI_DIR=%AVGKEY_DIR%\PropertyInspector

SET MANIFEST_FILE=manifest.json

SET TARGET_DIR=%PLUGIN_DIR%
SET RELEASE_DIR=Release\

rmdir /s /q %RELEASE_DIR%
rmdir /s /q %TARGET_DIR%
mkdir %RELEASE_DIR%
mkdir %TARGET_DIR%

robocopy %SDTOOLS_RELEASE_DIR%\ %TARGET_DIR%\
robocopy %AVGKEY_RELEASE_DIR%\ %TARGET_DIR%\
robocopy %AVGKEY_DIR%\ %TARGET_DIR%\ %MANIFEST_FILE%

robocopy %AVGKEY_PI_DIR%\ %TARGET_DIR%\PropertyInspector\ /E
robocopy %AVGKEY_IMG_DIR%\ %TARGET_DIR%\Images\ /E
robocopy %AVGKEY_RES_DIR%\ %TARGET_DIR%\Res\ /E

Rem disable your anti-virus system if it cannot release the your.streamDeckPlugin file.
DistributionTool.exe -b -i %TARGET_DIR% -o %RELEASE_DIR%

Rem remove belows for debugging
rmdir /s /q %TARGET_DIR%