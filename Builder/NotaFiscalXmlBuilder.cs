using System.Globalization;
using System.Xml.Linq;
using FC.NFSe.Sandbox.Models;
using FC.NFSe.Sandbox.Util;
using System.Security.Cryptography.X509Certificates;

namespace FC.NFSe.Sandbox.Builder
{
    public static class NotaFiscalXmlBuilder
    {
        public static XElement GerarXmlRps(NotaFiscal nota, X509Certificate2 certificado)
        {
            string F(decimal v) => v.ToString("0.00", CultureInfo.InvariantCulture);
            string assinaturaRps = RpsAssinador.GerarAssinaturaRps(nota, certificado);

            // GERAÇÃO UNQUALIFIED (Sem namespace nas tags filhas)
            return new XElement("RPS",
                new XElement("Assinatura", assinaturaRps),
                new XElement("ChaveRPS",
                    new XElement("InscricaoPrestador", (nota.InscricaoMunicipalPrestador ?? "").PadLeft(12, '0')),
                    new XElement("SerieRPS", nota.Serie),
                    new XElement("NumeroRPS", nota.NumeroRPS)
                ),
                new XElement("TipoRPS", "RPS"),
                new XElement("DataEmissao", nota.DataEmissao.ToString("yyyy-MM-dd")),
                new XElement("StatusRPS", "N"),
                new XElement("TributacaoRPS", nota.TipoTributacao),

                // -- CAMPOS DA VERSÃO 1 --
                new XElement("ValorDeducoes", F(0)),
                new XElement("ValorPIS", F(0)),
                new XElement("ValorCOFINS", F(0)),
                new XElement("ValorINSS", F(0)),
                new XElement("ValorIR", F(0)),
                new XElement("ValorCSLL", F(0)),
                new XElement("CodigoServico", nota.CodigoServico),
                new XElement("AliquotaServicos", F(0.05m)),
                new XElement("ISSRetido", nota.IssRetido.ToString().ToLower()),
                new XElement("CPFCNPJTomador",
                    new XElement("CNPJ", nota.CNPJTomador)
                ),
                new XElement("Discriminacao", nota.DescricaoServico),

                // -- CAMPOS DA VERSÃO 2 (REFORMA TRIBUTÁRIA) --
                new XElement("ValorInicialCobrado", F(nota.ValorServicos)),
                new XElement("ValorIPI", F(0)),
                new XElement("ExigibilidadeSuspensa", "0"),
                new XElement("PagamentoParceladoAntecipado", "0"),

                // ✅ CORREÇÃO: NBS Válido para Software (1.15.01.00)
                new XElement("NBS", "115010000"),

                new XElement("cLocPrestacao", "3550308"), // Código IBGE SP

                // ... (código anterior)

                new XElement("IBSCBS",
                    new XElement("finNFSe", "0"),
                    new XElement("indFinal", "1"),

                    // ✅ cIndOp: 100101 (Este funcionou! Mantenha)
                    new XElement("cIndOp", "100101"),

                    new XElement("indDest", "0"),
                    new XElement("valores",
                        new XElement("trib",
                            new XElement("gIBSCBS",

                                // 🔴 CORREÇÃO DO ERRO 628
                                // Mude de "101001" para "000001" (Código Padrão/Exemplo do Manual)
                                new XElement("cClassTrib", "000001")
                            )
                        )
                    )
                )
            );
        }
    }
}