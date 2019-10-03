@echo off

PowerShell Expand-Archive -Path ".\ODP.NET_Managed_ODAC122cR1.zip" -DestinationPath ".\ODP.NET"

start /D .\ODP.NET /wait cmd /c "call install_odpm.bat %SystemDrive%\ODP.NET both true > %CUSTOM_SETUP_SCRIPT_LOG_DIR%\install2.log"
echo Execute install_odpm.bat completed

REM install_odpm.bat will redirect some of the standard output to %SystemDrive%\ODP.NET\install.log,
REM we need to copy it to %CUSTOM_SETUP_SCRIPT_LOG_DIR% so that it can be uploaded to your blob container

start /wait xcopy /R /F /Y %SystemDrive%\ODP.NET\install.log %CUSTOM_SETUP_SCRIPT_LOG_DIR%
echo Copied %SystemDrive%\ODP.NET\install.log to %CUSTOM_SETUP_SCRIPT_LOG_DIR%
