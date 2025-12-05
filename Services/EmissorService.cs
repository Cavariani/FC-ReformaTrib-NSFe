using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FC.NFSe.Sandbox.Builder;
using FC.NFSe.Sandbox.Models;
using FC.NFSe.Sandbox.Xml;

namespace FC.NFSe.Sandbox.Services
{
    public class EmissorService
    {
        private readonly X509Certificate2 _certificado;
        private readonly string _caminhoPfx;
        private readonly string _senhaPfx;

        public EmissorService(string caminhoPfx, string senhaPfx)
        {
            _caminhoPfx = caminhoPfx;
            _senhaPfx = senhaPfx;

            if (!File.Exists(caminhoPfx))
                throw new FileNotFoundException($"Certificado não encontrado: {caminhoPfx}");

            _certificado = new X509Certificate2(
                caminhoPfx,
                senhaPfx,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable
            );
        }

        public async Task<RetornoEnvioLoteRPS?> EmitirNotaAsync(NotaFiscal nota, bool modoTeste = true)
        {
            // 1. Preparar Lista (Lote de 1 rps)
            var listaRps = new List<System.Xml.Linq.XElement>();
            listaRps.Add(NotaFiscalXmlBuilder.GerarXmlRps(nota, _certificado));

            // 2. Construir XML do Lote
            var xmlLote = LoteNfseBuilder.CriarLoteXml(listaRps, nota.CNPJPrestador);

            // 3. Salvar Temporariamente (Para assinar)
            string pastaTemp = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            Directory.CreateDirectory(pastaTemp); // Garante que a pasta existe

            string caminhoXml = Path.Combine(pastaTemp, $"lote_{nota.NumeroRPS}.xml");
            string caminhoXmlAssinado = Path.Combine(pastaTemp, $"lote_{nota.NumeroRPS}_assinado.xml");

            xmlLote.Save(caminhoXml);

            // 4. Assinar
            try
            {
                XmlAssinador.AssinarXml(caminhoXml, caminhoXmlAssinado, _caminhoPfx, _senhaPfx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na assinatura digital: {ex.Message}");
            }

            // 5. Enviar (Chama o nosso WebService já pronto)
            return await EnvioWebService.EnviarLoteAssinadoAsync(
                caminhoXmlAssinado,
                _caminhoPfx,
                _senhaPfx,
                modoTeste
            );
        }
    }
}