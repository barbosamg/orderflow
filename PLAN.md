# Plano de execução - OrderFlow

## 1. Objetivo

Construir o OrderFlow como um projeto de estudo e portfólio em C#/.NET 10, demonstrando três formas complementares de comunicação:

- HTTP síncrono para operações da API REST;
- SignalR para chat e atualização de status em tempo real;
- RabbitMQ para processamento assíncrono por um Worker .NET.

O resultado deve executar localmente com Docker Compose, ser implantável em Kubernetes local, possuir testes automatizados, pipeline de integração contínua e documentação suficiente para demonstração pública.

## 2. Resultado esperado

Ao final do MVP será possível:

- cadastrar, consultar, atualizar e desativar produtos;
- cadastrar e consultar clientes;
- criar pedidos com um ou mais itens;
- calcular o total no backend e baixar o estoque de forma transacional;
- consultar pedidos e alterar seu status;
- trocar mensagens em uma sala SignalR isolada por pedido;
- persistir e recuperar o histórico do chat no PostgreSQL;
- publicar o evento `order.created` no RabbitMQ;
- consumir o evento no Worker com confirmação manual e idempotência;
- iniciar API, Worker, PostgreSQL e RabbitMQ com um único comando;
- executar a solução em Kubernetes local;
- comprovar build, testes e geração das imagens por GitHub Actions;
- apresentar o projeto por README, diagrama, screenshots e vídeo curto.

## 3. Escopo e decisões arquiteturais

### 3.1 Componentes

| Componente | Responsabilidade |
|---|---|
| `OrderFlow.Domain` | Entidades, enums e regras centrais sem dependências de infraestrutura |
| `OrderFlow.Application` | DTOs, contratos e casos de uso |
| `OrderFlow.Infrastructure` | EF Core, PostgreSQL, migrations e integração RabbitMQ |
| `OrderFlow.Api` | Controllers, OpenAPI, health check, SignalR e página de demonstração |
| `OrderFlow.Worker` | Consumo e processamento assíncrono de eventos |
| `OrderFlow.Tests` | Testes unitários e, posteriormente, testes de integração |

### 3.2 Fluxos principais

1. O cliente chama a API por HTTP.
2. A API valida as regras, persiste os dados no PostgreSQL e responde ao cliente.
3. Ao criar um pedido, a API publica `order.created` no RabbitMQ.
4. O Worker consome o evento, processa a tarefa e registra o `EventId` em `processed_events`.
5. Usuários conectados entram no grupo SignalR `order:{orderId}`.
6. Mensagens são persistidas no PostgreSQL antes do broadcast para o grupo.
7. Mudanças de status são enviadas ao grupo por `IHubContext<OrderChatHub>`.

### 3.3 Princípios obrigatórios

- PostgreSQL é a fonte durável da verdade.
- SignalR não armazena estado permanente.
- RabbitMQ não é utilizado como banco de dados.
- O domínio protege suas próprias invariantes.
- Controllers não alteram propriedades das entidades diretamente.
- Segredos reais não entram no Git.
- Logs incluem `OrderId` e `EventId`, mas nunca senhas, tokens ou connection strings.
- Cada etapa concluída deve gerar um commit pequeno, descritivo e executável.

### 3.4 Fora do MVP

Os itens abaixo ficam no backlog avançado e não devem bloquear a primeira versão funcional:

- autenticação e autorização com JWT;
- Outbox Pattern;
- Dead Letter Queue e reprocessamento;
- retry com backoff;
- Redis backplane para SignalR;
- OpenTelemetry;
- Helm, Ingress e TLS;
- banco PostgreSQL gerenciado;
- frontend completo em React, Angular ou Blazor.

## 4. Pré-requisitos e validação do ambiente

### Tarefas

- [x] Confirmar Windows 11 e PowerShell atualizados.
- [x] Validar WSL 2 com `wsl --status` e `wsl --update`.
- [x] Instalar ou validar .NET 10 SDK.
- [x] Instalar ou validar Git.
- [x] Instalar Docker Desktop com backend WSL 2.
- [x] Habilitar Kubernetes no Docker Desktop.
- [x] Instalar ou validar `kubectl`.
- [x] Escolher VS Code como editor principal.
- [ ] Instalar extensões úteis para C#, Docker, Kubernetes, REST e YAML.
- [x] Confirmar que nenhuma porta necessária está ocupada: 5432, 5672, 8080 e 15672.

### Evidências

```powershell
dotnet --info
dotnet --list-sdks
docker version
docker compose version
kubectl version --client
kubectl config current-context
kubectl get nodes
git --version
```

### Critério de saída

Todos os comandos respondem corretamente, o Docker apresenta Client e Server e existe pelo menos um nó Kubernetes em estado `Ready`.

### Commit sugerido

`chore: document development prerequisites`

## 5. Fases de implementação

### Fase 1 - Estrutura da solução (concluída em 2026-07-21)

#### Tarefas

- [x] Inicializar o repositório Git.
- [x] Criar `OrderFlow.slnx`, formato padrão do .NET 10.
- [x] Criar os projetos Domain, Application, Infrastructure, API, Worker e Tests.
- [x] Adicionar todos os projetos à solução.
- [x] Configurar referências respeitando os limites de cada camada.
- [x] Instalar EF Core/Npgsql, RabbitMQ.Client, OpenAPI nativo e ferramentas de migrations.
- [x] Criar `.gitignore` e validar restore/build/test inicial.
- [ ] Preparar diretórios `requests`, `k8s`, `docs` e `.github/workflows` quando forem necessários.

#### Comandos executados

```powershell
dotnet new sln -n OrderFlow

New-Item -ItemType Directory -Path .\src, .\tests

dotnet new classlib -n OrderFlow.Domain -o .\src\OrderFlow.Domain --framework net10.0 --no-restore
dotnet new classlib -n OrderFlow.Application -o .\src\OrderFlow.Application --framework net10.0 --no-restore
dotnet new classlib -n OrderFlow.Infrastructure -o .\src\OrderFlow.Infrastructure --framework net10.0 --no-restore
dotnet new webapi -n OrderFlow.Api -o .\src\OrderFlow.Api --use-controllers --framework net10.0 --no-restore
dotnet new worker -n OrderFlow.Worker -o .\src\OrderFlow.Worker --framework net10.0 --no-restore
dotnet new xunit -n OrderFlow.Tests -o .\tests\OrderFlow.Tests --framework net10.0 --no-restore
```

O uso de `--framework net10.0` fixou explicitamente o target inicial. O uso de `--no-restore` evitou restaurações repetidas durante o bootstrap; o restore completo foi executado depois da configuração dos projetos, referências e pacotes.

#### Validação

```powershell
dotnet restore
dotnet build
dotnet test
```

#### Critério de saída

A solução compila sem erros e `OrderFlow.Domain` não referencia ASP.NET Core, EF Core ou RabbitMQ.

Critério atendido em 2026-07-21: restore e build concluídos sem avisos, teste inicial aprovado e isolamento do projeto Domain confirmado.

#### Commits realizados

- `45ff89c` - `chore: estrutura solução inicial do OrderFlow em .NET 10`
- `371152c` - `chore: configure project references and dependencies`
- `b60410c` - `feat: adiciona status e regras de transição de pedidos`
- `8e81c7a` - `feat: adiciona entidade de produto e controle de estoque`

### Fase 2 - Domínio e regras de negócio

#### Tarefas

- [x] Criar `OrderStatus`: Created, Confirmed, Preparing, Shipped, Delivered e Cancelled.
- [x] Implementar `Product` com nome, descrição, preço, estoque, situação e data de criação.
- [x] Implementar validações de preço, estoque e quantidade.
- [x] Implementar exclusão lógica/ativação do produto por comportamento de domínio.
- [ ] Implementar `Customer` com normalização de nome e e-mail.
- [ ] Implementar `Order` e `OrderItem` com cálculo de total no domínio.
- [ ] Impedir alterações em pedidos entregues ou cancelados.
- [x] Definir e testar transições de status permitidas; não aceitar qualquer salto apenas porque o enum existe.
- [ ] Implementar `ChatMessage` com limite de 1.000 caracteres.
- [ ] Implementar `ProcessedEvent` para idempotência do Worker.

#### Testes mínimos

- [ ] Total calculado para múltiplos itens.
- [ ] Rejeição de preço, quantidade e estoque inválidos.
- [ ] Rejeição de retirada acima do estoque disponível.
- [ ] Rejeição de pedido sem cliente ou sem itens.
- [ ] Rejeição de alteração em pedido finalizado.
- [ ] Rejeição de mensagem vazia ou acima do limite.

#### Critério de saída

As regras centrais são executáveis e testáveis sem API, banco ou broker.

#### Commit sugerido

`feat: model products customers orders and chat domain`

### Fase 3 - Persistência com PostgreSQL e EF Core

#### Tarefas

- [ ] Criar `OrderFlowDbContext` e os `DbSet` necessários.
- [ ] Criar configurações EF para Product, Customer, Order, OrderItem, ChatMessage e ProcessedEvent.
- [ ] Definir tamanhos, precisão decimal, conversão de status e comportamentos de exclusão.
- [ ] Configurar relacionamentos entre cliente, pedido e itens.
- [ ] Usar acesso por campo para a coleção privada de itens do pedido.
- [ ] Criar índices para produtos, histórico de pedidos e histórico do chat.
- [ ] Criar índice único de `ProcessedEvent.EventId`.
- [ ] Criar extensão de injeção de dependência da infraestrutura.
- [ ] Configurar connection string somente para laboratório local.
- [ ] Criar e revisar a migration `InitialCreate`.
- [ ] Aplicar a migration e inspecionar o schema gerado.

#### Índices obrigatórios

| Tabela | Índice |
|---|---|
| `products` | `name` |
| `orders` | `customer_id, created_at` |
| `chat_messages` | `order_id, sent_at` |
| `processed_events` | `event_id` único |

#### Critério de saída

O schema é criado por migration, os relacionamentos funcionam e o banco rejeita `EventId` duplicado.

#### Commit sugerido

`feat: add PostgreSQL persistence and initial migration`

### Fase 4 - API REST e casos de uso

#### Tarefas

- [ ] Criar DTOs de entrada e saída sem expor entidades diretamente.
- [ ] Implementar CRUD de produtos, incluindo `PUT` e desativação lógica no `DELETE`.
- [ ] Implementar criação e listagem de clientes.
- [ ] Implementar consulta de cliente por ID se necessária para a interface.
- [ ] Implementar `OrderService` para criação transacional do pedido.
- [ ] Validar cliente, produtos ativos, quantidade e estoque antes do commit.
- [ ] Consolidar itens repetidos ou rejeitá-los explicitamente para evitar baixa incorreta de estoque.
- [ ] Implementar criação, detalhamento e alteração de status do pedido.
- [ ] Implementar histórico de mensagens por HTTP.
- [ ] Configurar Controllers, OpenAPI nativo, arquivos estáticos e health check.
- [ ] Criar coleção `.http` com o fluxo completo e dados encadeados.

#### Endpoints previstos

| Método | Rota | Resultado esperado |
|---|---|---|
| GET | `/api/products` | Lista produtos |
| POST | `/api/products` | Cria produto |
| GET | `/api/products/{id}` | Detalha produto |
| PUT | `/api/products/{id}` | Atualiza produto |
| DELETE | `/api/products/{id}` | Desativa produto |
| GET | `/api/customers` | Lista clientes |
| POST | `/api/customers` | Cria cliente |
| POST | `/api/orders` | Cria pedido |
| GET | `/api/orders/{id}` | Retorna pedido e itens |
| PATCH | `/api/orders/{id}/status` | Altera status |
| GET | `/api/orders/{id}/messages` | Retorna histórico do chat |

#### Critério de saída

O fluxo produto -> cliente -> pedido pode ser executado pelo arquivo `.http`, com contrato publicado em OpenAPI e total e estoque corretos no banco.

#### Commit sugerido

`feat: implement product customer and order API`

### Fase 5 - RabbitMQ e Worker

#### Tarefas

- [ ] Criar o contrato `OrderCreatedEvent` com `EventId`, `OrderId`, `CustomerId`, total e data.
- [ ] Criar `IEventPublisher` e `RabbitMqOptions`.
- [ ] Implementar conexão e canal duradouros no publicador.
- [ ] Declarar exchange topic `orderflow.events`.
- [ ] Publicar eventos persistentes com routing key `order.created`.
- [ ] Proteger uso concorrente do canal do publicador.
- [ ] Registrar publicador como singleton e descartá-lo corretamente.
- [ ] Implementar Worker com exchange, fila durable e binding.
- [ ] Configurar `BasicQos` e ack manual.
- [ ] Registrar `ProcessedEvent` antes do ack.
- [ ] Confirmar evento repetido sem repetir o efeito.
- [ ] Tratar falha com nack sem requeue infinito.
- [ ] Documentar a limitação do MVP: commit no banco e publicação não são atômicos.

#### Critério de saída

Criar um pedido publica um evento, o Worker o processa uma única vez e somente envia ack após persistir o resultado.

#### Commit sugerido

`feat: publish and consume order created events`

### Fase 6 - SignalR e interface de demonstração

#### Tarefas

- [ ] Criar `OrderChatHub`.
- [ ] Validar a existência do pedido antes de entrar no grupo ou enviar mensagem.
- [ ] Implementar entrada e saída do grupo `order:{orderId}`.
- [ ] Persistir mensagens antes de emitir `MessageReceived`.
- [ ] Usar `IHubContext` no endpoint de status para emitir `StatusChanged`.
- [ ] Criar página HTML responsiva de demonstração.
- [ ] Carregar o histórico HTTP ao conectar; essa integração não está completa no exemplo da apostila.
- [ ] Tratar reconexão, estado dos botões e mensagens de erro no frontend.
- [ ] Testar isolamento entre pedidos diferentes.
- [ ] Registrar como requisito futuro a autorização de acesso à sala.

#### Cenários de validação

1. Duas janelas entram no mesmo pedido e conversam nos dois sentidos.
2. Uma terceira janela em outro pedido não recebe as mensagens.
3. A alteração de status aparece em tempo real nas duas janelas corretas.
4. Após atualizar a página, o histórico é recarregado do PostgreSQL.
5. Pedido inexistente e mensagem inválida retornam erro controlado.

#### Critério de saída

Chat, histórico e atualização de status funcionam sem vazamento entre salas.

#### Commit sugerido

`feat: add per-order realtime chat and status updates`

### Fase 7 - Docker Compose

#### Tarefas

- [ ] Criar Dockerfiles multi-stage para API e Worker.
- [ ] Criar `.dockerignore`.
- [ ] Criar `docker-compose.yml` com PostgreSQL, RabbitMQ, API e Worker.
- [ ] Configurar volumes persistentes para PostgreSQL e RabbitMQ.
- [ ] Configurar health checks dos serviços de infraestrutura.
- [ ] Usar nomes de serviço `postgres` e `rabbitmq` no DNS interno.
- [ ] Injetar configurações por variáveis de ambiente.
- [ ] Aplicar migrations automaticamente apenas no laboratório.
- [ ] Validar rebuild completo sem depender de artefatos locais.
- [ ] Documentar que `docker compose down -v` remove os dados.

#### Validação

```powershell
docker compose build
docker compose up -d
docker compose ps
docker compose logs --tail 100 api
docker compose logs --tail 100 worker
docker compose exec rabbitmq rabbitmqctl list_queues name messages consumers
```

#### Critério de saída

`docker compose up -d` inicia todo o ambiente saudável e o fluxo funcional completo opera em `http://localhost:8080`.

#### Commit sugerido

`feat: run OrderFlow stack with Docker Compose`

### Fase 8 - Qualidade, erros e testes de integração

#### Tarefas

- [ ] Implementar Problem Details global para erros conhecidos.
- [ ] Mapear corretamente 400, 404, 409 e 500.
- [ ] Padronizar logs estruturados com identificadores de correlação.
- [ ] Garantir ausência de segredos e dados pessoais nos logs.
- [ ] Executar testes unitários com cobertura.
- [ ] Adicionar testes de integração com PostgreSQL e RabbitMQ reais, preferencialmente via Testcontainers.
- [ ] Testar criação de pedido e persistência transacional.
- [ ] Testar publicação, consumo, ack e idempotência.
- [ ] Testar SignalR com dois clientes e isolamento entre grupos.
- [ ] Testar endpoint `/health`.
- [ ] Executar análise de dependências vulneráveis.

#### Validação

```powershell
dotnet build --configuration Release
dotnet test --configuration Release
dotnet test --collect:"XPlat Code Coverage"
dotnet list package --vulnerable --include-transitive
```

#### Critério de saída

Build e testes passam, erros conhecidos usam Problem Details e os logs permitem acompanhar pedido e evento sem expor credenciais.

#### Commit sugerido

`test: cover domain API messaging and realtime flows`

### Fase 9 - Kubernetes local

#### Tarefas

- [ ] Publicar imagens versionadas da API e Worker no GHCR.
- [ ] Criar namespace `orderflow`.
- [ ] Criar ConfigMap para configurações não sensíveis.
- [ ] Criar Secret para credenciais do laboratório.
- [ ] Evitar senha literal na connection string dos Deployments.
- [ ] Implantar PostgreSQL com PVC, readiness probe e limites de recursos.
- [ ] Implantar RabbitMQ com readiness probe e Service.
- [ ] Implantar API com probes HTTP e uma réplica.
- [ ] Implantar Worker e validar múltiplos consumidores.
- [ ] Aplicar manifests em ordem e acompanhar rollouts.
- [ ] Validar acesso por port-forward.
- [ ] Excluir um Pod da API e comprovar autorrecuperação.
- [ ] Escalar Worker para três réplicas e comprovar distribuição das mensagens.

#### Validação

```powershell
kubectl apply -f k8s/
kubectl get all -n orderflow
kubectl get pvc -n orderflow
kubectl rollout status deployment/orderflow-api -n orderflow
kubectl logs deployment/orderflow-api -n orderflow
kubectl logs deployment/orderflow-worker -n orderflow
kubectl port-forward service/orderflow-api 8080:8080 -n orderflow
```

#### Critério de saída

Pods ficam `Running` e `Ready`, PVC fica `Bound`, a API responde por port-forward e pedidos são processados pelo Worker.

#### Restrição consciente

Manter uma réplica da API no MVP. Para escalar SignalR, planejar sticky sessions e Redis backplane ou serviço gerenciado.

#### Commit sugerido

`feat: deploy OrderFlow to local Kubernetes`

### Fase 10 - CI, documentação e portfólio

#### Tarefas

- [ ] Criar workflow de GitHub Actions para restore, build e test em Pull Requests.
- [ ] Publicar imagens de API e Worker no GHCR após push na `main`.
- [ ] Usar tags imutáveis por versão ou SHA além de `latest`.
- [ ] Criar README com objetivo, arquitetura, tecnologias e decisões.
- [ ] Documentar execução com Docker Compose e Kubernetes.
- [ ] Incluir diagrama dos fluxos HTTP, SignalR e RabbitMQ.
- [ ] Capturar screenshots sem informações pessoais ou segredos.
- [ ] Gravar vídeo de 60 a 90 segundos mostrando o fluxo completo.
- [ ] Criar licença e release `v1.0.0`.
- [ ] Preparar publicação explicando decisões e aprendizados concretos.

#### Evidências esperadas

- pipeline verde;
- imagens disponíveis no GHCR;
- evidência do documento OpenAPI ou da interface de exploração adotada;
- duas janelas no mesmo chat;
- mudança de status em tempo real;
- exchange, fila e consumidor no RabbitMQ Management;
- containers em execução;
- Pods, Services e Deployments no Kubernetes;
- vídeo curto e README navegável.

#### Critério de saída

Uma pessoa externa consegue clonar o repositório, iniciar o ambiente pelas instruções e compreender por que cada componente existe.

#### Commits sugeridos

- `ci: build test and publish container images`
- `docs: add architecture setup and project demo`
- `chore: prepare v1.0.0 release`

## 6. Matriz de aceitação do MVP

| Cenário | Evidência | Status inicial |
|---|---|---|
| CRUD de produtos | Requisições e registros no PostgreSQL | Pendente |
| Cadastro de clientes | Requisições e registros no PostgreSQL | Pendente |
| Pedido com múltiplos itens | Total e estoque conferidos | Pendente |
| Transação de pedido | Falha não deixa estoque parcialmente baixado | Pendente |
| Evento assíncrono | Exchange/fila e log do Worker | Pendente |
| Idempotência | Mesmo `EventId` não duplica efeito | Pendente |
| Ack após sucesso | Mensagem só é removida após persistência | Pendente |
| Chat na mesma sala | Duas janelas recebem mensagens | Pendente |
| Isolamento de salas | Outro pedido não recebe mensagens | Pendente |
| Histórico persistente | Reload recupera mensagens por HTTP | Pendente |
| Status em tempo real | Duas janelas recebem `StatusChanged` | Pendente |
| Docker Compose | Todos os serviços saudáveis | Pendente |
| Testes | Suite verde e cobertura coletada | Pendente |
| Kubernetes | Pods Ready, PVC Bound e fluxo funcional | Pendente |
| CI | Workflow verde e imagens publicadas | Pendente |
| Portfólio | README, diagrama, screenshots, vídeo e release | Pendente |

## 7. Riscos e medidas preventivas

| Risco | Medida preventiva |
|---|---|
| Banco confirma e publicação RabbitMQ falha | Registrar limitação do MVP e priorizar Outbox após a versão funcional |
| Mensagem inválida entra em repetição infinita | Usar nack sem requeue e evoluir para DLQ |
| Evento é entregue mais de uma vez | `EventId` único e processamento idempotente |
| Concorrência permite estoque negativo | Transação, regra de domínio e estratégia explícita de concorrência no banco |
| Mensagem vaza entre pedidos | Grupo sempre derivado de `OrderId` e teste de isolamento |
| Usuário acessa sala de outro pedido | Não publicar externamente antes de JWT e autorização por pedido |
| API SignalR perde conexões ao escalar | Uma réplica no MVP; Redis backplane/sticky sessions na evolução |
| Segredo aparece em manifest ou log | Secret/variáveis, varredura antes do commit e revisão do pipeline |
| Dados desaparecem no laboratório | Volumes/PVC e cuidado com `down -v` ou remoção do namespace |
| Imagem não inicia no cluster | Tags existentes, visibilidade do GHCR, probes e inspeção de eventos |
| Versões da apostila ficam defasadas | Verificar documentação oficial antes de instalar SDK, pacotes e imagens |

## 8. Backlog avançado priorizado

Executar somente após o MVP estável:

1. Outbox Pattern para consistência entre PostgreSQL e RabbitMQ.
2. Dead Letter Queue, retry com backoff e processo seguro de reenvio.
3. JWT, autorização por pedido e proteção do Hub SignalR.
4. Testcontainers para integração reproduzível.
5. OpenTelemetry para rastrear API -> RabbitMQ -> Worker.
6. Redis backplane para múltiplas réplicas da API.
7. Busca e paginação de produtos.
8. Cupom de desconto com validade e regras de aplicação.
9. Evento `order.status-changed` e consumidor adicional.
10. Métricas de pedidos por status.
11. Upload de comprovante em armazenamento de objetos.
12. Helm Chart, Ingress e TLS.
13. Teste de carga da criação de pedidos.
14. PostgreSQL gerenciado e estratégia de backup.

## 9. Cronograma sugerido de quatro semanas

| Semana | Foco | Entrega verificável |
|---|---|---|
| 1 | Ambiente, solução, domínio, EF Core e PostgreSQL | Build verde, migration e CRUD inicial |
| 2 | Pedidos, estoque, RabbitMQ e Worker | Pedido transacional e evento idempotente processado |
| 3 | SignalR, interface e Docker Compose | Demonstração local completa em duas janelas |
| 4 | Testes, Kubernetes, CI e documentação | Release pública com pipeline e evidências |

## 10. Estratégia de execução e commits

- Trabalhar em uma fase por vez.
- Antes de cada mudança, registrar ou escolher o cenário de aceitação correspondente.
- Ao terminar a fase, executar build, testes e validações específicas.
- Não avançar com falhas conhecidas sem registrá-las no plano.
- Fazer commits pequenos usando `chore`, `feat`, `fix`, `test`, `docs` e `ci`.
- Atualizar os checkboxes deste documento conforme as evidências forem produzidas.
- Criar tag/release somente quando toda a matriz do MVP estiver atendida.

## 11. Definição de pronto

O projeto estará concluído quando:

- [ ] todos os itens da matriz de aceitação do MVP estiverem comprovados;
- [ ] `dotnet build` e `dotnet test` passarem em máquina limpa e no CI;
- [ ] Docker Compose iniciar o ambiente completo com um comando;
- [ ] o ambiente Kubernetes local executar o mesmo fluxo funcional;
- [ ] não houver segredo real versionado;
- [ ] o repositório contiver documentação e evidências de demonstração;
- [ ] for possível explicar, sem consultar o código, a responsabilidade de cada componente, seu comportamento em caso de falha e como observá-lo.

## 12. Próxima ação

Continuar a **Fase 2 - Domínio e regras de negócio** concluindo `Product` com alteração controlada de nome, descrição e preço, seguida dos respectivos testes unitários; depois, iniciar `Customer`.
