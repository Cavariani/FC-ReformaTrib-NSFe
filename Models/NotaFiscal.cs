namespace FC.NFSe.Sandbox.Models
{
    public class NotaFiscal
    {
        public string NumeroRPS { get; set; }
        public string Serie { get; set; } = "AAAAA"; // Ajuste para 5 chars na assinatura
        public DateTime DataEmissao { get; set; }

        public string InscricaoMunicipalPrestador { get; set; } // OBRIGATÓRIO PARA ASSINATURA
        public string CNPJPrestador { get; set; }
        public string RazaoSocialPrestador { get; set; }

        public string CNPJTomador { get; set; }
        public string CPFTomador { get; set; } // Adicionado
        public string RazaoSocialTomador { get; set; }
        public string EnderecoTomador { get; set; }
        public string CodigoMunicipio { get; set; }
        public string UF { get; set; } = "SP";
        public string CEP { get; set; } = "00000000";

        // Valores Reformulados v2
        public decimal ValorServicos { get; set; } // Usado como ValorInicialCobrado
        public decimal ValorIBS { get; set; }
        public decimal ValorCBS { get; set; }

        public string CodigoServico { get; set; } // Ex: "02658"
        public string DescricaoServico { get; set; }

        public bool IssRetido { get; set; } = false;
        public string TipoTributacao { get; set; } = "T";
    }
}