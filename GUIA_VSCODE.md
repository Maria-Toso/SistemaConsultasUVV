# Guia rápido — Visual Studio Code

## 1. Instalar os componentes

Confirme que o computador possui:

- Visual Studio Code;
- SDK do .NET 8;
- extensão **C# Dev Kit**, da Microsoft;
- SQL Server Express LocalDB.

No terminal:

```powershell
dotnet --list-sdks
```

Deve aparecer pelo menos uma linha iniciada por `8.`.

## 2. Evitar o erro “veio de outro computador”

Antes de extrair o arquivo:

1. clique com o botão direito no ZIP;
2. escolha **Propriedades**;
3. se aparecer a opção **Desbloquear**, marque-a;
4. clique em **Aplicar** e extraia o ZIP novamente.

Se o projeto já foi extraído, abra o PowerShell na pasta do projeto e execute:

```powershell
Get-ChildItem -Recurse -File | Unblock-File
```

O configurador automático também tenta remover esse bloqueio.

## 3. Configuração automática

1. Extraia todo o ZIP; não execute dentro do arquivo compactado.
2. Dê dois cliques em `CONFIGURAR_VSCODE.cmd`.
3. Aguarde o configurador verificar os programas, restaurar os pacotes, compilar e criar o banco.
4. Se aparecer uma mensagem vermelha, tire uma captura completa antes de fechar a janela.

## 4. Abrir corretamente no VS Code

Selecione **Arquivo > Abrir Pasta > SistemaConsultasUVV**.

Abra a pasta que contém `SistemaConsultasUVV.csproj`. Não abra somente um arquivo `.cs`.

Quando o VS Code recomendar extensões, escolha **Instalar**.

## 5. Executar

Use uma destas opções:

- pressione `F5` e escolha **.NET: Executar Consultas UVV**;
- dê dois cliques em `INICIAR_SISTEMA.cmd`;
- execute `dotnet run --launch-profile http` no terminal integrado.

Abra `http://localhost:5098`. O Swagger fica em `http://localhost:5098/swagger`.

## 6. Comandos manuais

Dentro da pasta do projeto:

```powershell
dotnet restore
dotnet tool restore
dotnet build
dotnet ef database update
dotnet run --launch-profile http
```

## 7. Tarefas prontas do VS Code

Pressione `Ctrl + Shift + P`, procure **Tasks: Run Task** e escolha:

- `1. Restaurar pacotes`;
- `2. Restaurar ferramenta do EF Core`;
- `3. Criar ou atualizar banco`;
- `Compilar projeto`;
- `Executar projeto`.

## Erros comuns

### “dotnet não é reconhecido”

O SDK do .NET não está instalado ou o terminal ainda não reconheceu a instalação. Instale o SDK do .NET 8 e reinicie o Windows ou, pelo menos, o VS Code.

### “A compatible .NET SDK was not found”

Execute `dotnet --list-sdks`. O projeto precisa de um SDK `8.x`.

### “sqllocaldb não é reconhecido”

Instale o **SQL Server Express LocalDB**. Pelo Visual Studio Installer, ele aparece em **Componentes individuais**. O VS Code pode ser usado normalmente depois da instalação desse componente.

### Erro ao conectar ou cadastrar usuário

Na pasta do projeto, execute:

```powershell
sqllocaldb start MSSQLLocalDB
dotnet tool restore
dotnet ef database update
```

Depois pare o sistema com `Ctrl+C` e execute novamente.

### O banco ficou incompatível após alterações

Não apague nada primeiro. Execute `dotnet ef database update` e copie a mensagem completa caso continue com erro.

### A porta 5098 já está sendo usada

Feche a janela antiga do sistema ou, no terminal que a está executando, pressione `Ctrl+C`. Em seguida, inicie novamente.

### A página não abre automaticamente

Abra manualmente `http://localhost:5098`. Se o terminal exibir outra URL depois de `Now listening on:`, use a URL mostrada por ele.
