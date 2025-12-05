using System.Collections.Generic;
using System.Xml.Serialization;

namespace FC.NFSe.Sandbox.Models
{
    [XmlRoot("RetornoEnvioLoteRPS", Namespace = "http://www.prefeitura.sp.gov.br/nfe")]
    public class RetornoEnvioLoteRPS
    {
        [XmlElement("Cabecalho", Namespace = "")]
        public CabecalhoRetorno? Cabecalho { get; set; }

        [XmlElement("Alerta", Namespace = "")]
        public List<EventoRetorno>? Alertas { get; set; }

        [XmlElement("Erro", Namespace = "")]
        public List<EventoRetorno>? Erros { get; set; }

        [XmlElement("ChaveNFeRPS", Namespace = "")]
        public List<ChaveNFeRPS>? ChavesNFe { get; set; }
    }

    public class CabecalhoRetorno
    {
        [XmlAttribute]
        public string? Versao { get; set; }

        public bool Sucesso { get; set; }

        public InformacoesLote? InformacoesLote { get; set; }
    }

    public class InformacoesLote
    {
        public string? NumeroLote { get; set; }
        public string? InscricaoPrestador { get; set; }
        public string? DataEnvioLote { get; set; }
        public int QtdNotasProcessadas { get; set; }
    }

    public class EventoRetorno
    {
        public int Codigo { get; set; }
        public string? Descricao { get; set; }
        public ChaveRPS? ChaveRPS { get; set; }
    }

    public class ChaveRPS
    {
        public string? InscricaoPrestador { get; set; }
        public string? SerieRPS { get; set; }
        public string? NumeroRPS { get; set; }
    }

    public class ChaveNFeRPS
    {
        public ChaveNFe? ChaveNFe { get; set; }
    }

    public class ChaveNFe
    {
        public string? NumeroNFe { get; set; }
        public string? CodigoVerificacao { get; set; }
    }
}