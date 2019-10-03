@echo off

xcopy /F /Y SleepTask.dll "%ProgramFiles%\Microsoft SQL Server\140\DTS\Tasks"
xcopy /F /Y SleepTask.dll "%ProgramFiles(x86)%\Microsoft SQL Server\140\DTS\Tasks"

gacutil\gacutil /i SleepTask.dll /f

echo Successfully installed Sleep Task.

REM If you want to persist access credentials for file shares, use the command below:
REM cmdkey /add:fileshareserver /user:xxx /pass:yyy
REM You can then access \\fileshareserver\folder directly in your SSIS packages.