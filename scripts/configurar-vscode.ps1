[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)

function Invoke-Checked {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Command,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    & $Command @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "O comando '$Command $($Arguments -join ' ')' terminou com o código $LASTEXITCODE."
    }
}

Push-Location $ProjectRoot

try {
    Write-Host "" 
    Write-Host "Configurando o Sistema de Consultas UVV..." -ForegroundColor Cyan

    # Remove o bloqueio "veio de outro computador" (Mark of the Web) dos arquivos extraídos.
    Get-ChildItem -Path $ProjectRoot -Recurse -File |
        Unblock-File -ErrorAction SilentlyContinue

    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw "O SDK do .NET não foi encontrado. Instale o SDK do .NET 8 e abra novamente este arquivo."
    }

    $InstalledSdks = & dotnet --list-sdks
    if (-not ($InstalledSdks | Where-Object { $_ -match '^8\.' })) {
        Write-Host "SDKs encontrados:" -ForegroundColor Yellow
        $InstalledSdks | ForEach-Object { Write-Host "  $_" }
        throw "O SDK do .NET 8 não está instalado. O projeto precisa de uma versão 8.x do SDK."
    }

    if (-not (Get-Command sqllocaldb -ErrorAction SilentlyContinue)) {
        throw "O SQL Server LocalDB não foi encontrado. Instale o componente SQL Server Express LocalDB."
    }

    $Instances = & sqllocaldb info
    if ($LASTEXITCODE -ne 0) {
        throw "Não foi possível consultar as instâncias do SQL Server LocalDB."
    }

    if ($Instances -notcontains "MSSQLLocalDB") {
        Write-Host "Criando a instância MSSQLLocalDB..." -ForegroundColor DarkCyan
        Invoke-Checked -Command "sqllocaldb" -Arguments @("create", "MSSQLLocalDB")
    }

    Write-Host "Iniciando o SQL Server LocalDB..." -ForegroundColor DarkCyan
    Invoke-Checked -Command "sqllocaldb" -Arguments @("start", "MSSQLLocalDB")

    Write-Host "Restaurando os pacotes NuGet..." -ForegroundColor DarkCyan
    Invoke-Checked -Command "dotnet" -Arguments @("restore", "SistemaConsultasUVV.csproj")

    Write-Host "Restaurando a ferramenta local do Entity Framework..." -ForegroundColor DarkCyan
    Invoke-Checked -Command "dotnet" -Arguments @("tool", "restore")

    Write-Host "Compilando o projeto..." -ForegroundColor DarkCyan
    Invoke-Checked -Command "dotnet" -Arguments @("build", "SistemaConsultasUVV.csproj", "--no-restore")

    Write-Host "Criando ou atualizando o banco de dados..." -ForegroundColor DarkCyan
    Invoke-Checked -Command "dotnet" -Arguments @("ef", "database", "update")

    Write-Host "" 
    Write-Host "Configuração concluída com sucesso." -ForegroundColor Green
    Write-Host "Abra esta pasta no VS Code e pressione F5, ou use INICIAR_SISTEMA.cmd." -ForegroundColor Green
}
catch {
    Write-Host "" 
    Write-Host "A configuração não foi concluída:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host "" 
    Write-Host "Consulte o arquivo GUIA_VSCODE.md ou envie uma captura deste erro." -ForegroundColor Yellow
    exit 1
}
finally {
    Pop-Location
}
