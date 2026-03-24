Projeto Concilig 


1. Criar o projeto no Visual Studio utilizando ASP.NET Core Web App (Razor Pages). FEITO!

2. Executar o projeto e validar se as telas de login e cadastro estao funcionando corretament. FEITO!

3. Configurar a string de conexao do servidor SQL Server correto. FEITO!

4. Executar os comandos Add-Migration InitialCreate e Update-Database para criar o banco de
dados e as tabelas do Identity.

5. Criar as entidades Contrato e Importacao dentro da pasta Models com os campos necessarios
para o dominio.

6. Atualizar o DbContext adicionando DbSet para Contratos e Importacoes.

7. Criar nova migration com Add-Migration CriacaoEntidades e atualizar o banco com
Update-Database.

8. Criar a pasta Services e implementar o ImportacaoService com a logica de leitura do arquivo
CSV separado por ponto e virgula.

9. Registrar o ImportacaoService no Program.cs utilizando AddScoped.

10. Criar a pagina Razor Pages chamada Upload com formulario para envio de arquivo

11. Implementar no Upload o metodo OnPostAsync para chamar o ImportacaoService e salvar os
dados no banco.

12. Validar que os dados estao sendo inseridos corretamente nas tabelas Contratos e
Importacoes.

13. Criar pagina de listagem de contratos exibindo os principais campos.

14. Criar pagina de historico de importacoes exibindo nome do arquivo, data, usuario e quantidade
de registros.

15. Implementar consulta por cliente com soma total dos contratos e calculo do maior atraso em
dias.

16. Garantir tratamento de erros na leitura do CSV ignorando linhas invalidas.

17. Organizar o codigo seguindo separacao de responsabilidades entre Pages e Services

18. Revisar o projeto completo garantindo que tudo esta funcional, limpo e organizado.

19. Realizar testes finais de importacao, login e consultas.

20. Subir o projeto para um repositorio Git com commits organizados e descritivos.