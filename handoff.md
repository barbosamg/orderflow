# Handoff - OrderFlow

Registro vivo para retomar o projeto sem perder contexto. Atualizar este arquivo ao final de cada sessão ou antes de trocar de agente/desenvolvedor.

## Estado atual

| Campo | Valor |
|---|---|
| Data da última atualização | 2026-07-20 |
| Fase atual | Planejamento concluído; preparação do ambiente ainda não iniciada |
| Último item concluído | Criação do `TODO.md` e deste `handoff.md` |
| Próximo item | `TODO.md` - seção 1.1, validar Windows, PowerShell e WSL 2 |
| Bloqueios | Nenhum bloqueio conhecido |
| Status do MVP | Não iniciado |

## Último trabalho realizado

- A apostila `Apostila_OrderFlow_DotNet_Docker_Kubernetes_PostgreSQL_SignalR_RabbitMQ.pdf` foi lida integralmente, totalizando 37 páginas.
- O planejamento técnico completo foi consolidado em `PLAN.md`.
- O `TODO.md` foi criado com o checklist desde a preparação do ambiente até a release `v1.0.0` e o backlog pós-MVP.
- Este arquivo foi criado para registrar continuamente o estado, evidências, decisões e próxima ação.
- O Poppler 26.02.0 foi movido para `C:\Users\mateu\.local\poppler\26.02.0\Library\bin` e registrado no PATH do usuário.

## Arquivos existentes na raiz

- `Apostila_OrderFlow_DotNet_Docker_Kubernetes_PostgreSQL_SignalR_RabbitMQ.pdf` - fonte didática original.
- `PLAN.md` - planejamento, arquitetura, critérios de aceite, riscos e cronograma.
- `TODO.md` - checklist operacional que deve ser marcado durante a implementação.
- `handoff.md` - contexto de continuidade e estado mais recente.

Ainda não existem solução .NET, projetos em `src`, testes, repositório Git inicializado ou infraestrutura da aplicação.

## Decisões já tomadas

- Stack-base: C#/.NET 10, ASP.NET Core, EF Core, PostgreSQL, SignalR, RabbitMQ, Docker Compose e Kubernetes.
- Arquitetura sem exagero: Domain, Application, Infrastructure, API, Worker e Tests.
- PostgreSQL será a fonte durável da verdade.
- SignalR será usado somente para entrega em tempo real; o histórico ficará no PostgreSQL.
- RabbitMQ será usado para trabalho assíncrono; não substituirá persistência.
- O Worker usará ack manual e idempotência por `EventId` único.
- O MVP publicará o evento depois do commit no banco; Outbox fica no backlog avançado.
- A API ficará com uma réplica no Kubernetes enquanto não houver Redis backplane/sticky sessions.
- Produtos serão desativados logicamente em vez de removidos quando puderem ser referenciados por pedidos.
- Segredos reais não serão commitados.
- Cada entrega funcional deve terminar com validação, atualização do TODO/handoff e commit pequeno.

## Validações já realizadas

- PDF aberto e validado com 37 páginas, sem criptografia.
- Texto extraído de todas as páginas; nenhuma página sem conteúdo textual.
- Amostras visuais da primeira, intermediária e última página conferidas.
- Poppler 26.02.0 executou `pdfinfo`, `pdftotext` e `pdftoppm` corretamente.
- `PLAN.md` criado e revisado estruturalmente.
- `TODO.md` e `handoff.md` criados em 2026-07-20.

## Próxima ação exata

Abrir um novo PowerShell e executar, na ordem:

```powershell
winver
$PSVersionTable
wsl --status
wsl --update
git --version
dotnet --info
dotnet --list-sdks
dotnet --version
docker version
docker compose version
kubectl version --client
kubectl config current-context
kubectl get nodes
pdfinfo -v
```

Depois:

1. registrar as versões e eventuais erros neste arquivo;
2. marcar os itens comprovados nas seções 1.1 a 1.4 do `TODO.md`;
3. verificar as portas 5432, 5672, 8080 e 15672;
4. somente então iniciar a seção 2, criando o repositório e a solução.

## Pendências e riscos imediatos

- Confirmar se .NET 10 SDK está instalado e compatível com os pacotes atuais.
- Confirmar se Docker Desktop está iniciado e integrado ao WSL 2.
- Confirmar se Kubernetes está habilitado e com nó `Ready`.
- Confirmar as portas locais antes de criar o Compose.
- Verificar documentação oficial no momento de instalar versões, pois a apostila foi preparada em julho de 2026.

## Histórico resumido

| Data | Entrega | Resultado |
|---|---|---|
| 2026-07-20 | Leitura da apostila | 37 páginas lidas e conteúdo validado |
| 2026-07-20 | `PLAN.md` | Planejamento completo criado |
| 2026-07-20 | Organização do Poppler | Movido para fora do projeto e adicionado ao PATH do usuário |
| 2026-07-20 | `TODO.md` | Checklist integral criado |
| 2026-07-20 | `handoff.md` | Registro de continuidade criado |

## Modelo para a próxima atualização

Ao encerrar a próxima sessão, atualizar pelo menos:

```markdown
## Estado atual
- Data/hora:
- Fase e subseção do TODO:
- Último checkbox concluído:
- Próximo checkbox:
- Commit mais recente:
- Bloqueios:

## Evidências da sessão
- Comandos executados:
- Testes executados:
- Resultados:
- Arquivos criados ou alterados:
- Decisões tomadas:
```
