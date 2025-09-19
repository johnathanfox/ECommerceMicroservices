# 🛒 Sistema de E-Commerce com Microserviços

Olá! Este é um projeto completo de e-commerce construído com microserviços usando .NET 9. Foi desenvolvido pensando em demonstrar conhecimentos sólidos de arquitetura moderna e boas práticas de desenvolvimento.

## 💡 O que você vai encontrar aqui

Criei um sistema que simula uma loja online real, onde você pode:
- Cadastrar produtos no estoque
- Fazer pedidos de compra
- Acompanhar o estoque em tempo real
- Tudo isso de forma segura e escalável!

## ✨ Por que este projeto é especial?

🎯 **Atende todos os requisitos técnicos** que uma empresa moderna espera:
- ✅ **Gestão completa de produtos** - Você pode criar, editar, consultar e remover produtos
- ✅ **Sistema de pedidos inteligente** - Valida se tem estoque antes de confirmar a compra
- ✅ **Comunicação em tempo real** - Os serviços "conversam" entre si automaticamente
- ✅ **Segurança robusta** - Só quem tem permissão acessa o sistema
- ✅ **Código profissional** - Testes, logs, documentação... tudo nos conformes!
- ✅ **Pronto para crescer** - Arquitetura pensada para escalar conforme a demanda  

## 🏗️ Como funciona por dentro?

Imagine que você tem uma loja online. Eu dividi tudo em 3 partes principais que trabalham juntas:

### 🚪 **Porteiro (ApiGateway - Porta 5002)**
É o cara que fica na entrada da sua loja virtual:
- Verifica se você tem permissão para entrar (JWT)
- Te direciona para o lugar certo dependendo do que você quer fazer
- Mantém tudo organizado e seguro
- Tem uma documentação linda no Swagger para você testar tudo

### 🛒 **Vendedor (SalesService - Porta 5000)**
É quem cuida das vendas da sua loja:
- Recebe os pedidos dos clientes
- Confere se tem produto no estoque antes de confirmar
- Acompanha o status de cada pedido (pendente, confirmado, etc.)
- Avisa o estoque quando algo foi vendido
- Guarda tudo no banco de dados para não perder nada

### 📦 **Estoquista (StockService - Porta 5001)**
É quem gerencia todos os produtos:
- Cadastra novos produtos (nome, preço, descrição, quantidade)
- Controla quanto tem de cada item
- Atualiza o estoque automaticamente quando algo é vendido
- Te avisa se um produto está acabando
- Já vem com alguns produtos de exemplo para você testar

## 🚀 Vamos colocar para funcionar?

### O que você precisa ter instalado

1. **.NET 9.0 SDK** - O mais novo da Microsoft
2. **RabbitMQ** - Para os serviços conversarem entre si
3. **Visual Studio 2022** ou **VS Code** - Seu editor favorito

### Instalando o RabbitMQ (é mais fácil do que parece!)

**Se você usa Windows:**
```bash
choco install rabbitmq
```

**Se prefere Docker (recomendo!):**
```bash
docker run -d --hostname rabbitmq --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### Rodando o sistema

É simples! Só seguir estes passos:

1. **Primeiro, baixe as dependências:**
```bash
dotnet restore
```

2. **Agora abra 3 terminais e rode cada serviço:**

```bash
# Terminal 1 - Estoque (roda na porta 5001)
cd StockService
dotnet run

# Terminal 2 - Vendas (roda na porta 5000)
cd SalesService
dotnet run

# Terminal 3 - Gateway (roda na porta 5002)
cd ApiGateway
dotnet run
```

**Pronto!** Agora você pode acessar http://localhost:5002/swagger e brincar com a API 🎉

## 📡 Endpoints Completos

### 🔐 **Autenticação (ApiGateway)**
```http
POST http://localhost:5002/api/auth
Content-Type: application/json

{
  "username": "user",
  "password": "password"
}
```

### 📦 **Gestão de Produtos (via ApiGateway)**

#### Listar Todos os Produtos
```http
GET http://localhost:5002/api/v1/products
Authorization: Bearer {token}
```

#### Cadastrar Novo Produto
```http
POST http://localhost:5002/api/v1/products
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Smartphone Samsung Galaxy S24",
  "description": "Smartphone Android com 256GB de armazenamento",
  "price": 2499.99,
  "quantity": 50
}
```

#### Consultar Produto Específico
```http
GET http://localhost:5002/api/v1/products/{productId}
Authorization: Bearer {token}
```

#### Atualizar Produto
```http
PUT http://localhost:5002/api/v1/products/{productId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Nome Atualizado",
  "price": 1999.99,
  "quantity": 30
}
```

#### Verificar Disponibilidade de Estoque
```http
GET http://localhost:5002/api/v1/products/{productId}/availability?quantity=5
Authorization: Bearer {token}
```

### 🛒 **Gestão de Pedidos (via ApiGateway)**

#### Criar Pedido
```http
POST http://localhost:5002/api/v1/orders
Authorization: Bearer {token}
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2,
  "customerName": "João Silva",
  "customerEmail": "joao@exemplo.com"
}
```

#### Listar Todos os Pedidos
```http
GET http://localhost:5002/api/v1/orders
Authorization: Bearer {token}
```

#### Consultar Pedido Específico
```http
GET http://localhost:5002/api/v1/orders/{orderId}
Authorization: Bearer {token}
```

#### Consultar Pedidos por Cliente
```http
GET http://localhost:5002/api/v1/orders/customer/{email}
Authorization: Bearer {token}
```

#### Atualizar Status do Pedido
```http
PATCH http://localhost:5002/api/v1/orders/{orderId}/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "Confirmed"
}
```

### 🔧 **Acesso Direto aos Serviços (Desenvolvimento)**

#### SalesService (http://localhost:5000)
- `GET /api/orders` - Listar pedidos
- `POST /api/orders` - Criar pedido
- `GET /api/orders/{id}` - Consultar pedido
- `GET /api/orders/customer/{email}` - Pedidos por cliente

#### StockService (http://localhost:5001)
- `GET /api/stock` - Listar produtos
- `POST /api/stock` - Cadastrar produto
- `GET /api/stock/{id}` - Consultar produto
- `PUT /api/stock/{id}` - Atualizar produto
- `DELETE /api/stock/{id}` - Remover produto

## 🔧 Configuração

### Portas dos Serviços
- **ApiGateway**: 5002
- **SalesService**: 5000  
- **StockService**: 5001

### JWT Configuration
As configurações JWT estão no `appsettings.json` do ApiGateway:
```json
{
  "Jwt": {
    "Key": "a-very-secure-jwt-key-that-is-at-least-256-bits-long-for-production-use-2024",
    "Issuer": "ECommerceMicroservices",
    "Audience": "ECommerceClients"
  }
}
```

### RabbitMQ
- **Host**: localhost
- **Porta**: 5672
- **Fila**: order_stock_queue

## 🧪 Vamos testar se está tudo funcionando?

Aqui estão alguns comandos para você testar rapidinho:

### 1. **Primeiro, pegue seu "passe" de entrada:**
```bash
curl -X POST http://localhost:5002/api/auth \
  -H "Content-Type: application/json" \
  -d '{"username":"user","password":"password"}'
```

### 2. **Agora faça um pedido:**
```bash
curl -X POST http://localhost:5002/api/v1/orders \
  -H "Authorization: Bearer {seu-token}" \
  -H "Content-Type: application/json" \
  -d '{"productId":1,"quantity":3,"customerName":"João","customerEmail":"joao@teste.com"}'
```

### 3. **Confira se o estoque foi atualizado:**
```bash
curl -X GET http://localhost:5002/api/v1/products/1 \
  -H "Authorization: Bearer {seu-token}"
```

## 📊 Documentação interativa (Swagger)

Cada serviço tem sua própria documentação onde você pode testar tudo na interface:

- **Portal Principal**: http://localhost:5002/swagger
- **Vendas**: http://localhost:5000/swagger  
- **Estoque**: http://localhost:5001/swagger

## 🔄 Como tudo funciona junto (o fluxo mágico!)

1. **Você faz login** → Recebe um token de acesso
2. **Faz um pedido** → O sistema confere se tem estoque
3. **Se tiver produto** → Confirma o pedido e avisa o estoque
4. **O estoque se atualiza sozinho** → Via mensageria (RabbitMQ)
5. **Você pode acompanhar tudo** → Consultas em tempo real

É como uma orquestra onde cada músico (serviço) toca sua parte perfeitamente! 🎵

## 🛠️ Tecnologias Utilizadas

- **ASP.NET Core 9.0** - Framework principal
- **Entity Framework Core 9.0** - ORM com InMemory Database
- **Ocelot 24.0.1** - API Gateway
- **JWT Bearer Authentication** - Segurança
- **RabbitMQ** - Message Broker assíncrono
- **Serilog** - Logs estruturados
- **Swagger/OpenAPI** - Documentação automática
- **xUnit + Moq** - Testes unitários
- **Dependency Injection** - Injeção de dependência nativa

## 🧪 Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test Tests/StockService.Tests/
```

## 📊 Funcionalidades Implementadas vs Requisitos

| Requisito | Status | Implementação |
|-----------|--------|---------------|
| Cadastro de Produtos | ✅ | CRUD completo no StockService |
| Consulta de Produtos | ✅ | API REST com filtros |
| Atualização de Estoque | ✅ | Automática via RabbitMQ |
| Criação de Pedidos | ✅ | Com validação de estoque |
| Consulta de Pedidos | ✅ | Por ID, cliente, listagem |
| Notificação de Vendas | ✅ | RabbitMQ assíncrono |
| Autenticação JWT | ✅ | Chave segura de 256 bits |
| API Gateway | ✅ | Ocelot com roteamento |
| Entity Framework | ✅ | InMemory + SQL Server ready |
| Tratamento de Exceções | ✅ | Try-catch + logs estruturados |
| Validações de Entrada | ✅ | Data Annotations + ModelState |
| Logs Estruturados | ✅ | Serilog com arquivo + console |
| Testes Unitários | ✅ | xUnit + Moq + InMemory DB |
| Separação de Responsabilidades | ✅ | Services + Controllers + DTOs |
| Boas Práticas de API | ✅ | RESTful + HTTP Status Codes |

## 🎯 Critérios de Aceitação - TODOS ATENDIDOS

✅ **Sistema permite cadastro de produtos** - StockService com CRUD completo  
✅ **Sistema permite criação de pedidos** - SalesService com validação  
✅ **Comunicação eficiente via RabbitMQ** - Mensagens assíncronas  
✅ **API Gateway direciona requisições** - Ocelot configurado  
✅ **Sistema seguro com JWT** - Autenticação em todos endpoints  
✅ **Código bem estruturado** - Services, DTOs, separação clara  

## 🏆 Extras Implementados

✅ **Testes Unitários** - Cobertura das funcionalidades principais  
✅ **Logs Estruturados** - Serilog para monitoramento  
✅ **Validações Robustas** - Data Annotations + tratamento de erros  
✅ **Documentação Swagger** - APIs autodocumentadas  
✅ **Entity Framework** - Persistência profissional  
✅ **Arquitetura Escalável** - Pronta para novos microserviços  


