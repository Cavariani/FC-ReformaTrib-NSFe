using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace FC.NFSe.Sandbox.Xml
{
    public static class XmlAssinador
    {
        public static void AssinarXml(string xmlPath, string xmlOutputPath, string caminhoPfx, string senhaPfx)
        {
            // Carrega o certificado
            var cert = new X509Certificate2(
                caminhoPfx,
                senhaPfx,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable
            );

            // Carrega XML
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(xmlPath);

            // Agora a assinatura é no ELEMENTO RAIZ
            var raiz = doc.DocumentElement
                ?? throw new Exception("Não foi possível identificar o elemento raiz do XML.");

            // Assinador
            var signedXml = new SignedXml(doc)
            {
                SigningKey = cert.GetRSAPrivateKey()
            };

            // Referência ao documento inteiro (ID da raiz)
            var reference = new Reference("");
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());
            signedXml.AddReference(reference);

            // Info do certificado
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));
            signedXml.KeyInfo = keyInfo;

            // Assina
            signedXml.ComputeSignature();
            var xmlDigitalSignature = signedXml.GetXml();

            

            // Anexa a assinatura AO FINAL da raiz
            raiz.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            doc.Save(xmlOutputPath);
        }
    }
}
