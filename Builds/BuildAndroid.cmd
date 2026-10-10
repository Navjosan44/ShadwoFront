@echo off
set UNITY_EXE=%1
if "%UNITY_EXE%"=="" goto noexe
"%UNITY_EXE%" -batchmode -quit -projectPath "%~dp0.." -executeMethod BuildScript.BuildAndroidAPK -logfile "%~dp0build.log" 2>&1
exit /b %ERRORLEVEL%
:noexe
echo Unity executable not provided
exit /b 1
