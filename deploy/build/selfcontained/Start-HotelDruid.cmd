@echo off
setlocal
cd /d "%~dp0"
if not exist "%~dp0HotelDruid.Api.exe" (
  echo HotelDruid.Api.exe was not found in this folder.
  exit /b 1
)
if not exist "%~dp0data" mkdir "%~dp0data"
if not exist "%~dp0logs" mkdir "%~dp0logs"
set "HOTELDRUID_DATAROOT=%~dp0data"
set "HOTELDRUID_LOGDIR=%~dp0logs"
set "ASPNETCORE_ENVIRONMENT=Production"
set "ASPNETCORE_URLS=http://127.0.0.1:8080"
echo HotelDruid is starting at http://127.0.0.1:8080/
echo The hotel files are in the data folder next to this command.
echo Close this window to stop the desk.
"%~dp0HotelDruid.Api.exe"
exit /b %ERRORLEVEL%
