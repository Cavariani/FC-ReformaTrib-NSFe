using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using FC.NFSe.Sandbox.Models;

namespace FC.NFSe.Sandbox.Services
{
    public static class EnvioWebService
    {
        private const string Endpoint = "https://nfews.prefeitura.sp.gov.br/lotenfe.asmx";

        // MUDANÇA: Adicionei '?' no retorno para permitir retornar null sem aviso
        public static async Task<RetornoEnvioLoteRPS?> EnviarLoteAssinadoAsync(
            string caminhoXmlAssinado,
            string caminhoPfx,
            string senhaPfx,
            bool modoTeste = true
        )
        {
            if (!File.Exists(caminhoXmlAssinado))
                throw new FileNotFoundException($"Arquivo XML não encontrado: {caminhoXmlAssinado}");

            var xmlAssinado = await File.ReadAllTextAsync(caminhoXmlAssinado, Encoding.UTF8);
            var conteudoXml = RemoverDeclaracaoXml(xmlAssinado);
            string mensagemXmlCData = $"<![CDATA[{conteudoXml}]]>";

            string actionName = modoTeste ? "TesteEnvioLoteRPS" : "EnvioLoteRPS";
            string soapAction = $"http://www.prefeitura.sp.gov.br/nfe/ws/{actionName}";
            string bodyTagName = modoTeste ? "TesteEnvioLoteRPSRequest" : "EnvioLoteRPSRequest";
            string bodyNamespace = "http://www.prefeitura.sp.gov.br/nfe";

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

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(
                new X509Certificate2(caminhoPfx, senhaPfx, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable)
            );
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Clear();

            var content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml");
            content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("action", $"\"{soapAction}\""));

            Console.WriteLine($"🚀 Enviando MODO SÍNCRONO (v2) | SOAP 1.2");

            try
            {
                var response = await client.PostAsync(Endpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"🔄 Status HTTP: {response.StatusCode}");

                try
                {
                    var docSoap = XDocument.Parse(responseString);
                    var noRetorno = docSoap.Descendants()
                                           .FirstOrDefault(x => x.Name.LocalName == "RetornoXML" ||
                                                                x.Name.LocalName.EndsWith("Result"));

                    if (noRetorno != null && !string.IsNullOrWhiteSpace(noRetorno.Value))
                    {
                        string xmlInternoReal = noRetorno.Value;

                        // Debug visual
                        try { Console.WriteLine(XDocument.Parse(xmlInternoReal).ToString()); }
                        catch { Console.WriteLine(xmlInternoReal); }

                        // DESERIALIZAÇÃO CORRIGIDA (Sem bloco using duplicado)
                        var serializer = new XmlSerializer(typeof(RetornoEnvioLoteRPS));
                        using (var reader = new StringReader(xmlInternoReal))
                        {
                            // O '?' aqui diz pro compilador "confia, eu sei que pode vir nulo"
                            var resultado = (RetornoEnvioLoteRPS?)serializer.Deserialize(reader);
                            return resultado;
                        }
                    }
                    else
                    {
                        Console.WriteLine(docSoap.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Erro ao ler resposta: {ex.Message}");
                    Console.WriteLine(responseString);
                }

                Console.WriteLine("--------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro na requisição HTTP: {ex.Message}");
            }

            return null; // Agora permitido pois mudamos o retorno para Task<RetornoEnvioLoteRPS?>
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