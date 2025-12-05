# Integração NFSe SP - v2 (Reforma Tributária) 🇧🇷

Este repositório contém uma **Prova de Conceito (POC)** funcional em C# (.NET 8) para emissão de Notas Fiscais de Serviço (NFSe) no município de São Paulo, compatível com o **Layout v2 (Reforma Tributária - IBS/CBS)**.

> ⚠️ **Status Atual:** POC Funcional. Comunicação, Assinatura e Validação de Lote funcionando com sucesso no ambiente da Prefeitura.

## 🎯 Contexto

A Prefeitura de SP lançou o **Layout v2** para adequação à Reforma Tributária. A documentação oficial apresenta desafios em relação aos protocolos de comunicação (SOAP) e validação de esquemas (XSD).

Este projeto resolve a barreira de comunicação, conseguindo enviar um lote assinado e receber o retorno de processamento com sucesso.

## 🚀 Como Funciona (A "Golden Config")

Para quem for mexer no código, o "segredo" da integração que faz funcionar é:

1.  **Protocolo:** **SOAP 1.2** (Obrigatório). O SOAP 1.1 retorna Erro 500. É necessário passar a `Action` dentro do Header `Content-Type`.
2.  **Método de Envio:** **Síncrono** (`lotenfe.asmx`).
    * *Nota:* O método Assíncrono (`Async`) para a v2 demonstrou instabilidade/erros de schema durante os testes, por isso optamos pelo Síncrono que processa o lote em tempo real.
3.  **Estrutura do XML:**
    * Raiz (`PedidoEnvioLoteRPS`) com prefixo de namespace (ex: `nfe:`).
    * Tags filhas **Unqualified** (sem prefixo de namespace).
4.  **Ambiente:** Utiliza a URL de Produção (`nfews.prefeitura.sp.gov.br`), mas consome o método `TesteEnvioLoteRPS`.
    * Isso garante validação real (Certificado, XSD, Regras de Negócio) **sem gerar validade jurídica ou dívida tributária**.

## 🛠️ Stack Tecnológica

* **Linguagem:** C# (.NET 8.0 Console Application)
* **Comunicação:** `HttpClient` (Montagem manual do Envelope SOAP 1.2)
* **Assinatura:** `System.Security.Cryptography.Xml` (SignedXml)
* **Criptografia:** Certificado Digital A1 (.pfx)

## 📋 Pré-requisitos para Rodar

1.  **Certificado Digital:** É necessário um arquivo `.pfx` válido (Modelo A1).
2.  **Configuração:**
    * Coloque o arquivo `.pfx` na raiz do executável ou configure o caminho no `Program.cs`.
    * Ajuste a senha do certificado no código.
    * **Importante:** O CNPJ do prestador no XML deve bater com o CNPJ do certificado.

## 📍 Estado Atual do Projeto

O projeto encontra-se na fase de **MVP/Prototipação**.

- [x] **Autenticação TLS:** Conexão segura com certificado cliente.
- [x] **Assinatura Digital:** Assinatura do RPS válida (SHA1/RSA).
- [x] **Comunicação SOAP 1.2:** Handshake e envio do Envelope corretos.
- [x] **Validação XSD v2:** XML estruturado conforme Manual da Reforma Tributária.
- [x] **Retorno de Sucesso:** A prefeitura processa o lote e retorna `<Sucesso>true</Sucesso>`.
- [ ] **Deserialização:** O retorno ainda é lido como string bruta no console. Necessário mapear para objetos C#.
- [ ] **Dados Dinâmicos:** Os dados da nota (Tomador, Serviço, Valor) estão *hardcoded* no `Program.cs`.
- [ ] **Gestão de RPS:** Não há controle automático da numeração sequencial do RPS.

## 📂 Estrutura de Pastas

* `/Builder`: Lógica de construção do XML (RPS e Lote) e regras de negócio da v2 (IBS/CBS).
* `/Services`: Camada de transporte (SOAP Client).
* `/Xml/Schemas`: Arquivos XSD oficiais para validação local.
* `/Models`: Classes de domínio (NotaFiscal, etc).

---

**Aviso:** Não suba arquivos `.pfx` ou senhas reais para este repositório. Use variáveis de ambiente ou segredos em produção.
