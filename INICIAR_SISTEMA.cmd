@echo off
title Sistema de Consultas UVV
cd /d "%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
    echo O SDK do .NET nao foi encontrado.
    echo Execute primeiro o arquivo CONFIGURAR_VSCODE.cmd.
    pause
    exit /b 1
)

echo Iniciando em http://localhost:5098 ...
start "" powershell -NoProfile -Command "Start-Sleep -Seconds 4; Start-Process 'http://localhost:5098'"
dotnet run --launch-profile http

echo.
echo O sistema foi encerrado ou ocorreu um erro.
pause
