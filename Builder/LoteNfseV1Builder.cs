using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace FC.NFSe.Sandbox.Builder
{
    public static class LoteNfseV1Builder
    {
        private static readonly XNamespace NsNfe = "http://www.prefeitura.sp.gov.br/nfe";

        public static XDocument CriarLoteXmlV1(
            List<XElement> rpsList,
            string cnpjRemetente,
            decimal valorTotalServicos
        )
        {
            if (rpsList == null) throw new ArgumentNullException(nameof(rpsList));
            if (string.IsNullOrWhiteSpace(cnpjRemetente))
                throw new ArgumentNullException(nameof(cnpjRemetente));

            string hoje = DateTime.Today.ToString("yyyy-MM-dd");
            string F(decimal v) => v.ToString("0.00", CultureInfo.InvariantCulture);

            // Layout oficial v1:
            // Cabecalho Versao="1" + CPFCNPJRemetente, transacao, dtInicio, dtFim,
            // QtdRPS, ValorTotalServicos, ValorTotalDeducoes
            var cabecalho = new XElement("Cabecalho",
                new XAttribute("Versao", "1"),
                new XElement("CPFCNPJRemetente",
                    new XElement("CNPJ", cnpjRemetente)
                ),
                new XElement("transacao", true),
                new XElement("dtInicio", hoje),
                new XElement("dtFim", hoje),
                new XElement("QtdRPS", rpsList.Count),
                new XElement("ValorTotalServicos", F(valorTotalServicos)),
                new XElement("ValorTotalDeducoes", F(0))
            );

            var raiz = new XElement(NsNfe + "PedidoEnvioLoteRPS",
                cabecalho,
                rpsList
            );

            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                raiz
            );
        }
    }
}
