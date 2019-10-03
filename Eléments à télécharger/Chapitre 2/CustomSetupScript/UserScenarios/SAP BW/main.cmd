@echo off

REM Copy for SAP connector depended file
start /wait xcopy /R /F /Y %cd%\librfc32.dll %windir%\System32\
