# API de apólice de seguro automóvel

API REST para gerenciamento de apólices de seguro automóvel, desenvolvida como parte do hands-on **Desenvolvedor(a) Jr — Segfy**.

A aplicação permite cadastrar, consultar, atualizar e remover apólices, com persistência em SQLite e consulta SQL de apólices que vencem nos próximos 30 dias.

## Tecnologias

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- xUnit + Moq (testes unitários)
- Swagger (documentação da API)

## Arquitetura

O projeto está organizado em camadas:

```
src/
├── InsurancePolicy.Api            → Controllers e configuração da API
├── InsurancePolicy.Application    → Regras de negócio, DTOs e validações
├── InsurancePolicy.Domain         → Entidades e enums
└── InsurancePolicy.Infrastructure → Repositório, EF Core e migrations

tests/
└── InsurancePolicy.Tests          → Testes unitários
```

A validação de CPF/CNPJ foi separada em `DocumentoSeguradoValidator` para facilitar manutenção e testes.

## Como executar

### 1. Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)

Não é necessário instalar banco de dados — o SQLite é criado automaticamente na primeira execução.

Verifique se o SDK está instalado:

```bash
dotnet --version
```

Deve retornar `9.x.x`.

### 2. Clonar o repositório

```bash
git clone https://github.com/MayaraPereira17/insurance-policy-api.git
cd insurance-policy-api
```

### 3. Restaurar dependências e compilar

```bash
dotnet restore
dotnet build
```

### 4. Subir a API

```bash
dotnet run --project src/InsurancePolicy.Api
```

Na primeira execução, o arquivo `src/InsurancePolicy.Api/insurance.db` é criado e as migrations são aplicadas automaticamente.

**Com hot reload (desenvolvimento):**

```bash
dotnet watch --project src/InsurancePolicy.Api
```

### 5. Acessar a documentação

- Swagger (HTTP): http://localhost:5177/swagger
- HTTPS (opcional): https://localhost:7031/swagger

> O Swagger só fica disponível em ambiente de desenvolvimento.

## Como executar os testes

```bash
dotnet test
```

Ou apenas o projeto de testes:

```bash
dotnet test tests/InsurancePolicy.Tests
```

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/apolices` | Cria uma apólice |
| GET | `/api/apolices` | Lista todas as apólices |
| GET | `/api/apolices/{id}` | Busca apólice por ID |
| GET | `/api/apolices/vencendo` | Lista apólices ativas que vencem em até 30 dias |
| PUT | `/api/apolices/{id}` | Atualiza uma apólice |
| DELETE | `/api/apolices/{id}` | Remove uma apólice |

## Exemplo — criar apólice

**POST** `/api/apolices`

```json
{
  "cpfCnpjSegurado": "52998224725",
  "placaVeiculo": "BRA2E19",
  "valorPremio": 289.90,
  "dataInicioVigencia": "2026-01-01",
  "dataFimVigencia": "2026-07-20"
}
```

**Resposta (201 Created):**

```json
{
  "id": "guid-gerado-automaticamente",
  "numeroApolice": "SEG-2026-0001",
  "cpfCnpjSegurado": "52998224725",
  "placaVeiculo": "BRA2E19",
  "valorPremio": 289.90,
  "dataInicioVigencia": "2026-01-01",
  "dataFimVigencia": "2026-07-20",
  "status": 1
}
```

Guarde o campo `id` da resposta — ele será usado nas rotas que exigem `{id}` na URL.

## Usando o `{id}` na URL

Algumas rotas usam `{id}` no caminho da URL. Esse valor é o **GUID** retornado ao criar a apólice (campo `id` da resposta do POST). Substitua `{id}` pelo valor real, por exemplo:

```
guid-gerado-automaticamente
```

**Buscar por ID**

```
GET /api/apolices/guid-gerado-automaticamente
```

**Atualizar**

O `id` vai na URL (qual apólice alterar) e os novos dados vão no corpo da requisição:

```
PUT /api/apolices/guid-gerado-automaticamente
```

```json
{
  "cpfCnpjSegurado": "52998224725",
  "placaVeiculo": "BRA2E19",
  "valorPremio": 350.00,
  "dataInicioVigencia": "2026-01-01",
  "dataFimVigencia": "2026-12-31",
  "status": 2
}
```

**Remover**

```
DELETE /api/apolices/guid-gerado-automaticamente
```

> No Swagger, cole o `id` no campo correspondente ao testar GET, PUT ou DELETE.

## Status da apólice

| Valor | Status |
|-------|--------|
| 1 | Ativa |
| 2 | Cancelada |
| 3 | Expirada |

## Validações

- **CPF:** 11 dígitos (com ou sem máscara)
- **CNPJ numérico:** 14 caracteres com dígitos verificadores válidos
- **CNPJ alfanumérico:** suporte ao novo formato da Receita Federal (ex.: `8H.9XJ.6M0/0001-73`)
- **Placa:** formato brasileiro (ex.: `ABC1234` ou `ABC1D23`)
- **Prêmio:** deve ser maior que zero
- **Vigência:** data de término deve ser posterior à data de início

## Consulta SQL — apólices vencendo

A rota `GET /api/apolices/vencendo` utiliza consulta SQL direta para retornar apólices **ativas** cuja data de término da vigência está entre hoje e os próximos 30 dias.

## Autora

Mayara Pereira 

e-mail: mayaracapereira45@gmail.com
