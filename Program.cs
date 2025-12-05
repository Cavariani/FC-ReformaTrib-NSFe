using FC.NFSe.Sandbox.Models;
using FC.NFSe.Sandbox.Builder;
using FC.NFSe.Sandbox.Xml;
using FC.NFSe.Sandbox.Services;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

// Ponto de entrada da aplicação
await MainAsync();

static async Task MainAsync()
{
    Console.WriteLine("🚀 Iniciando processo de envio NFSe v2 (Reforma Tributária - SÍNCRONO)...");

    string caminhoPfx = "MACSO.pfx";
    string senhaPfx = "BC1200";

    if (!File.Exists(caminhoPfx))
    {
        Console.WriteLine($"❌ Arquivo .pfx não encontrado: {Path.GetFullPath(caminhoPfx)}");
        return;
    }

    var cert = new X509Certificate2(
        caminhoPfx,
        senhaPfx,
        X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable
    );

    // 1. DADOS DA NOTA (Ajustados para passar nas regras de negócio)
    var notas = new List<NotaFiscal>
    {
        new NotaFiscal
        {
            // Incrementei para 0003 para evitar erro de duplicidade
            NumeroRPS = "0003",
            Serie = "AAAAA",
            DataEmissao = DateTime.Today,
            
            // ✅ CORREÇÃO 1: Seu CCM real (sem pontos)
            InscricaoMunicipalPrestador = "40095380", 
            
            // ✅ CORREÇÃO 2: Seu CNPJ (Dono do certificado)
            CNPJPrestador = "05379035000105",
            RazaoSocialPrestador = "MACSO LEGATE TECNOLOGIA E SISTEMAS",
            
            // Tomador = Prestador (Auto-emissão para teste)
            CNPJTomador = "05379035000105",
            RazaoSocialTomador = "MACSO LEGATE (AUTO TESTE)",
            EnderecoTomador = "Rua X, 123",
            CodigoMunicipio = "3550308", // São Paulo
            
            ValorServicos = 1500.00M,
            ValorIBS = 1.50M,
            ValorCBS = 13.50M,
            
            // ✅ CORREÇÃO 3: Código de Serviço da sua Ficha (02800 = Software)
            CodigoServico = "02800",
            DescricaoServico = "Desenvolvimento de software customizavel - Teste API V2",

            IssRetido = false,
            TipoTributacao = "T"
        }
    };

    // 2. BUILDER DO LOTE (Com prefixo nfe:)
    var cnpjRemetente = notas.First().CNPJPrestador;
    var xmlLote = LoteNfseBuilder.CriarLoteXml(rpsList: CreateRpsList(notas, cert), cnpjRemetente);

    string caminhoXml = Path.Combine(Directory.GetCurrentDirectory(), "lote.xml");
    xmlLote.Save(caminhoXml);
    Console.WriteLine($"📁 XML gerado em: {caminhoXml}");

    // 3. ASSINATURA
    string caminhoXmlAssinado = Path.Combine(Directory.GetCurrentDirectory(), "lote_assinado.xml");
    try
    {
        XmlAssinador.AssinarXml(caminhoXml, caminhoXmlAssinado, caminhoPfx, senhaPfx);
        Console.WriteLine("🔏 Lote assinado com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao assinar: {ex.Message}");
        return;
    }

    // 4. VALIDAÇÃO XSD (Síncrono v2)
    string[] schemas = new[]
    {
        Path.Combine("Xml", "Schemas", "PedidoEnvioLoteRPS_v02.xsd"),
        Path.Combine("Xml", "Schemas", "TiposNFe_v02.xsd"),
        Path.Combine("Xml", "Schemas", "xmldsig-core-schema_v02.xsd")
    };

    Console.WriteLine("🔍 Verificando estrutura XSD local...");
    bool isLocalValid = XmlValidador.ValidarXml(caminhoXmlAssinado, schemas, out var erros);

    if (isLocalValid)
        Console.WriteLine("✅ Lote XML válido localmente!");
    else
    {
        Console.WriteLine("⚠️ Validação local falhou (mas vamos tentar enviar): " + erros);
    }

    // 5. ENVIO SÍNCRONO (SOAP 1.2)
    try
    {
        await EnvioWebService.EnviarLoteAssinadoAsync(
            caminhoXmlAssinado,
            caminhoPfx,
            senhaPfx,
            modoTeste: true
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Erro fatal: " + ex.Message);
    }

    Console.WriteLine("\n🏁 Fim.");
    Console.ReadKey();
}

static List<XElement> CreateRpsList(List<NotaFiscal> notas, X509Certificate2 cert)
{
    var list = new List<XElement>();
    foreach (var n in notas) list.Add(NotaFiscalXmlBuilder.GerarXmlRps(n, cert));
    return list;
}