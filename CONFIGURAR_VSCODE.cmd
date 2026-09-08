@echo off
title Configurar Sistema de Consultas UVV
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File ".\scripts\configurar-vscode.ps1"
echo.
pause
