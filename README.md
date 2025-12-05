# Emissor NFSe SP - Reforma Tributária (v2.0) 🇧🇷

Aplicação em C# (.NET 8) para emissão de Notas Fiscais de Serviço (NFSe) no município de São Paulo, 100% compatível com o **Layout v2 (Reforma Tributária - IBS/CBS)**.

> 🚀 **Status:** Funcional. Interface de Terminal interativa com gestão automática de numeração RPS.

## ✨ Funcionalidades

* **Terminal Interativo:** Solicita dados do cliente e valores em tempo real.
* **Gestão de RPS:** Controla a numeração sequencial automaticamente (via arquivo local), evitando erros de duplicidade.
* **Reforma Tributária:** Gera XMLs compatíveis com os novos campos de IBS, CBS e NBS.
* **Arquitetura Limpa:** Lógica separada em Serviços (`EmissorService`), Modelos e Builders.
* **Validação de Schema:** Trata endereços estruturados e regras de validação da Prefeitura.
* **Comunicação SOAP 1.2:** Configuração correta de envelopes e headers para o endpoint síncrono da SP.

## 🛠️ Stack Tecnológica

* **Linguagem:** C# (.NET 8.0 Console Application)
* **Bibliotecas:** Nativas (`System.Xml`, `System.Net.Http`, `System.Security.Cryptography`).
* **Persistência:** Arquivo de texto simples para controle de sequência (`ultimo_rps.txt`).

## 📋 Como Usar

1.  **Certificado Digital:** Coloque seu arquivo `.pfx` na raiz do projeto (ou configure o caminho no `Program.cs`).
2.  **Configuração:** Ajuste a senha do certificado e o CNPJ do Prestador no código.
3.  **Executar:**
    ```bash
    dotnet run
    ```
4.  **Interagir:** Digite o CNPJ do cliente e o valor do serviço quando solicitado.

## 📂 Estrutura do Projeto

* `/Builder`: Montagem do XML (RPS e Lote) seguindo o Schema v2.
* `/Services`: `EmissorService` (Orquestrador) e `EnvioWebService` (SOAP Client).
* `/Util`: Gerenciador de sequência de RPS.
* `/Models`: Classes de domínio (NotaFiscal, RetornoNFSe).
* `/Xml/Schemas`: XSDs oficiais para validação.

## ⚠️ Notas Importantes

* O projeto está configurado para o ambiente de **Produção** (`nfews`), mas utilizando o método `TesteEnvioLoteRPS`. Isso valida regras reais sem gerar validade jurídica.
* As alíquotas de IBS/CBS estão fixas em 1% e 9% para fins de demonstração técnica.

---
**Disclaimer:** Este projeto é uma Prova de Conceito (POC) para desenvolvedores.
