# Handoff - OrderFlow

Registro vivo para retomar o projeto sem perder contexto. Atualizar este arquivo ao final de cada sessão ou antes de trocar de agente/desenvolvedor.

## Estado atual

| Campo | Valor |
|---|---|
| Data da última atualização | 2026-07-20 |
| Fase atual | Inicialização da solução em andamento |
| Último item concluído | Inclusão dos seis projetos no `OrderFlow.slnx` e preparação do commit de bootstrap |
| Próximo item | Configurar as referências entre Domain, Application, Infrastructure, API, Worker e Tests |
| Bloqueios | Nenhum bloqueio conhecido; Poppler é opcional e não participa da aplicação |
| Status do MVP | Não iniciado |

## Último trabalho realizado

- A apostila `Apostila_OrderFlow_DotNet_Docker_Kubernetes_PostgreSQL_SignalR_RabbitMQ.pdf` foi lida integralmente, totalizando 37 páginas.
- O planejamento técnico completo foi consolidado em `PLAN.md`.
- O `TODO.md` foi criado com o checklist desde a preparação do ambiente até a release `v1.0.0` e o backlog pós-MVP.
- Este arquivo foi criado para registrar continuamente o estado, evidências, decisões e próxima ação.
- O Poppler 26.02.0 foi movido para `C:\Users\mateu\.local\poppler\26.02.0\Library\bin` e registrado no PATH do usuário.
- O `README.md` foi reformulado como vitrine do projeto, priorizando core funcional, arquitetura, tecnologias, domínio, endpoints, decisões técnicas e diferenciais.

## Arquivos existentes na raiz

- `Apostila_OrderFlow_DotNet_Docker_Kubernetes_PostgreSQL_SignalR_RabbitMQ.pdf` - fonte didática original.
- `PLAN.md` - planejamento, arquitetura, critérios de aceite, riscos e cronograma.
- `TODO.md` - checklist operacional que deve ser marcado durante a implementação.
- `handoff.md` - contexto de continuidade e estado mais recente.
- `README.md` - apresentação pública e documentação principal do repositório.
- `.gitignore` - regras de exclusão versionadas no repositório.
- `OrderFlow.slnx` - solução .NET 10 criada, ainda sem projetos.

O repositório Git está inicializado na branch `main`, com remoto `https://github.com/barbosamg/orderflow.git` e commit mais recente `05800c2 add readme`. A solução e os seis projetos-base existem, mas ainda não foram adicionados ao `OrderFlow.slnx`, referenciados entre si ou restaurados.

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
- A solução usará `OrderFlow.slnx`, formato padrão e moderno do .NET 10, em vez do `.sln` legado.

## Validações já realizadas

- PDF aberto e validado com 37 páginas, sem criptografia.
- Texto extraído de todas as páginas; nenhuma página sem conteúdo textual.
- Amostras visuais da primeira, intermediária e última página conferidas.
- Poppler 26.02.0 executou `pdfinfo`, `pdftotext` e `pdftoppm` corretamente.
- `PLAN.md` criado e revisado estruturalmente.
- `TODO.md` e `handoff.md` criados em 2026-07-20.
- `README.md` criado e conferido contra o estado real do repositório em 2026-07-20.
- PowerShell 7.6.3 validado.
- WSL com versão padrão 2 e distribuição padrão `docker-desktop` validado.
- Git 2.55.0.windows.3 validado.
- SDK .NET principal 10.0.300 validado; SDKs 2.2.207, 8.0.423, 9.0.316, 10.0.201 e 10.0.300 instalados.
- Docker Compose v2.28.1-desktop.1 validado.
- Cliente kubectl v1.29.2 validado.
- Na primeira tentativa, `docker version` encontrou o cliente 27.0.3, mas não conectou ao pipe `dockerDesktopLinuxEngine`; o problema foi resolvido ao iniciar o Docker Desktop.
- Na primeira tentativa, o Kubernetes não possuía `current-context`; o problema foi resolvido após habilitar o cluster local.
- Docker Desktop 4.32.0 iniciado com Engine 27.0.3 para Linux/amd64.
- `docker version` validado com Client e Server acessíveis no contexto `desktop-linux`.
- Contexto Kubernetes `docker-desktop` configurado.
- Nó `docker-desktop` validado em estado `Ready`, control-plane, Kubernetes v1.29.2.
- Durante a verificação seguinte, o Docker Desktop informou que sua distro WSL foi encerrada abruptamente (`running wsl-bootstrap: exit status 1`), possivelmente durante o reinício de componentes provocado por `wsl --update`.
- Recuperação executada com Docker Desktop fechado, `wsl --shutdown` e reinício limpo.
- Distribuição padrão alterada de `docker-desktop` para `Ubuntu`; ambas permanecem em WSL 2.
- Engine Docker recuperado com Client e Server 27.0.3 acessíveis.
- Contexto `docker-desktop` preservado; a primeira chamada ao cluster após o reinício retornou `EOF`, indicando que a API ainda não estava pronta.
- Nova verificação confirmou Docker Client e Server funcionando, mas `kubectl cluster-info`, `kubectl get nodes` e `kubectl get pods --all-namespaces` continuaram retornando `EOF`.
- Foi decidido reiniciar o Windows antes de realizar qualquer reset, remoção de distribuição ou alteração destrutiva no cluster.
- Após reiniciar o Windows, a integração opcional do Docker com Ubuntu falhou ao executar `docker-desktop-user-distro` com `Exec format error`.
- Como o fluxo do projeto usa PowerShell, foi decidido desabilitar/ignorar temporariamente apenas a integração com Ubuntu, preservando o backend WSL 2 do Docker Desktop.
- Integração opcional Docker-WSL com Ubuntu desabilitada nas configurações do Docker Desktop.
- Docker Desktop atualizado de 4.32.0 para 4.82.0; Engine atualizado de 27.0.3 para 29.6.1.
- Kubernetes atualizado de v1.29.2 para v1.36.1.
- Control plane acessível em `https://127.0.0.1:65106`.
- Nó `desktop-control-plane` validado em estado `Ready`.
- Windows 11 25H2 confirmado pelo build 26200.8894; `Get-ComputerInfo` mostrou o rótulo legado `Windows 10 Home Single Language`.
- .NET SDK 10.0.300, MSBuild 18.6.3 e runtime .NET 10.0.8 validados em Windows x64.
- Git configurado com usuário `Mateus Barbosa` e e-mail GitHub noreply.
- VS Code 1.129.1 x64 validado.
- Portas 5432, 5672, 8080 e 15672 sem processos em estado `Listen`.
- Poppler permanece instalado em `C:\Users\mateu\.local\poppler\26.02.0\Library\bin` e registrado no PATH persistente do usuário, mas `pdfinfo` não foi localizado pela sessão PowerShell atual.
- `OrderFlow.slnx` criado com 25 bytes e conteúdo XML válido `<Solution></Solution>`.
- `dotnet sln .\OrderFlow.slnx list` confirmou que a solução ainda não possui projetos.
- Diretórios `src` e `tests` criados e validados na raiz do repositório.
- Projetos Domain, Application, Infrastructure, API, Worker e Tests gerados com target `net10.0` e opção `--no-restore`.
- Seis arquivos `.csproj` conferidos; API usa `Microsoft.NET.Sdk.Web`, Worker usa `Microsoft.NET.Sdk.Worker` e os demais usam `Microsoft.NET.Sdk`.
- Template da API incluiu `Microsoft.AspNetCore.OpenApi` 10.0.8; Worker incluiu `Microsoft.Extensions.Hosting` 10.0.8; Tests incluiu xUnit, Microsoft.NET.Test.Sdk e coverlet.
- `OrderFlow.slnx` organizado com pasta lógica `/src/` contendo cinco projetos e `/tests/` contendo `OrderFlow.Tests`.
- `dotnet sln .\OrderFlow.slnx list` conferido com exatamente seis projetos.
- Bootstrap preparado para commit com 27 arquivos antes da atualização final deste handoff; referências entre projetos, restore, build e testes ainda não foram executados.

## Próxima ação exata

Na próxima sessão, configurar as referências permitidas entre os projetos:

```powershell
dotnet add .\src\OrderFlow.Application\OrderFlow.Application.csproj reference .\src\OrderFlow.Domain\OrderFlow.Domain.csproj
dotnet add .\src\OrderFlow.Infrastructure\OrderFlow.Infrastructure.csproj reference .\src\OrderFlow.Domain\OrderFlow.Domain.csproj .\src\OrderFlow.Application\OrderFlow.Application.csproj
dotnet add .\src\OrderFlow.Api\OrderFlow.Api.csproj reference .\src\OrderFlow.Application\OrderFlow.Application.csproj .\src\OrderFlow.Infrastructure\OrderFlow.Infrastructure.csproj
dotnet add .\src\OrderFlow.Worker\OrderFlow.Worker.csproj reference .\src\OrderFlow.Application\OrderFlow.Application.csproj .\src\OrderFlow.Infrastructure\OrderFlow.Infrastructure.csproj
dotnet add .\tests\OrderFlow.Tests\OrderFlow.Tests.csproj reference .\src\OrderFlow.Domain\OrderFlow.Domain.csproj .\src\OrderFlow.Application\OrderFlow.Application.csproj
```

Depois, conferir os `ProjectReference`, executar um único restore e validar build/test.

## Pendências e riscos imediatos

- Revisar a compatibilidade das versões dos pacotes NuGet quando eles forem adicionados.
- Validar novamente as portas antes de iniciar o Docker Compose.
- Verificar documentação oficial no momento de instalar versões, pois a apostila foi preparada em julho de 2026.

## Histórico resumido

| Data | Entrega | Resultado |
|---|---|---|
| 2026-07-20 | Leitura da apostila | 37 páginas lidas e conteúdo validado |
| 2026-07-20 | `PLAN.md` | Planejamento completo criado |
| 2026-07-20 | Organização do Poppler | Movido para fora do projeto e adicionado ao PATH do usuário |
| 2026-07-20 | `TODO.md` | Checklist integral criado |
| 2026-07-20 | `handoff.md` | Registro de continuidade criado |
| 2026-07-20 | `README.md` | Vitrine técnica criada sem declarar o MVP como implementado |
| 2026-07-20 | Diagnóstico Docker/Kubernetes | Docker saudável; Kubernetes persistiu com `EOF`; reinicialização do Windows definida como próximo teste |
| 2026-07-20 | Recuperação Docker/Kubernetes | Integração Ubuntu desabilitada; Docker Desktop 4.82.0 e Kubernetes 1.36.1 validados com nó `Ready` |
| 2026-07-20 | `OrderFlow.slnx` | Solução .NET 10 criada e validada sem projetos |
| 2026-07-20 | Diretórios-base | `src` e `tests` criados e validados |
| 2026-07-20 | Projetos-base | Seis projetos `net10.0` gerados sem restore individual |
| 2026-07-20 | Solução organizada | Seis projetos adicionados ao `OrderFlow.slnx`; referências ficam para a próxima sessão |

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
