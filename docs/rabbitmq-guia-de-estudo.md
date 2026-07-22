# RabbitMQ no OrderFlow - guia de estudo

## 1. Objetivo

Este documento explica como o RabbitMQ participa do OrderFlow e quais garantias cada configuração oferece. O fluxo estudado é:

```text
OrderFlow.Api
    |
    | publica order.created
    v
exchange topic: orderflow.events
    |
    | binding order.created
    v
fila: orderflow.worker.order-created
    |
    | entrega com ack manual
    v
OrderFlow.Worker
    |
    v
processed_events no PostgreSQL
```

A API continua responsável pela transação do pedido. O RabbitMQ transporta uma notificação assíncrona para o Worker executar processamento posterior sem aumentar o tempo da operação HTTP.

## 2. Conceitos fundamentais

### Publisher

É a aplicação que publica uma mensagem. No OrderFlow, a API é o publisher de `OrderCreatedEvent`.

O publisher não publica diretamente em uma fila. No AMQP 0-9-1, ele publica em uma **exchange**, informando uma routing key.

### Exchange

A exchange recebe mensagens dos publishers e decide para quais filas encaminhá-las. Ela funciona como uma tabela de roteamento e não como armazenamento.

O OrderFlow usará:

```text
Nome: orderflow.events
Tipo: topic
Durable: true
Auto-delete: false
```

Uma exchange durável permanece declarada depois da reinicialização do broker. `Auto-delete: false` evita sua remoção automática quando bindings deixam de existir.

### Routing key

É o identificador usado pela exchange para decidir o destino da mensagem. O primeiro evento será publicado com:

```text
order.created
```

A separação por pontos é especialmente útil em exchanges `topic`, pois permite bindings exatos ou com curingas.

### Binding

É a regra que conecta uma exchange a uma fila. Exemplo:

```text
exchange: orderflow.events
fila: orderflow.worker.order-created
binding key: order.created
```

Sem binding compatível, a exchange recebe a mensagem, mas não encontra uma fila de destino. Se a publicação não usar a opção `mandatory`, a mensagem não roteável pode ser descartada.

### Queue

A fila armazena mensagens até que um consumidor possa recebê-las. Em condições normais ela segue FIFO, mas prioridades, múltiplos consumidores e redelivery podem alterar a ordem efetivamente observada.

### Consumer

É a aplicação que recebe mensagens da fila. No OrderFlow, será o projeto `OrderFlow.Worker`.

## 3. Tipos de exchange

| Tipo | Regra de roteamento | Exemplo de uso |
|---|---|---|
| `direct` | Correspondência exata da routing key | Uma fila específica para `order.created` |
| `topic` | Padrões separados por pontos | Eventos `order.created`, `order.cancelled` e `order.*` |
| `fanout` | Envia uma cópia para todas as filas vinculadas | Broadcast sem considerar routing key |
| `headers` | Usa cabeçalhos em vez da routing key | Regras baseadas em múltiplos metadados |
| default | Exchange direct interna com nome vazio | Publicação simplificada usando o nome da fila |

### Por que `topic` no OrderFlow

O MVP começa com `order.created`, mas poderá receber eventos como `order.status-changed` e `order.cancelled`. Uma exchange `topic` permite que consumidores diferentes usem bindings como:

```text
order.created
order.status-changed
order.*
#
```

Em bindings de topic, `*` representa exatamente um segmento e `#` representa zero ou mais segmentos.

## 4. Tipos de fila

### Classic queue

É a fila tradicional. Pode ser durável ou temporária e, nas versões modernas do RabbitMQ, não oferece replicação entre nós.

Uso adequado:

- laboratório local com um único broker;
- cargas simples;
- filas temporárias ou exclusivas;
- situações nas quais replicação não é necessária.

Decisão inicial do OrderFlow: usar uma classic queue durável no Docker Compose de um nó. Isso facilita o estudo sem sugerir alta disponibilidade inexistente.

### Quorum queue

É uma fila durável e replicada baseada no algoritmo Raft. Foi projetada para segurança de dados e eleição previsível de líder em clusters.

Uso adequado:

- produção com cluster de três ou mais nós;
- mensagens importantes que precisam sobreviver à perda de um nó;
- cenários que exigem poison message handling e dead-lettering mais robusto.

É declarada com o argumento:

```text
x-queue-type = quorum
```

Em um broker de apenas um nó, uma quorum queue não fornece alta disponibilidade real, pois não existem outras réplicas para assumir a liderança.

### Stream

É um log imutável e persistente no qual consumidores acompanham offsets. É indicado para retenção longa, replay e grandes volumes de eventos.

Não é a escolha do MVP porque o Worker precisa processar uma fila de trabalho e remover logicamente mensagens confirmadas, não manter um histórico reaproveitável para múltiplos replays.

## 5. Durabilidade e persistência

Três configurações diferentes participam da sobrevivência a reinicializações:

| Elemento | Configuração |
|---|---|
| Exchange | `durable: true` |
| Fila classic | `durable: true` |
| Mensagem | `Persistent = true` |

Configurar apenas uma delas não basta. Uma mensagem persistente publicada para uma fila temporária ainda pode desaparecer quando a fila for removida.

Mesmo com as três configurações, o publisher não deve presumir sucesso apenas porque escreveu no socket. Para isso existem publisher confirms.

## 6. Publisher confirms

Publisher confirm é a confirmação enviada pelo RabbitMQ ao produtor quando a publicação foi aceita pelo broker.

Ele responde à pergunta:

> O RabbitMQ aceitou esta publicação?

Não responde às perguntas:

- o Worker recebeu a mensagem?
- o Worker concluiu o processamento?
- o banco do Worker foi atualizado?

Essas garantias pertencem ao ack do consumidor e à idempotência.

O publisher do OrderFlow habilitará confirms no canal. Se houver `nack`, timeout, canal fechado ou mensagem retornada por falta de rota, a API deverá registrar a falha e lançar uma exceção controlada.

Publicar com `mandatory: true` é importante para detectar uma mensagem que chegou à exchange, mas não encontrou binding compatível. Sem essa opção, uma mensagem não roteável pode ser descartada.

## 7. Ack e nack do consumidor

O Worker usará confirmação manual:

```text
autoAck: false
```

O fluxo correto será:

1. RabbitMQ entrega a mensagem ao Worker.
2. O Worker desserializa e valida o evento.
3. O Worker consulta `processed_events` pelo `EventId`.
4. Se ainda não processado, executa o efeito e persiste `ProcessedEvent`.
5. Somente depois do commit, envia `BasicAck`.

Se a conexão ou o canal cair antes do ack, o RabbitMQ recoloca a entrega pendente na fila. Isso produz semântica **at least once**: uma mensagem pode ser entregue mais de uma vez.

### Nack e requeue

| Situação | Ação sugerida |
|---|---|
| Falha transitória conhecida | Retry limitado e observável |
| Mensagem inválida | `nack` sem requeue |
| Falha permanente | Encaminhar para DLQ |
| Processo caiu antes do ack | RabbitMQ faz requeue automaticamente |

Usar `nack` com `requeue: true` indefinidamente cria um loop quente: a mesma mensagem é entregue, falha e volta imediatamente para a fila.

## 8. Prefetch

Prefetch limita quantas mensagens ainda não confirmadas o broker entrega a um consumidor.

Exemplo planejado:

```text
PrefetchCount: 16
```

Com prefetch 16, cada consumidor poderá manter até 16 entregas sem ack. Valor muito baixo reduz throughput; valor muito alto aumenta memória, tempo de recuperação e quantidade de mensagens reentregues quando um Worker cai.

O valor inicial deverá ser medido. Não existe um número universalmente ideal.

## 9. Dead-letter queue

Uma Dead Letter Exchange recebe mensagens rejeitadas, expiradas ou removidas por determinadas políticas da fila de origem. A DLQ é uma fila vinculada a essa exchange para inspeção e tratamento posterior.

Topologia futura:

```text
orderflow.worker.order-created
    |
    | dead-letter
    v
orderflow.dead-letter
    |
    v
orderflow.worker.order-created.dead-letter
```

O RabbitMQ recomenda configurar dead lettering por policies quando possível, pois policies podem mudar sem excluir e recriar filas. Argumentos `x-arguments` gravados no código são mais difíceis de alterar.

A DLQ não corrige uma mensagem. Ela impede repetição infinita e preserva evidência para diagnóstico ou reprocessamento controlado.

## 10. Idempotência

Como a entrega é at least once, o Worker precisa aceitar eventos repetidos sem repetir o efeito.

O OrderFlow já possui `ProcessedEvent` com `EventId` único. O algoritmo será:

```text
receber evento
    |
    v
EventId já existe?
    | sim                 | não
    v                     v
enviar ack          executar efeito
                           |
                           v
                 persistir ProcessedEvent
                           |
                           v
                       enviar ack
```

A restrição única no PostgreSQL é a proteção final contra dois consumidores processarem simultaneamente o mesmo `EventId`.

## 11. Conexões e canais no cliente .NET

O projeto usa `RabbitMQ.Client` 7.2.1.

Boas práticas para o publisher:

- manter conexão de longa duração;
- evitar abrir uma conexão TCP por mensagem;
- criar canal duradouro para publicação;
- habilitar recuperação automática de conexão e topologia;
- não publicar concorrentemente no mesmo canal sem sincronização;
- usar `SemaphoreSlim` no publisher singleton ou um pool de canais;
- descartar canal e conexão durante o encerramento da aplicação.

Conexões são mais pesadas e representam uma sessão TCP. Canais são conexões lógicas multiplexadas, mais leves, mas não devem receber publicações simultâneas sem coordenação.

## 12. Configuração planejada

Estrutura de `appsettings` para o laboratório:

```json
{
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "ExchangeName": "orderflow.events",
    "ExchangeType": "topic",
    "OrderCreatedRoutingKey": "order.created",
    "OrderCreatedQueue": "orderflow.worker.order-created",
    "DeadLetterExchangeName": "orderflow.dead-letter",
    "DeadLetterQueueName": "orderflow.worker.order-created.dead-letter",
    "PrefetchCount": 16
  }
}
```

Em Docker Compose, `HostName` será `rabbitmq`, que é o nome DNS do serviço dentro da rede. No host local será `localhost`.

Credenciais reais não devem ser versionadas. Em produção, `UserName` e `Password` devem vir de variáveis de ambiente ou secret manager.

## 13. Responsabilidade de cada projeto

### OrderFlow.Application

- define `OrderCreatedEvent`;
- define `IOrderCreatedPublisher`;
- chama o publisher sem conhecer RabbitMQ.

### OrderFlow.Infrastructure

- contém `RabbitMqOptions`;
- cria conexão e canal;
- declara a exchange;
- serializa o evento;
- publica com mensagem persistente, `mandatory` e confirms;
- registra logs e erros de publicação.

### OrderFlow.Api

- fornece configuração;
- registra o publisher e o `OrderService`;
- inicia e encerra o publisher com a aplicação.

### OrderFlow.Worker

- declara exchange, fila e binding;
- configura prefetch e ack manual;
- consome e desserializa mensagens;
- garante idempotência com `ProcessedEvent`;
- envia ack somente depois da persistência.

## 14. Falhas e diagnóstico

| Sintoma | Causa provável | Diagnóstico |
|---|---|---|
| Conexão recusada | Broker parado ou porta incorreta | Verificar container, porta 5672 e logs |
| `ACCESS_REFUSED` | Usuário, senha ou virtual host inválido | Conferir credenciais e permissões |
| `NOT_FOUND - no exchange` | Exchange não declarada | Verificar inicialização da topologia |
| `PRECONDITION_FAILED` | Entidade existente com propriedades diferentes | Comparar tipo, durable, auto-delete e argumentos |
| Mensagem publicada, fila vazia | Binding incompatível | Conferir exchange, routing key e binding key |
| Mensagem volta repetidamente | Falha sem ack ou nack com requeue infinito | Inspecionar logs, redelivery e DLQ |
| Mensagem processada duas vezes | Falta de idempotência | Consultar `processed_events` e índice único |
| API salvou pedido, mas evento não apareceu | Falha entre commit e publicação | Limitação sem Outbox; correlacionar OrderId e EventId |

## 15. Observabilidade pelo RabbitMQ Management

Na interface de administração, observar:

- estado da conexão da API e do Worker;
- canais abertos;
- taxa de entrada na exchange;
- bindings da exchange;
- mensagens `Ready` na fila;
- mensagens `Unacked` entregues e ainda não confirmadas;
- número de consumidores;
- redeliveries;
- mensagens na DLQ.

Comandos úteis quando o broker estiver em um container chamado `orderflow-rabbitmq`:

```powershell
rtk docker exec orderflow-rabbitmq rabbitmqctl list_exchanges name type durable
rtk docker exec orderflow-rabbitmq rabbitmqctl list_queues name type durable messages_ready messages_unacknowledged consumers
rtk docker exec orderflow-rabbitmq rabbitmqctl list_bindings source_name destination_name routing_key
rtk docker logs --tail 100 orderflow-rabbitmq
```

Interpretação:

- `messages_ready` crescendo indica que o consumidor não está acompanhando ou está parado;
- `messages_unacknowledged` alto indica processamento lento ou consumidor travado;
- zero consumidores indica que o Worker não se conectou;
- ausência de binding explica mensagens não roteadas;
- mensagens na DLQ exigem diagnóstico antes de reprocessamento.

## 16. Limitação sem Outbox

O fluxo atual será:

```text
commit PostgreSQL -> publicar RabbitMQ
```

Se o processo cair entre essas duas operações, o pedido existirá sem evento. RabbitMQ e PostgreSQL não participam da mesma transação distribuída.

O Outbox Pattern resolve isso gravando o pedido e uma linha de outbox na mesma transação PostgreSQL. Outro processo publica os registros pendentes e marca o envio. Assim, a intenção de publicar não se perde mesmo quando o broker está indisponível.

No MVP, a limitação será aceita e documentada. O publisher usará confirms e logs estruturados, mas isso não substitui Outbox.

## 17. Referências oficiais

- [Exchanges](https://www.rabbitmq.com/docs/exchanges)
- [Queues](https://www.rabbitmq.com/docs/queues)
- [Quorum queues](https://www.rabbitmq.com/docs/quorum-queues)
- [Consumer acknowledgements and publisher confirms](https://www.rabbitmq.com/docs/confirms)
- [Publishers](https://www.rabbitmq.com/docs/publishers)
- [Dead Letter Exchanges](https://www.rabbitmq.com/docs/dlx)
- [.NET/C# Client API Guide](https://www.rabbitmq.com/client-libraries/dotnet-api-guide)

