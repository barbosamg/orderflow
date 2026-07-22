# TODO - OrderFlow

Checklist operacional completo para construir o projeto descrito no `PLAN.md` e na apostila.

## Como usar este arquivo

- Marcar `[x]` somente depois de executar e validar o item.
- Registrar comandos executados, resultados e problemas relevantes no `handoff.md`.
- Atualizar o `handoff.md` antes de encerrar cada sessão.
- Não avançar de fase enquanto o critério de saída da fase atual não estiver atendido.
- Criar commits pequenos depois de cada entrega funcional validada.
- Nunca versionar senhas, tokens, connection strings reais ou dados pessoais.

## Estado geral

- [x] Apostila lida integralmente, totalizando 37 páginas.
- [x] Planejamento consolidado em `PLAN.md`.
- [x] Checklist operacional criado em `TODO.md`.
- [x] Registro de continuidade criado em `handoff.md`.
- [x] Ambiente de desenvolvimento validado.
- [x] Solução .NET criada e estrutura inicial validada.
- [ ] MVP funcional em Docker Compose.
- [ ] Testes automatizados concluídos.
- [ ] Implantação Kubernetes validada.
- [ ] Pipeline de CI/CD validado.
- [ ] Release de portfólio publicada.

---

## 1. Preparação do ambiente

### 1.1 Validar Windows, PowerShell e WSL 2

- [ ] Abrir o PowerShell como administrador quando o comando exigir elevação.
- [x] Confirmar Windows 11 atualizado por `Get-ComputerInfo` ou `winver`.
- [x] Executar `$PSVersionTable` e registrar a versão do PowerShell no `handoff.md`.
- [x] Executar `wsl --status`.
- [x] Executar `wsl --update`.
- [ ] Se o WSL não estiver instalado, executar `wsl --install` e reiniciar o computador.
- [x] Confirmar que o WSL 2 está configurado como backend do Docker Desktop.

### 1.2 Validar Git e .NET

- [x] Executar `git --version`.
- [x] Executar `dotnet --info`.
- [x] Executar `dotnet --list-sdks`.
- [x] Executar `dotnet --version`.
- [x] Confirmar que o SDK principal começa com `10.`.
- [ ] Se necessário, instalar uma versão .NET LTS compatível e ajustar imagens/pacotes do projeto.
- [x] Configurar nome e e-mail do Git se ainda não existirem.
- [x] Executar `git config --global user.name`.
- [x] Executar `git config --global user.email`.

### 1.3 Validar Docker Desktop e Kubernetes

- [x] Abrir o Docker Desktop.
- [x] Confirmar o mecanismo baseado em WSL 2.
- [x] Executar `docker version`.
- [x] Confirmar que `docker version` mostra Client e Server.
- [x] Executar `docker compose version`.
- [x] Habilitar Kubernetes no Docker Desktop.
- [x] Executar `kubectl version --client`.
- [x] Executar `kubectl config current-context`.
- [x] Confirmar o contexto `docker-desktop` ou registrar o contexto adotado.
- [x] Executar `kubectl get nodes`.
- [x] Confirmar pelo menos um nó em estado `Ready`.

### 1.4 Validar ferramentas auxiliares

- [x] Escolher VS Code ou Visual Studio como editor principal.
- [ ] Instalar C# Dev Kit.
- [ ] Instalar extensões Docker, Kubernetes, REST Client e YAML.
- [ ] Instalar uma ferramenta opcional para PostgreSQL, como DBeaver ou pgAdmin.
- [ ] (Opcional, sem impacto no OrderFlow) Confirmar que o Poppler está acessível em um novo terminal com `pdfinfo -v`.

### 1.5 Verificar portas locais

- [x] Confirmar disponibilidade da porta PostgreSQL `5432`.
- [x] Confirmar disponibilidade da porta RabbitMQ AMQP `5672`.
- [x] Confirmar disponibilidade da porta API `8080`.
- [x] Confirmar disponibilidade da porta RabbitMQ Management `15672`.
- [ ] Se houver conflito, documentar e definir portas externas alternativas.

### 1.6 Critério de saída da preparação

- [x] .NET, Git, Docker, Compose e kubectl respondem sem erro.
- [x] Docker Client e Server estão acessíveis.
- [x] Kubernetes possui nó `Ready`.
- [x] Versões verificadas foram registradas no `handoff.md`.

---

## 2. Inicialização do repositório e da solução

### 2.1 Inicializar Git

- [x] Executar `git init` na raiz do projeto.
- [x] Confirmar a branch inicial com `git branch --show-current`.
- [x] Renomear para `main`, se necessário, com `git branch -M main`.
- [x] Criar `.gitignore` com `dotnet new gitignore`.
- [ ] Revisar o `.gitignore` para excluir `bin`, `obj`, `.vs`, cobertura, resultados de testes e segredos locais.

### 2.2 Criar solução e diretórios

- [x] Executar `dotnet new sln -n OrderFlow` e criar `OrderFlow.slnx`.
- [x] Criar o diretório `src`.
- [x] Criar o diretório `tests`.
- [ ] Criar posteriormente `requests`, `k8s`, `docs` e `.github/workflows` quando suas fases começarem.

### 2.3 Criar projetos

- [x] Executar `dotnet new classlib -n OrderFlow.Domain -o .\src\OrderFlow.Domain --framework net10.0 --no-restore`.
- [x] Executar `dotnet new classlib -n OrderFlow.Application -o .\src\OrderFlow.Application --framework net10.0 --no-restore`.
- [x] Executar `dotnet new classlib -n OrderFlow.Infrastructure -o .\src\OrderFlow.Infrastructure --framework net10.0 --no-restore`.
- [x] Executar `dotnet new webapi -n OrderFlow.Api -o .\src\OrderFlow.Api --use-controllers --framework net10.0 --no-restore`.
- [x] Executar `dotnet new worker -n OrderFlow.Worker -o .\src\OrderFlow.Worker --framework net10.0 --no-restore`.
- [x] Executar `dotnet new xunit -n OrderFlow.Tests -o .\tests\OrderFlow.Tests --framework net10.0 --no-restore`.
- [ ] Remover arquivos de exemplo que não serão utilizados.

### 2.4 Adicionar projetos à solução

- [x] Adicionar `OrderFlow.Domain` à solução.
- [x] Adicionar `OrderFlow.Application` à solução.
- [x] Adicionar `OrderFlow.Infrastructure` à solução.
- [x] Adicionar `OrderFlow.Api` à solução.
- [x] Adicionar `OrderFlow.Worker` à solução.
- [x] Adicionar `OrderFlow.Tests` à solução.
- [x] Executar `dotnet sln list` e conferir os seis projetos.

### 2.5 Configurar referências entre projetos

- [x] Application referencia Domain.
- [x] Infrastructure referencia Domain.
- [x] Infrastructure referencia Application.
- [x] API referencia Application.
- [x] API referencia Infrastructure.
- [x] Worker referencia Application.
- [x] Worker referencia Infrastructure.
- [x] Tests referencia Domain.
- [x] Tests referencia Application.
- [x] Confirmar que Domain não referencia infraestrutura ou frameworks web.

### 2.6 Instalar pacotes

- [x] Instalar `Npgsql.EntityFrameworkCore.PostgreSQL` em Infrastructure.
- [x] Instalar `Microsoft.EntityFrameworkCore.Design` em Infrastructure.
- [x] Instalar `RabbitMQ.Client` em Infrastructure.
- [x] Confirmar que a API já usa `Microsoft.AspNetCore.OpenApi`; `Swashbuckle.AspNetCore` não foi necessário.
- [x] Fixar `Microsoft.OpenApi` 2.7.5 para corrigir a vulnerabilidade `GHSA-v5pm-xwqc-g5wc`.
- [x] Instalar `Microsoft.EntityFrameworkCore.Design` em API.
- [x] Confirmar que `Microsoft.EntityFrameworkCore.Design` não é necessário no Worker; a API será o projeto de inicialização das migrations.
- [x] Instalar ou atualizar a ferramenta global `dotnet-ef`.
- [x] Fixar versões compatíveis com .NET 10 nos arquivos `.csproj`.
- [x] Executar `dotnet list package` e revisar as versões resolvidas.

### 2.7 Validar estrutura inicial

- [x] Executar `dotnet restore`.
- [x] Executar `dotnet build`.
- [x] Executar `dotnet test`.
- [x] Confirmar zero erros.
- [x] Revisar `git status --short`.
- [x] Criar o commit `chore: configure project references and dependencies` (`371152c`).
- [x] Atualizar `handoff.md` com o hash do commit.

---

## 3. Domínio e regras de negócio

### 3.1 Status do pedido

- [x] Criar `src/OrderFlow.Domain/Orders/OrderStatus.cs`.
- [x] Adicionar Created, Confirmed, Preparing, Shipped, Delivered e Cancelled.
- [x] Definir transições válidas de status.
- [x] Rejeitar alteração após Delivered ou Cancelled.
- [x] Testar transições válidas e inválidas.

### 3.2 Produto

- [x] Criar `Product` com Id, Name, Description, Price, Stock, IsActive e CreatedAt.
- [x] Criar construtor protegido para EF Core.
- [x] Validar nome obrigatório.
- [x] Validar preço maior que zero.
- [x] Arredondar preço para duas casas decimais.
- [x] Validar estoque não negativo.
- [x] Implementar alteração de dados.
- [x] Implementar alteração de estoque.
- [x] Implementar baixa de estoque.
- [x] Rejeitar quantidade inválida ou superior ao estoque.
- [x] Implementar ativação e desativação lógica.

### 3.3 Cliente

- [x] Criar `Customer` com Id, Name, Email e CreatedAt.
- [x] Validar nome obrigatório.
- [x] Validar formato mínimo do e-mail.
- [x] Normalizar espaços do nome.
- [x] Normalizar e-mail para letras minúsculas.
- [ ] Decidir e implementar unicidade do e-mail no banco.

### 3.4 Pedido e itens

- [x] Criar `Order` com coleção privada de itens.
- [x] Criar `OrderItem`.
- [x] Validar CustomerId.
- [x] Validar ProductId.
- [x] Validar quantidade e preço unitário.
- [x] Copiar o nome e preço do produto para o item como snapshot.
- [x] Recalcular o total sempre que um item for incluído.
- [x] Impedir pedido vazio.
- [x] Definir tratamento de ProductId repetido na mesma requisição.
- [x] Atualizar `UpdatedAt` ao mudar o status.

### 3.5 Chat e eventos processados

- [x] Criar `ChatMessage`.
- [x] Validar OrderId, Sender e Text.
- [x] Limitar texto a 1.000 caracteres.
- [x] Criar `ProcessedEvent`.
- [x] Armazenar EventId, EventType, Payload e ProcessedAt.

### 3.6 Testes unitários do domínio

- [x] Testar cálculo do total com múltiplos itens.
- [x] Testar preço inválido.
- [x] Testar estoque negativo.
- [x] Testar retirada acima do estoque.
- [x] Testar pedido sem itens.
- [x] Testar pedido sem cliente.
- [x] Testar alteração de pedido finalizado.
- [x] Testar transições inválidas de status.
- [x] Testar mensagem vazia.
- [x] Testar mensagem acima do limite.
- [x] Executar `dotnet test`.
- [x] Criar os commits funcionais das entidades da Fase 2.
- [x] Atualizar `handoff.md`.

---

## 4. PostgreSQL e Entity Framework Core

### 4.1 DbContext

- [x] Criar `OrderFlowDbContext`.
- [x] Adicionar DbSet de Products.
- [x] Adicionar DbSet de Customers.
- [x] Adicionar DbSet de Orders.
- [x] Adicionar DbSet de OrderItems.
- [x] Adicionar DbSet de ChatMessages.
- [x] Adicionar DbSet de ProcessedEvents.
- [x] Aplicar configurações da assembly automaticamente.

### 4.2 Mapeamentos EF Core

- [x] Criar `ProductConfiguration`.
- [x] Criar `CustomerConfiguration`.
- [x] Criar `OrderConfiguration`.
- [x] Criar `OrderItemConfiguration`.
- [x] Criar `ChatMessageConfiguration`.
- [x] Criar `ProcessedEventConfiguration`.
- [x] Definir nomes de tabelas e chaves primárias.
- [x] Definir tamanhos máximos das strings.
- [x] Definir precisão decimal `12,2`.
- [x] Converter status para string.
- [x] Configurar Customer -> Orders com exclusão restrita.
- [x] Configurar Order -> Items com exclusão em cascata.
- [x] Configurar acesso por campo à coleção privada de itens.

### 4.3 Índices e integridade

- [x] Criar índice de `products(name)`.
- [x] Criar índice de `orders(customer_id, created_at)`.
- [x] Criar índice de `chat_messages(order_id, sent_at)`.
- [x] Criar índice único de `processed_events(event_id)`.
- [x] Criar índice único de e-mail do cliente.
- [x] Revisar nomes e tipos gerados para PostgreSQL.

### 4.4 Injeção de dependência e configuração

- [x] Criar `Infrastructure.DependencyInjection`.
- [x] Ler a connection string `Postgres`.
- [x] Falhar na inicialização se a connection string estiver ausente.
- [x] Registrar `OrderFlowDbContext` com Npgsql.
- [x] Criar `appsettings.Development.json` para laboratório local.
- [x] Garantir que credenciais reais não sejam versionadas.

### 4.5 Migration inicial

- [x] Executar `dotnet ef migrations add InitialCreate --project src/OrderFlow.Infrastructure --startup-project src/OrderFlow.Api --output-dir Persistence/Migrations`.
- [x] Revisar todos os comandos da migration.
- [x] Subir PostgreSQL para desenvolvimento.
- [x] Executar `dotnet ef database update --project src/OrderFlow.Infrastructure --startup-project src/OrderFlow.Api`.
- [x] Inspecionar tabelas, FKs e índices no banco.
- [x] Testar que EventId duplicado é rejeitado.
- [x] Executar build e testes.
- [x] Criar o commit `feat: adiciona persistência PostgreSQL e migration inicial` (`a7c3bf5`).
- [x] Atualizar `handoff.md`.

---

## 5. API REST

### 5.1 DTOs e contratos

- [x] Criar DTOs de criação e resposta de produto.
- [x] Criar DTOs de atualização de produto.
- [x] Criar DTOs de criação e resposta de cliente.
- [x] Criar DTOs de criação e detalhamento de pedido.
- [x] Criar DTO de alteração de status.
- [x] Criar DTO de mensagem do chat.
- [x] Não retornar entidades EF diretamente.

### 5.2 ProdutosController

- [x] Implementar `GET /api/products`.
- [x] Implementar `GET /api/products/{id}`.
- [x] Implementar `POST /api/products`.
- [x] Implementar `PUT /api/products/{id}`.
- [x] Implementar `DELETE /api/products/{id}` como desativação lógica.
- [x] Usar `AsNoTracking` nas consultas somente leitura.
- [x] Retornar 404 para produto inexistente.
- [x] Retornar 201 e localização na criação.

### 5.3 CustomersController

- [x] Implementar `GET /api/customers`.
- [x] Implementar `GET /api/customers/{id}` se necessário.
- [x] Implementar `POST /api/customers`.
- [x] Tratar conflito de e-mail duplicado.

### 5.4 Criação de pedido

- [x] Criar contratos de serviço, repositório e publicação do evento de pedido.
- [x] Preparar repositórios rastreados e unidade de trabalho com transação serializável.
- [ ] Criar `OrderService`.
- [ ] Rejeitar pedido sem itens.
- [ ] Confirmar existência do cliente.
- [ ] Buscar apenas produtos ativos.
- [ ] Validar todos os produtos solicitados.
- [ ] Tratar itens repetidos de forma explícita.
- [ ] Abrir transação no banco.
- [ ] Baixar estoque.
- [ ] Montar pedido e itens.
- [ ] Salvar alterações.
- [ ] Confirmar a transação.
- [ ] Publicar `order.created` após o commit no MVP.
- [ ] Registrar em comentário/documentação a evolução futura para Outbox.

### 5.5 OrdersController

- [ ] Implementar `POST /api/orders`.
- [ ] Implementar `GET /api/orders/{id}` com itens.
- [ ] Implementar `PATCH /api/orders/{id}/status`.
- [ ] Implementar `GET /api/orders/{id}/messages`.
- [ ] Retornar status HTTP coerentes.

### 5.6 Inicialização da API

- [ ] Registrar Controllers.
- [ ] Validar e manter o OpenAPI nativo do ASP.NET Core.
- [ ] Registrar SignalR.
- [ ] Registrar Infrastructure.
- [ ] Registrar OrderService.
- [ ] Registrar health checks.
- [ ] Servir arquivos estáticos.
- [ ] Mapear Controllers.
- [ ] Mapear `/health`.
- [ ] Mapear posteriormente `/hubs/orders`.

### 5.7 Requisições de validação

- [ ] Criar `requests/OrderFlow.http`.
- [ ] Adicionar criação e listagem de produtos.
- [ ] Adicionar criação e listagem de clientes.
- [ ] Adicionar criação e consulta de pedido.
- [ ] Adicionar alteração de status.
- [ ] Adicionar consulta do histórico.
- [ ] Validar total calculado.
- [ ] Validar baixa de estoque.
- [ ] Validar rollback em falha intermediária.
- [ ] Executar build e testes.
- [ ] Criar o commit `feat: implementa API de produtos clientes e pedidos`.
- [ ] Atualizar `handoff.md`.

---

## 6. RabbitMQ e Worker

### 6.1 Contratos e opções

- [ ] Criar `OrderCreatedEvent`.
- [ ] Criar `IEventPublisher`.
- [ ] Criar `RabbitMqOptions`.
- [ ] Configurar Host, Port, User, Password, Exchange e Queue.

### 6.2 Publicador

- [ ] Criar `RabbitMqPublisher`.
- [ ] Manter conexão e canal de longa duração.
- [ ] Habilitar recuperação automática.
- [ ] Declarar exchange topic durable.
- [ ] Serializar mensagens em JSON UTF-8.
- [ ] Definir ContentType `application/json`.
- [ ] Marcar mensagens como persistentes.
- [ ] Proteger publicação concorrente no mesmo canal.
- [ ] Implementar descarte assíncrono.
- [ ] Registrar o publicador como singleton.

### 6.3 Worker consumidor

- [ ] Criar `OrderCreatedWorker`.
- [ ] Declarar exchange `orderflow.events`.
- [ ] Declarar fila durable `orderflow.order-created`.
- [ ] Criar binding para routing key `order.created`.
- [ ] Configurar prefetch com `BasicQos`.
- [ ] Consumir com `autoAck: false`.
- [ ] Desserializar e validar o evento.
- [ ] Criar scope por mensagem.
- [ ] Verificar EventId antes de processar.
- [ ] Registrar ProcessedEvent.
- [ ] Enviar ack somente após persistência bem-sucedida.
- [ ] Enviar nack sem requeue em falha não recuperada.
- [ ] Liberar conexão e canal no encerramento.
- [ ] Registrar Worker no `Program.cs`.

### 6.4 Testes de mensageria

- [ ] Confirmar exchange no painel do RabbitMQ.
- [ ] Confirmar fila e binding.
- [ ] Criar pedido e observar publicação.
- [ ] Confirmar consumo no log do Worker.
- [ ] Confirmar registro em `processed_events`.
- [ ] Reenviar o mesmo EventId.
- [ ] Confirmar ausência de efeito duplicado.
- [ ] Testar payload inválido sem loop infinito.
- [ ] Criar o commit `feat: publica e consome eventos de pedido criado`.
- [ ] Atualizar `handoff.md`.

---

## 7. SignalR e interface de demonstração

### 7.1 Hub e grupos

- [ ] Criar `OrderChatHub`.
- [ ] Criar padrão de grupo `order:{orderId}`.
- [ ] Implementar `JoinOrder`.
- [ ] Implementar `LeaveOrder`.
- [ ] Validar pedido antes de entrar na sala.
- [ ] Implementar `SendMessage`.
- [ ] Persistir mensagem antes do broadcast.
- [ ] Emitir `MessageReceived` somente ao grupo correto.
- [ ] Não guardar estado permanente em propriedades do Hub.

### 7.2 Status em tempo real

- [ ] Injetar `IHubContext<OrderChatHub>` em OrdersController.
- [ ] Emitir `StatusChanged` após persistir o novo status.
- [ ] Enviar evento somente para `order:{orderId}`.

### 7.3 Página web

- [ ] Criar `src/OrderFlow.Api/wwwroot/index.html`.
- [ ] Adicionar campo de OrderId.
- [ ] Adicionar campo de remetente.
- [ ] Adicionar conexão automática/reconexão SignalR.
- [ ] Adicionar lista de mensagens.
- [ ] Adicionar envio de mensagem.
- [ ] Mostrar estado conectado/desconectado.
- [ ] Carregar histórico por HTTP ao entrar na sala.
- [ ] Tratar erro de pedido inexistente.
- [ ] Desabilitar envio quando desconectado.
- [ ] Escapar conteúdo por `textContent` para evitar HTML injetado.

### 7.4 Testes manuais SignalR

- [ ] Criar dois pedidos diferentes.
- [ ] Abrir duas janelas no primeiro pedido.
- [ ] Trocar mensagens nos dois sentidos.
- [ ] Abrir terceira janela no segundo pedido.
- [ ] Confirmar isolamento entre as salas.
- [ ] Alterar status e observar as janelas corretas.
- [ ] Atualizar a página e recuperar histórico.
- [ ] Testar reconexão após reiniciar a API.
- [ ] Criar o commit `feat: adiciona chat por pedido e atualizações de status em tempo real`.
- [ ] Atualizar `handoff.md`.

---

## 8. Docker e Docker Compose

### 8.1 Imagens da aplicação

- [ ] Criar Dockerfile multi-stage da API.
- [ ] Usar SDK .NET 10 na etapa de build.
- [ ] Usar runtime ASP.NET 10 na imagem final da API.
- [ ] Expor porta 8080.
- [ ] Criar Dockerfile multi-stage do Worker.
- [ ] Usar runtime .NET 10 na imagem final do Worker.
- [ ] Criar `.dockerignore`.

### 8.2 Compose

- [ ] Criar `docker-compose.yml`.
- [ ] Adicionar PostgreSQL com volume e health check.
- [ ] Adicionar RabbitMQ Management com volume e health check.
- [ ] Adicionar API com build, portas, ambiente e dependências saudáveis.
- [ ] Adicionar Worker com build, ambiente e dependências saudáveis.
- [ ] Usar `postgres` e `rabbitmq` como hostnames internos.
- [ ] Não usar `localhost` entre containers.
- [ ] Configurar aplicação de migrations para laboratório.

### 8.3 Validação do Compose

- [ ] Executar `docker compose build`.
- [ ] Executar `docker compose up -d`.
- [ ] Executar `docker compose ps`.
- [ ] Confirmar todos os serviços saudáveis.
- [ ] Abrir `http://localhost:8080`.
- [ ] Abrir `http://localhost:8080/openapi/v1.json`.
- [ ] Validar `http://localhost:8080/health` com resposta 200.
- [ ] Abrir `http://localhost:15672`.
- [ ] Executar o fluxo produto -> cliente -> pedido -> Worker -> chat.
- [ ] Executar `docker compose logs --tail 100 api`.
- [ ] Executar `docker compose logs --tail 100 worker`.
- [ ] Confirmar persistência após reiniciar containers.
- [ ] Documentar que `docker compose down -v` apaga os volumes.
- [ ] Criar o commit `feat: executa stack do OrderFlow com Docker Compose`.
- [ ] Atualizar `handoff.md`.

---

## 9. Qualidade, erros e observabilidade básica

### 9.1 Tratamento de erros

- [ ] Implementar Problem Details global.
- [ ] Mapear ArgumentException para 400.
- [ ] Mapear KeyNotFoundException para 404.
- [ ] Mapear InvalidOperationException para 409.
- [ ] Mapear falhas inesperadas para 500.
- [ ] Evitar expor stack trace em ambiente não local.

### 9.2 Logs

- [ ] Usar logs estruturados.
- [ ] Incluir OrderId no fluxo de pedido.
- [ ] Incluir EventId no fluxo assíncrono.
- [ ] Usar Information para fluxo normal.
- [ ] Usar Warning para situações recuperáveis.
- [ ] Usar Error para falhas.
- [ ] Não registrar senhas, tokens ou connection strings.
- [ ] Confirmar logs em stdout/stderr.

### 9.3 Testes e segurança básica

- [ ] Executar todos os testes unitários.
- [ ] Coletar cobertura com `dotnet test --collect:"XPlat Code Coverage"`.
- [ ] Adicionar testes de integração com Testcontainers.
- [ ] Testar API com PostgreSQL real em container.
- [ ] Testar RabbitMQ real em container.
- [ ] Testar SignalR com múltiplos clientes.
- [ ] Testar rollback e concorrência de estoque.
- [ ] Executar `dotnet list package --vulnerable --include-transitive`.
- [ ] Revisar repositório em busca de segredos.
- [ ] Executar build Release sem warnings relevantes.
- [ ] Criar o commit `test: cobre domínio API mensageria e fluxos em tempo real`.
- [ ] Atualizar `handoff.md`.

---

## 10. Kubernetes local

### 10.1 Preparar imagens

- [ ] Definir usuário/organização do GHCR.
- [ ] Criar imagem versionada da API.
- [ ] Criar imagem versionada do Worker.
- [ ] Executar `docker login ghcr.io`.
- [ ] Publicar imagem da API.
- [ ] Publicar imagem do Worker.
- [ ] Confirmar visibilidade adequada dos pacotes.

### 10.2 Manifests base

- [ ] Criar `k8s/00-base.yaml`.
- [ ] Criar namespace `orderflow`.
- [ ] Criar ConfigMap.
- [ ] Criar Secret somente para laboratório.
- [ ] Não deixar senha literal na connection string dos Deployments.

### 10.3 PostgreSQL

- [ ] Criar `k8s/10-postgres.yaml`.
- [ ] Criar PVC de 2 Gi ou capacidade definida.
- [ ] Criar Deployment com uma réplica.
- [ ] Montar `/var/lib/postgresql/data`.
- [ ] Configurar readiness probe.
- [ ] Configurar requests e limits.
- [ ] Criar Service na porta 5432.

### 10.4 RabbitMQ

- [ ] Criar `k8s/20-rabbitmq.yaml`.
- [ ] Criar Deployment com uma réplica.
- [ ] Configurar credenciais via Secret.
- [ ] Configurar readiness probe.
- [ ] Criar Service para 5672 e 15672.

### 10.5 API

- [ ] Criar `k8s/30-api.yaml`.
- [ ] Referenciar imagem versionada no GHCR.
- [ ] Configurar variáveis por ConfigMap e Secret.
- [ ] Configurar readiness probe em `/health`.
- [ ] Configurar liveness probe em `/health`.
- [ ] Configurar requests e limits.
- [ ] Manter uma réplica da API no MVP.
- [ ] Criar Service na porta 8080.

### 10.6 Worker

- [ ] Criar `k8s/40-worker.yaml`.
- [ ] Referenciar imagem versionada no GHCR.
- [ ] Configurar variáveis por ConfigMap e Secret.
- [ ] Configurar requests e limits.

### 10.7 Aplicar e validar

- [ ] Executar `kubectl config use-context docker-desktop` quando aplicável.
- [ ] Executar `kubectl apply -f k8s/`.
- [ ] Executar `kubectl get all -n orderflow`.
- [ ] Executar `kubectl get pvc -n orderflow`.
- [ ] Confirmar PVC `Bound`.
- [ ] Confirmar Pods `Running` e `Ready`.
- [ ] Acompanhar rollout da API.
- [ ] Acompanhar logs da API.
- [ ] Acompanhar logs do Worker.
- [ ] Executar port-forward da API.
- [ ] Executar port-forward do RabbitMQ Management.
- [ ] Validar o fluxo completo.
- [ ] Excluir um Pod da API e confirmar autorrecuperação.
- [ ] Escalar Worker para três réplicas.
- [ ] Confirmar múltiplos consumidores sem duplicidade de efeito.
- [ ] Criar o commit `feat: implanta OrderFlow no Kubernetes local`.
- [ ] Atualizar `handoff.md`.

---

## 11. GitHub Actions e publicação das imagens

### 11.1 Pipeline de build e testes

- [ ] Criar `.github/workflows/ci.yml`.
- [ ] Executar workflow em push para `main`.
- [ ] Executar workflow em Pull Requests para `main`.
- [ ] Configurar permissões mínimas necessárias.
- [ ] Fazer checkout do código.
- [ ] Configurar .NET 10.
- [ ] Executar restore.
- [ ] Executar build Release sem restore.
- [ ] Executar testes sem rebuild.

### 11.2 Pipeline Docker/GHCR

- [ ] Executar job Docker somente após build e testes.
- [ ] Autenticar no GHCR com `GITHUB_TOKEN`.
- [ ] Construir e publicar imagem da API.
- [ ] Construir e publicar imagem do Worker.
- [ ] Publicar tag por SHA ou versão.
- [ ] Opcionalmente atualizar tag `latest`.
- [ ] Confirmar pacotes publicados no GHCR.
- [ ] Confirmar pipeline verde em máquina limpa.
- [ ] Criar o commit `ci: build test and publish container images`.
- [ ] Atualizar `handoff.md`.

---

## 12. README e documentação

### 12.1 Estrutura do README

- [x] Criar `README.md`.
- [x] Explicar proposta e problema resolvido.
- [ ] Adicionar demonstração visual no início.
- [x] Adicionar diagrama da arquitetura.
- [x] Explicar os fluxos HTTP, SignalR e RabbitMQ.
- [x] Listar tecnologias e versões.
- [x] Listar funcionalidades do MVP.
- [x] Documentar pré-requisitos.
- [ ] Documentar execução com Docker Compose.
- [ ] Documentar URLs locais.
- [ ] Documentar execução no Kubernetes.
- [x] Documentar testes.
- [x] Explicar idempotência, ack manual e limitação sem Outbox.
- [x] Explicar uma réplica da API SignalR no MVP.
- [ ] Adicionar seção de solução de problemas.
- [x] Adicionar próximos passos.

### 12.2 Evidências visuais

- [ ] Capturar catálogo/produtos e pedidos.
- [ ] Capturar duas janelas trocando mensagens.
- [ ] Capturar mudança de status em tempo real.
- [ ] Capturar o documento OpenAPI ou a interface de exploração adotada.
- [ ] Capturar RabbitMQ Management.
- [ ] Capturar containers no Docker Desktop.
- [ ] Capturar recursos Kubernetes.
- [ ] Capturar GitHub Actions verde.
- [ ] Verificar que screenshots não contêm dados pessoais ou segredos.

### 12.3 Material de portfólio

- [ ] Gravar vídeo de 60 a 90 segundos.
- [ ] Apresentar problema, stack e arquitetura.
- [ ] Demonstrar cadastro e criação de pedido.
- [ ] Demonstrar chat e status em tempo real.
- [ ] Demonstrar RabbitMQ e Worker.
- [ ] Demonstrar Docker/Kubernetes e pipeline.
- [ ] Preparar texto de publicação com decisões técnicas concretas.
- [ ] Criar o commit `docs: adiciona arquitetura configuração e demonstração do projeto`.
- [ ] Atualizar `handoff.md`.

---

## 13. Release do MVP

### 13.1 Auditoria final

- [ ] Clonar o repositório em uma pasta limpa.
- [ ] Executar restore, build e testes.
- [ ] Iniciar tudo com Docker Compose seguindo apenas o README.
- [ ] Executar o fluxo funcional completo.
- [ ] Validar todos os itens da matriz de aceitação do `PLAN.md`.
- [ ] Verificar ausência de segredos.
- [ ] Verificar links e comandos do README.
- [ ] Verificar imagens e manifests publicados.

### 13.2 Publicação

- [ ] Escolher e adicionar licença, por exemplo MIT.
- [ ] Atualizar versão para `1.0.0` quando aplicável.
- [ ] Criar changelog ou notas da release.
- [ ] Criar tag `v1.0.0`.
- [ ] Publicar release no GitHub.
- [ ] Confirmar artefatos, imagens e documentação acessíveis.
- [ ] Publicar a demonstração no canal escolhido.
- [ ] Criar o commit final `chore: prepara release v1.0.0` quando necessário.
- [ ] Atualizar `handoff.md` com estado `MVP concluído`.

---

## 14. Backlog avançado pós-MVP

### 14.1 Confiabilidade de mensageria

- [ ] Implementar Outbox Pattern.
- [ ] Configurar Dead Letter Queue.
- [ ] Implementar retry com backoff.
- [ ] Criar operação segura de reprocessamento.
- [ ] Criar evento `order.status-changed`.
- [ ] Criar consumidor adicional.

### 14.2 Segurança

- [ ] Implementar autenticação JWT.
- [ ] Implementar autorização por recurso/pedido.
- [ ] Impedir entrada na sala SignalR de outro cliente.
- [ ] Proteger endpoints administrativos.
- [ ] Usar gerenciador de segredos fora do laboratório.

### 14.3 Escala e observabilidade

- [ ] Adicionar Redis backplane ao SignalR.
- [ ] Testar múltiplas réplicas da API.
- [ ] Adicionar OpenTelemetry.
- [ ] Correlacionar traces entre API, RabbitMQ e Worker.
- [ ] Adicionar métricas de pedidos por status.
- [ ] Criar dashboards e alertas básicos.

### 14.4 Produto e infraestrutura

- [ ] Adicionar busca e paginação de produtos.
- [ ] Adicionar cupom de desconto.
- [ ] Adicionar upload de comprovante em object storage.
- [ ] Criar Helm Chart.
- [ ] Configurar Ingress e TLS.
- [ ] Avaliar PostgreSQL gerenciado e backups.
- [ ] Executar teste de carga da criação de pedidos.

---

## 15. Definição de projeto completo

- [ ] Todos os itens obrigatórios das seções 1 a 13 estão marcados.
- [ ] Todos os testes passam localmente e no CI.
- [ ] O ambiente completo inicia com um comando Docker Compose.
- [ ] A implantação Kubernetes executa o mesmo fluxo funcional.
- [ ] O chat é persistente e isolado por pedido.
- [ ] A mensageria possui ack manual e idempotência comprovada.
- [ ] Nenhum segredo real está versionado.
- [ ] README, diagrama, screenshots, vídeo e release estão publicados.
- [ ] O `handoff.md` registra a conclusão e as evoluções pendentes.
- [ ] É possível explicar por que cada componente existe, como falha e como observá-lo.
