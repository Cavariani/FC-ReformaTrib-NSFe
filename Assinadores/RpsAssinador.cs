using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FC.NFSe.Sandbox.Models;

namespace FC.NFSe.Sandbox.Util // Verifique se o namespace bate com a pasta "Assinadores" ou "Util"
{
    public static class RpsAssinador
    {
        public static string GerarAssinaturaRps(NotaFiscal nota, X509Certificate2 certificado)
        {
            // Tabela de Assinatura v2.0

            // 1. Inscrição Municipal Prestador (12 posições) - Tratamento de Nulo
            string inscricaoPrestador = (nota.InscricaoMunicipalPrestador ?? "0").PadLeft(12, '0');

            // 2. Série do RPS (5 posições)
            string serie = (nota.Serie ?? "").PadRight(5, ' ');

            // 3. Número do RPS (12 posições)
            string numeroRps = (nota.NumeroRPS ?? "0").PadLeft(12, '0');

            // 4. Data de Emissão (AAAAMMDD)
            string dataEmissao = nota.DataEmissao.ToString("yyyyMMdd");

            // 5. Tipo de Tributação (1 posição)
            string tributacao = (nota.TipoTributacao ?? "T").Substring(0, 1);

            // 6. Status do RPS (1 posição)
            string status = "N";

            // 7. ISS Retido (1 posição)
            string issRetido = nota.IssRetido ? "S" : "N";

            // 8. Valor Inicial/Final (15 posições)
            string valorServicos = ConverterValor(nota.ValorServicos);

            // 9. Valor das Deduções (15 posições)
            string valorDeducoes = ConverterValor(0);

            // 10. Código do Serviço (5 posições)
            string codigoServico = (nota.CodigoServico ?? "0").PadLeft(5, '0');

            // 11. Indicador CPF/CNPJ Tomador (1=CPF, 2=CNPJ, 3=Não Inf)
            string indTomador = "3";
            string cpfCnpjTomador = "".PadLeft(14, '0');

            if (!string.IsNullOrEmpty(nota.CNPJTomador))
            {
                indTomador = "2";
                cpfCnpjTomador = nota.CNPJTomador.PadLeft(14, '0');
            }
            else if (!string.IsNullOrEmpty(nota.CPFTomador))
            {
                indTomador = "1";
                cpfCnpjTomador = nota.CPFTomador.PadLeft(14, '0');
            }

            // Montagem da String
            var sb = new StringBuilder();
            sb.Append(inscricaoPrestador);
            sb.Append(serie);
            sb.Append(numeroRps);
            sb.Append(dataEmissao);
            sb.Append(tributacao);
            sb.Append(status);
            sb.Append(issRetido);
            sb.Append(valorServicos);
            sb.Append(valorDeducoes);
            sb.Append(codigoServico);
            sb.Append(indTomador);
            sb.Append(cpfCnpjTomador);

            // Assinatura SHA1 com RSA
            byte[] data = Encoding.ASCII.GetBytes(sb.ToString());

            using (RSA? rsa = certificado.GetRSAPrivateKey())
            {
                if (rsa == null)
                {
                    throw new InvalidOperationException("O certificado digital não possui uma chave privada válida para assinatura.");
                }

                // A Prefeitura de SP usa SHA1 (padrão legado ainda vigente para RPS)
                byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(signature);
            }
        }











        private static string ConverterValor(decimal valor)
        {
            long v = (long)(valor * 100);
            return v.ToString().PadLeft(15, '0'); 
        }
    }
}