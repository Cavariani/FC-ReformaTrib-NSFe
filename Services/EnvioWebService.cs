using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FC.NFSe.Sandbox.Services
{
    public static class EnvioWebService
    {
        // Endpoint SÍNCRONO (Ambiente Novo - nfews)
        private const string Endpoint = "https://nfews.prefeitura.sp.gov.br/lotenfe.asmx";

        public static async Task EnviarLoteAssinadoAsync(
            string caminhoXmlAssinado,
            string caminhoPfx,
            string senhaPfx,
            bool modoTeste = true
        )
        {
            if (!File.Exists(caminhoXmlAssinado))
                throw new FileNotFoundException($"Arquivo XML não encontrado: {caminhoXmlAssinado}");

            // Lê e prepara o XML
            var xmlAssinado = await File.ReadAllTextAsync(caminhoXmlAssinado, Encoding.UTF8);
            var conteudoXml = RemoverDeclaracaoXml(xmlAssinado);
            string mensagemXmlCData = $"<![CDATA[{conteudoXml}]]>";

            // 1. CONFIGURAÇÃO DA ACTION (SOAP 1.2)
            string actionName = modoTeste ? "TesteEnvioLoteRPS" : "EnvioLoteRPS";
            // O namespace com /ws/ é o padrão do novo ambiente
            string soapAction = $"http://www.prefeitura.sp.gov.br/nfe/ws/{actionName}";

            string bodyTagName = modoTeste ? "TesteEnvioLoteRPSRequest" : "EnvioLoteRPSRequest";
            string bodyNamespace = "http://www.prefeitura.sp.gov.br/nfe";

            // 2. ENVELOPE SOAP 1.2
            // Namespace específico: http://www.w3.org/2003/05/soap-envelope
            var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                 xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
  <soap12:Body>
    <{bodyTagName} xmlns=""{bodyNamespace}"">
      <VersaoSchema>2</VersaoSchema>
      <MensagemXML>{mensagemXmlCData}</MensagemXML>
    </{bodyTagName}>
  </soap12:Body>
</soap12:Envelope>";

            // 3. PREPARAÇÃO DO CLIENTE HTTP
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(
                new X509Certificate2(caminhoPfx, senhaPfx, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable)
            );
            // Ignorar erros de SSL (comum em homologação/certificado do governo)
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Clear();

            // 4. CONTENT-TYPE (Onde a Action vai no SOAP 1.2)
            var content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml");
            content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("action", $"\"{soapAction}\""));

            Console.WriteLine($"🚀 Enviando MODO SÍNCRONO (v2) | SOAP 1.2");
            Console.WriteLine($"🌐 URL: {Endpoint}");
            Console.WriteLine($"🔧 Action: {soapAction}");

            try
            {
                var response = await client.PostAsync(Endpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"🔄 Status HTTP: {response.StatusCode}");
                Console.WriteLine("📩 RESPOSTA DO SERVIDOR (FORMATADA):");

                // 5. TRATAMENTO VISUAL (Limpando o &lt; &gt;)
                try
                {
                    var docSoap = XDocument.Parse(responseString);

                    // Procura a tag que contém o XML escapado (string)
                    // Geralmente é <RetornoXML> ou <TesteEnvioLoteRPSResult>
                    var noRetorno = docSoap.Descendants()
                                           .FirstOrDefault(x => x.Name.LocalName == "RetornoXML" ||
                                                                x.Name.LocalName.EndsWith("Result"));

                    if (noRetorno != null && !string.IsNullOrWhiteSpace(noRetorno.Value))
                    {
                        // .Value decodifica automaticamente os caracteres HTML
                        string xmlInternoReal = noRetorno.Value;

                        // Parseia novamente para formatar bonito (identado)
                        var docInterno = XDocument.Parse(xmlInternoReal);
                        Console.WriteLine(docInterno.ToString());
                    }
                    else
                    {
                        // Se não achar a tag interna, mostra o envelope bruto mesmo
                        Console.WriteLine(docSoap.ToString());
                    }
                }
                catch
                {
                    // Se falhar o parse (ex: erro HTML do IIS), mostra bruto
                    Console.WriteLine(responseString);
                }

                Console.WriteLine("--------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro na requisição HTTP: {ex.Message}");
            }
        }

        private static string RemoverDeclaracaoXml(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return string.Empty;
            int index = xml.IndexOf("?>");
            if (index > -1) return xml.Substring(index + 2).Trim();
            return xml;
        }
    }
}