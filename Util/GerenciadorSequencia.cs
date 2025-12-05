using System;
using System.IO;

namespace FC.NFSe.Sandbox.Util
{
    public static class GerenciadorSequencia
    {
        private const string ARQUIVO_DB = "ultimo_rps.txt";

        public static string ObterProximoRps()
        {
            int ultimo = 0;

            // 1. Tenta ler o último número salvo
            if (File.Exists(ARQUIVO_DB))
            {
                string conteudo = File.ReadAllText(ARQUIVO_DB);
                int.TryParse(conteudo, out ultimo);
            }

            // 2. Incrementa
            int proximo = ultimo + 1;

            // 3. Salva o novo número para a próxima vez
            File.WriteAllText(ARQUIVO_DB, proximo.ToString());

            // 4. Retorna formatado (ex: "0008")
            return proximo.ToString().PadLeft(4, '0');
        }
    }
}