@echo off

echo "start main.cmd"
time /T

"%SystemRoot%\System32\msiexec.exe" /i AttunitySSISOraAdaptersSetup.msi /qn /norestart /log oracle32bit.log EulaForm_Property=Yes RestartSqlServerSilent=true 

"%SystemRoot%\System32\msiexec.exe" /i AttunitySSISOraAdaptersSetupX64.msi /qn /norestart /log oracle64bit.log EulaForm_Property=Yes RestartSqlServerSilent=true

start /wait xcopy /R /F /Y ".\*.log" "%CUSTOM_SETUP_SCRIPT_LOG_DIR%\" 

PowerShell Expand-Archive -Path ".\winx64_12201_client.zip" -DestinationPath ".\OracleClient"

REM Install Oracle Net client
.\OracleClient\client\setup.exe -silent -noconsole -ignorePrereq -showProgress -waitForCompletion -responseFile %cd%\client.rsp -J"-Doracle.install.client.validate.clientSupportedOSCheck=false"

echo "Complete setup"

REM Set TNS_ADMIN variable for SSIS to read tbsnames.ora file
REM the path is same as the path defiend in response file
setx TNS_ADMIN "%SystemDrive%\OracleClient\product\12.2.0\client_1\network\admin" /M
echo "set TNS_ADMIN"

REM Copy tnsnames.ora which contains the connection information to be used by SSIS package Oracle Connector
REM TNS Service name can also in format of host:port/service_name, which does not use tnsnames.ora 
start /wait xcopy /R /F /Y %cd%\tnsnames.ora %SystemDrive%\OracleClient\product\12.2.0\client_1\network\admin\

echo "copied tnsnames.ora"

REM setup will redirect some of the standard output to log files in folder %SystemDrive%\Program Files\Oracle\Inventory\logs\,
REM we need to copy it to %CUSTOM_SETUP_SCRIPT_LOG_DIR% so that it can be uploaded to your blob container

echo "Source log dir is %SystemDrive%\Program Files\Oracle\Inventory\logs\*.*"
dir "%SystemDrive%\Program Files\Oracle\Inventory\logs\*.*"

start /wait xcopy /R /F /Y "%SystemDrive%\Program Files\Oracle\Inventory\logs\*.*" "%CUSTOM_SETUP_SCRIPT_LOG_DIR%\" 

echo "copied logs"

echo "Target Log dir is %CUSTOM_SETUP_SCRIPT_LOG_DIR%"
dir "%CUSTOM_SETUP_SCRIPT_LOG_DIR%"

time /T
echo "Complete main.cmd"