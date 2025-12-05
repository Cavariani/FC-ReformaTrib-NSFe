using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FC.NFSe.Sandbox.Builder
{
    public static class LoteNfseBuilder
    {
        private static readonly XNamespace NsNfe = "http://www.prefeitura.sp.gov.br/nfe";

        public static XDocument CriarLoteXml(List<XElement> rpsList, string cnpjRemetente)
        {
            if (rpsList == null) throw new ArgumentNullException(nameof(rpsList));
            string hoje = DateTime.Today.ToString("yyyy-MM-dd");

            // FILHOS: Unqualified (Sem namespace)
            var cabecalho = new XElement("Cabecalho",
                new XAttribute("Versao", "2"),
                new XElement("CPFCNPJRemetente",
                    new XElement("CNPJ", cnpjRemetente)
                ),
                new XElement("transacao", "true"),
                new XElement("dtInicio", hoje),
                new XElement("dtFim", hoje),
                new XElement("QtdRPS", rpsList.Count)
            );

            // RAIZ: Com namespace e prefixo nfe
            var raiz = new XElement(NsNfe + "PedidoEnvioLoteRPS",
                new XAttribute(XNamespace.Xmlns + "nfe", NsNfe.NamespaceName),
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