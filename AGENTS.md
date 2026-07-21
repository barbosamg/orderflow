# Regras de colaboração do OrderFlow

- O desenvolvimento deste projeto deve acontecer em modo guiado: o usuário cria, altera e executa o código seguindo as orientações do Codex.
- O Codex não deve implementar código da aplicação diretamente, salvo quando o usuário autorizar isso de forma explícita.
- A atualização da documentação do projeto é responsabilidade do Codex ou do agente de IA em uso; ao concluir cada bloco funcional, o agente deve atualizar os documentos de acompanhamento aplicáveis e realizar o commit documental após validar o escopo.
- Cada orientação deve apresentar pelo menos os próximos 5 passos, numerados e em ordem de execução.
- Sempre que possível, cada passo deve incluir o comando ou conteúdo necessário e a validação esperada.
- Se algum passo falhar, o usuário deve interromper o bloco e enviar o erro; o Codex deve diagnosticar a falha antes de orientar a continuação.
- Depois que o usuário concluir um bloco, o Codex deve analisar os resultados antes de fornecer os próximos 5 passos ou mais.
- Todas as mensagens sugeridas devem seguir Conventional Commits: o prefixo deve permanecer em inglês, como `feat`, `fix`, `docs`, `test`, `refactor` ou `chore`, e a descrição após o prefixo deve ser escrita em português do Brasil.
- Nas classes C#, os membros devem seguir esta ordem: propriedades no início da classe, agrupadas por visibilidade na ordem `protected`, `private` e `public`; depois os construtores, primeiro o construtor sem parâmetros e em seguida os construtores com parâmetros; por último, os métodos.
- Todos os arquivos textuais do projeto devem permanecer em UTF-8 sem BOM, conforme a configuração `charset = utf-8` definida no `.editorconfig`.
