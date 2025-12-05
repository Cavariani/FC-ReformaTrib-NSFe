using FC.NFSe.Sandbox.Models;
using FC.NFSe.Sandbox.Services;
using FC.NFSe.Sandbox.Util;
using System;
using System.Threading.Tasks;

// Ponto de entrada
await MainAsync();

static async Task MainAsync()
{
    Console.Clear();
    Console.WriteLine("==============================================");
    Console.WriteLine("   🚀 EMISSOR DE NFSe SP - TERMINAL v2.0");
    Console.WriteLine("==============================================\n");

    // CONFIGURAÇÃO
    string certPath = "MACSO.pfx";
    string certPass = "BC1200";

    try
    {
        // 1. Prepara o Motor
        var emissor = new EmissorService(certPath, certPass);

        // 2. Coleta Dados do Usuário (Interatividade)
        Console.Write("Digite o CNPJ/CPF do Cliente (somente números): ");
        string docCliente = Console.ReadLine();

        Console.Write("Digite o Valor do Serviço (ex: 150.00): ");
        string valorString = Console.ReadLine();
        if (!decimal.TryParse(valorString, out decimal valorServico)) valorServico = 100.00M;

        Console.Write("Descrição do Serviço: ");
        string descricao = Console.ReadLine();

        // 3. Obtém RPS Automático
        string proximoRps = GerenciadorSequencia.ObterProximoRps();
        Console.WriteLine($"\n🔢 Gerando RPS Sequencial Nº: {proximoRps}...");

        // 4. Monta a Nota (Dados Dinâmicos + Configurações Fixas)
        var nota = new NotaFiscal
        {
            NumeroRPS = proximoRps,
            Serie = "AAAAA",
            DataEmissao = DateTime.Today,

            // PRESTADOR (Você)
            InscricaoMunicipalPrestador = "40095380",
            CNPJPrestador = "05379035000105",
            RazaoSocialPrestador = "MACSO LEGATE TECNOLOGIA",

            // TOMADOR (Cliente)
            CNPJTomador = docCliente,
            RazaoSocialTomador = "CLIENTE INFORMADO NO TERMINAL", // Em prod, você buscaria o nome no banco

            // Endereço (Mantive fixo para teste, mas poderia pedir também)
            LogradouroTomador = "Avenida Paulista",
            NumeroTomador = "2000",
            ComplementoTomador = "Conj 12",
            BairroTomador = "Bela Vista",
            CodigoMunicipio = "3550308",
            UFTomador = "SP",
            CepTomador = "01310100",

            // Valores e Impostos
            ValorServicos = valorServico,
            ValorIBS = valorServico * 0.01M, // 1% Simbólico
            ValorCBS = valorServico * 0.09M, // 9% Simbólico

            CodigoServico = "02800",
            DescricaoServico = descricao,
            IssRetido = false,
            TipoTributacao = "T"
        };

        // 5. Envia
        Console.WriteLine("\n📡 Enviando para a Prefeitura...");
        var retorno = await emissor.EmitirNotaAsync(nota, modoTeste: true);

        // 6. Resultado
        Console.WriteLine("\n----------------------------------------------");
        if (retorno != null && retorno.Cabecalho.Sucesso)
        {
            Console.WriteLine($"✅ SUCESSO! Nota de R$ {valorServico:N2} emitida.");
            if (retorno.Alertas != null)
                foreach (var a in retorno.Alertas) Console.WriteLine($"⚠️ [Alerta] {a.Descricao}");
        }
        else
        {
            Console.WriteLine("❌ FALHA NA EMISSÃO.");
            if (retorno?.Erros != null)
                foreach (var e in retorno.Erros) Console.WriteLine($"🚫 {e.Descricao}");
            else
                Console.WriteLine("🚫 Erro desconhecido de comunicação.");
        }
        Console.WriteLine("----------------------------------------------");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"💀 Erro fatal: {ex.Message}");
    }

    Console.WriteLine("\nPressione qualquer tecla para sair...");
    Console.ReadKey();
}