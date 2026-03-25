# Concilig.Desafio

Sistema web desenvolvido em **ASP.NET Core (Razor Pages)** com autenticação via ASP.NET Identity e banco de dados **SQL Server**. A aplicação permite que usuários autenticados importem arquivos CSV contendo contratos de clientes, visualizem o histórico de importações, consultem contratos por cliente e analisem o valor total e o atraso em dias de cada cliente.

---

## Funcionalidades

- Autenticação e cadastro de usuários (ASP.NET Identity)
- Upload e importação de arquivos CSV (separados por `;`, até 5 MB)
- Validação linha a linha do CSV — linhas inválidas são ignoradas e registradas como erro
- Histórico de importações (nome do arquivo, data, usuário, quantidade de registros)
- Listagem de contratos importados
- Consulta por cliente com:
  - Soma total dos valores dos contratos
  - Cálculo do maior atraso em dias
- Middleware global de tratamento de exceções

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) (local ou remoto)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) ou VS Code com extensão C#

---

## Configuracao do banco de dados

### 1. Alterar a Connection String

Abra o arquivo `appsettings.json` e substitua o valor de `DefaultConnection` pela string de conexao do seu servidor SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=ConciligDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Exemplos comuns:

| Cenario | Connection String |
|---|---|
| SQL Server local (Windows Auth) | `Server=localhost;Database=ConciligDB;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server com usuario e senha | `Server=localhost;Database=ConciligDB;User Id=sa;Password=SuaSenha;TrustServerCertificate=True;` |
| Instancia nomeada | `Server=MAQUINA\SQLEXPRESS;Database=ConciligDB;Trusted_Connection=True;TrustServerCertificate=True;` |

> O banco de dados `ConciligDB` sera criado automaticamente ao aplicar as migrations.

---

### 2. Aplicar as Migrations

As migrations ja estao criadas no projeto. Basta aplicar ao banco.

**Opcao A — Via Package Manager Console (Visual Studio):**

Abra o menu **Tools > NuGet Package Manager > Package Manager Console** e execute:

```powershell
Update-Database
```

**Opcao B — Via terminal (.NET CLI):**

Na pasta do projeto (`Concilig.Desafio/Concilig.Desafio`), execute:

```bash
dotnet ef database update
```

> Isso criara todas as tabelas necessarias: Identity (usuarios, roles, etc.), `Contratos` e `Importacoes`.

---

## Como rodar o projeto

### Via Visual Studio

1. Abra o arquivo `Concilig.Desafio.slnx`
2. Defina `Concilig.Desafio` como projeto de inicializacao (botao direito > "Set as Startup Project")
3. Pressione **F5** ou clique em **Run**

### Via terminal

Na pasta `Concilig.Desafio/Concilig.Desafio`:

```bash
dotnet run
```

A aplicacao estara disponivel em `https://localhost:<porta>` (a porta e exibida no terminal).

---

## Como usar

1. Acesse a aplicacao no navegador
2. Clique em **Register** para criar uma conta
3. Confirme o e-mail (em ambiente de desenvolvimento a confirmacao e simulada)
4. Faca login
5. Navegue ate **Importacoes > Upload**
6. Envie um arquivo `.csv` no formato abaixo

---

## Formato do arquivo CSV

O arquivo deve ser separado por `;` e conter um cabecalho na primeira linha (que sera ignorado). As colunas esperadas sao:

```
Nome;CPF;Contrato;Produto;Vencimento;Valor
```

Exemplo:

```csv
Nome;CPF;Contrato;Produto;Vencimento;Valor
João Silva;123.456.789-00;CTR-001;Emprestimo Pessoal;15/03/2025;1500,00
Maria Souza;987.654.321-00;CTR-002;Financiamento;01/01/2025;3200,50
```

Regras de validacao:
- O nome do cliente e o numero do contrato sao obrigatorios
- A data de vencimento deve estar no formato `dd/MM/yyyy`
- O valor deve usar virgula como separador decimal (padrao pt-BR)
- Linhas invalidas sao ignoradas — a importacao continua com as demais
- Tamanho maximo: **5 MB**

---

## Estrutura do projeto

```
Concilig.Desafio/
  Concilig.Desafio/
    Data/
      ApplicationDbContext.cs       # DbContext com Identity + Contratos + Importacoes
      Migrations/                   # Migrations do Entity Framework
    DTOs/
      ContratoImportadoDto.cs       # Dados intermediarios da leitura do CSV
    Middleware/
      GlobalExceptionMiddleware.cs  # Tratamento global de erros HTTP
    Models/
      Contrato.cs                   # Entidade Contrato
      Importacao.cs                 # Entidade Importacao
    Pages/
      Importacoes/
        Upload.cshtml               # Pagina de upload do CSV
        Index.cshtml                # Historico de importacoes
        Detalhes.cshtml             # Detalhes de uma importacao
        Consulta.cshtml             # Consulta por cliente
        Clientes.cshtml             # Listagem de contratos
    Services/
      IImportacaoService.cs         # Interface do servico
      ImportacaoService.cs          # Logica de leitura e persistencia do CSV
      ImportacaoResultado.cs        # Objeto de retorno da importacao
    appsettings.json                # Configuracoes (incluindo Connection String)
    Program.cs                      # Ponto de entrada e configuracao de DI
```

---

## Tecnologias utilizadas

- ASP.NET Core 8 — Razor Pages
- Entity Framework Core 8 — ORM
- ASP.NET Identity — autenticacao e autorizacao
- SQL Server — banco de dados relacional
- Bootstrap 5 — estilizacao das paginas