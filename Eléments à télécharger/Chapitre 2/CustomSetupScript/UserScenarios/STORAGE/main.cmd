@echo off

echo Start to install AzurePowershell.msi

msiexec /i AzurePowershell.msi /quiet /lv %CUSTOM_SETUP_SCRIPT_LOG_DIR%\AzurePowershell.log

echo Finished installation of AzurePowershell.msi

