@echo off
setlocal
where pythonw.exe >nul 2>nul
if not errorlevel 1 (
  start "" pythonw.exe "%~dp0tools\preview.py"
  exit /b
)
python "%~dp0tools\preview.py"
if errorlevel 1 pause
