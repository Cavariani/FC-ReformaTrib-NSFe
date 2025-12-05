//using System.Globalization;
//using System.Xml.Linq;
//using System.Security.Cryptography.X509Certificates;
//using FC.NFSe.Sandbox.Models;
//using FC.NFSe.Sandbox.Util;

//namespace FC.NFSe.Sandbox.Builder
//{
//    public static class NotaFiscalXmlV1Builder
//    {
//        public static XElement GerarXmlRpsV1(NotaFiscal nota, X509Certificate2 certificado)
//        {
//            string F(decimal v) => v.ToString("0.00", CultureInfo.InvariantCulture);

//            // Assinatura de RPS segue a tabela antiga (tpRPS v1)
//            string assinaturaRps = RpsAssinador.GerarAssinaturaRps(nota, certificado);

//            var rps = new XElement("RPS",
//                new XElement("Assinatura", assinaturaRps),

//                new XElement("ChaveRPS",
//                    new XElement("InscricaoPrestador",
//                        (nota.InscricaoMunicipalPrestador ?? "0").PadLeft(8, '0')
//                    ),
//                    new XElement("SerieRPS", nota.Serie ?? "UNICA"),
//                    new XElement("NumeroRPS", (nota.NumeroRPS ?? "0").PadLeft(12, '0'))
//                ),

//                new XElement("TipoRPS", "RPS-M"),
//                new XElement("DataEmissao", nota.DataEmissao.ToString("yyyy-MM-dd")),
//                new XElement("StatusRPS", "N"),
//                new XElement("TributacaoRPS", nota.TipoTributacao ?? "T"),

//                // v1 → usa ValorServicos, não ValorInicialCobrado
//                new XElement("ValorServicos", F(nota.ValorServicos)),
//                new XElement("ValorDeducoes", F(0)),
//                new XElement("ValorPIS", F(0)),
//                new XElement("ValorCOFINS", F(0)),
//                new XElement("ValorINSS", F(0)),
//                new XElement("ValorIR", F(0)),
//                new XElement("ValorCSLL", F(0)),

//                new XElement("CodigoServico", nota.CodigoServico),
//                // Aliquota típica de exemplo (ajusta se precisar)
//                new XElement("AliquotaServicos", F(0.05m)),
//                new XElement("ISSRetido", nota.IssRetido.ToString().ToLower()),

//                // Tomador – só usa o que existe no seu modelo
//                new XElement("CPFCNPJTomador",
//                    !string.IsNullOrEmpty(nota.CNPJTomador)
//                        ? new XElement("CNPJ", nota.CNPJTomador)
//                        : (!string.IsNullOrEmpty(nota.CPFTomador)
//                            ? new XElement("CPF", nota.CPFTomador)
//                            : null)
//                ),
//                new XElement("RazaoSocialTomador", nota.RazaoSocialTomador),
//                new XElement("EnderecoTomador", nota.EnderecoTomador),
//                new XElement("UFTomador", nota.UF),
//                new XElement("CEPTomador", nota.CEP),

//                new XElement("Discriminacao", nota.DescricaoServico)
//            );

//            return rps;
//        }
//    }
//}
