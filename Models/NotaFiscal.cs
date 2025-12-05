using System;

namespace FC.NFSe.Sandbox.Models
{
    public class NotaFiscal
    {
        public string? NumeroRPS { get; set; }
        public string Serie { get; set; } = "AAAAA";
        public DateTime DataEmissao { get; set; }

        public string? InscricaoMunicipalPrestador { get; set; }
        public string? CNPJPrestador { get; set; }
        public string? RazaoSocialPrestador { get; set; }

        public string? CNPJTomador { get; set; }
        public string? CPFTomador { get; set; }
        public string? RazaoSocialTomador { get; set; }

        // Endereço Estruturado (Nullable para evitar CS8618)
        public string? LogradouroTomador { get; set; }
        public string? NumeroTomador { get; set; }
        public string? ComplementoTomador { get; set; }
        public string? BairroTomador { get; set; }
        public string? CodigoMunicipio { get; set; }
        public string? UFTomador { get; set; }
        public string? CepTomador { get; set; }

        public decimal ValorServicos { get; set; }
        public decimal ValorIBS { get; set; }
        public decimal ValorCBS { get; set; }

        public string? CodigoServico { get; set; }
        public string? DescricaoServico { get; set; }

        public bool IssRetido { get; set; } = false;
        public string TipoTributacao { get; set; } = "T";
    }
}