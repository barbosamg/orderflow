# Regras de colaboração do OrderFlow

- O desenvolvimento deste projeto deve acontecer em modo guiado: o usuário cria, altera e executa o código seguindo as orientações do Codex.
- O Codex não deve implementar código da aplicação diretamente, salvo quando o usuário autorizar isso de forma explícita.
- A atualização da documentação do projeto é responsabilidade do Codex ou do agente de IA em uso; ao concluir cada bloco funcional, o agente deve atualizar os documentos de acompanhamento aplicáveis e realizar o commit documental após validar o escopo.
- Cada orientação deve apresentar pelo menos os próximos 5 passos, numerados e em ordem de execução.
- Sempre que possível, cada passo deve incluir o comando ou conteúdo necessário e a validação esperada.
- As orientações devem concluir a implementação da classe integralmente, incluindo sua validação, antes de iniciar os passos de criação dos testes; não alternar entre alterações parciais na classe e nos testes.
- Se algum passo falhar, o usuário deve interromper o bloco e enviar o erro; o Codex deve diagnosticar a falha antes de orientar a continuação.
- O Codex só deve validar código, arquivos, estado do Git, builds, testes ou outros resultados quando o usuário solicitar essa validação explicitamente.
- Quando o usuário pedir apenas para prosseguir, o Codex deve continuar com as próximas orientações sem executar verificações; somente quando a validação for solicitada deve analisar os resultados antes de fornecer os próximos 5 passos ou mais.
- Todos os arquivos de um commit devem ser adicionados ao staging por um único comando `git add`, apresentado e executado em um único passo; não dividir o staging do mesmo commit entre comandos ou passos diferentes.
- Todas as mensagens sugeridas devem seguir Conventional Commits: o prefixo deve permanecer em inglês, como `feat`, `fix`, `docs`, `test`, `refactor` ou `chore`, e a descrição após o prefixo deve ser escrita em português do Brasil.
- Nas classes C#, os membros devem seguir esta ordem: propriedades no início da classe, agrupadas por visibilidade na ordem `protected`, `private` e `public`; depois os construtores, primeiro o construtor sem parâmetros e em seguida os construtores com parâmetros; por último, os métodos.
- Todos os arquivos textuais do projeto devem permanecer em UTF-8 sem BOM, conforme a configuração `charset = utf-8` definida no `.editorconfig`.
