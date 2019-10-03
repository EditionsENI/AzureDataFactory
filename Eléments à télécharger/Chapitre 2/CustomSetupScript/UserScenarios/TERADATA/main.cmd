@echo off

echo "start main.cmd at %TIME%"

start /wait cmd /c "call install.cmd > %CUSTOM_SETUP_SCRIPT_LOG_DIR%\install.log"

echo "Complete main.cmd at %TIME%"