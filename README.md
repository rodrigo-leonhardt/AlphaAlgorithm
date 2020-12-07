# Implementação do algoritmo Alpha Miner

Projeto de conclusão da disciplina Mineração de Processos do Programa de Pós Graduação em Sistemas de Informação da USP-Leste.

A implementação foi realizada na linguagem C#, utilizando apenas o framework [.Net Core 3.0](https://docs.microsoft.com/pt-br/dotnet/core/install/ ".Net Core 3.0") sem o auxílio de bibliotecas focadas em Mineração de Processos.


## Executando o projeto

Utilize os seguintes comandos através do terminal de sua preferência:

    git clone https://github.com/rodrigo-leonhardt/AlphaAlgorithm.git
    dotnet build
	dotnet run

A aplicação será executada e solicitará o caminho para o arquivo do log de eventos a ser descoberto pelo algoritmo.
Depois haverá a opção de exibir informações como o log de eventos simples e a matriz de relacionamento entre atividades extraídas dos logs.
Finalmente o processamento será realizado e os resultados serão impressos no terminal.