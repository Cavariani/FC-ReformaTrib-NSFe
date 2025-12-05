using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace FC.NFSe.Sandbox.Xml;

public static class XmlValidador
{
    public static bool ValidarXml(string xmlPath, string[] xsdPaths, out string mensagensErro)
    {
        mensagensErro = string.Empty;
        bool valido = true;
        var erros = new StringBuilder();

        var settings = new XmlReaderSettings();
        foreach (var xsd in xsdPaths)
        {
            settings.Schemas.Add(null, xsd);
        }

        //settings.ValidationType = ValidationType.Schema;
        //settings.ValidationEventHandler += (sender, e) =>
        //{
        //    valido = false;
        //    erros.AppendLine($"[ERRO] {e.Message}");
        //};

        //using var reader = XmlReader.Create(xmlPath, settings);
        //try
        //{
        //    while (reader.Read()) { }
        //}
        //catch (Exception ex)
        //{
        //    valido = false;
        //    erros.AppendLine($"[EXCEÇÃO] {ex.Message}");
        //}


        settings.ValidationType = ValidationType.Schema;
        settings.ValidationEventHandler += (sender, e) =>
        {
            valido = false;
            erros.AppendLine($"[ERRO] {e.Message}");
        };


        using var reader = XmlReader.Create(xmlPath, settings);
        try
        {
            while (reader.Read()) { }
        }
        catch (Exception ex)
        {
            valido = false;
            erros.AppendLine($"[EXCEÇÃO] {ex.Message}");
        }






        mensagensErro = erros.ToString();
        return valido;
    }
}
