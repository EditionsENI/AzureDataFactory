@echo off

echo Start to install MsSqlCmdLnUtils.msi
msiexec /i MsSqlCmdLnUtils.msi /quiet /lv %CUSTOM_SETUP_SCRIPT_LOG_DIR%\MsSqlCmdLnUtils.log IACCEPTMSSQLCMDLNUTILSLICENSETERMS=YES
echo Finished installation of MsSqlCmdLnUtils.msi

