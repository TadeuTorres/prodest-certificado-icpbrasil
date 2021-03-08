using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Reflection;

namespace UnitTests.Context
{
    public static class CertificadoDigitalTestsContext
    {
        private const string ECpf = "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";
        private const string ECnpj = "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";

        // Só funciona no Windows 10 para frente, não roda se for 2012 ou anterior
        public static string SelfSignedPassword => "Qweasd123!@#";

        public static string SelfSignedPath => GetFilePath("Certificados/Outros/SelfSigned.pfx");

        public static string CnpjCerPath => GetFilePath("Certificados/Validos/cnpj.cer");

        public static byte[] ObterCertificado(CertificadoTipo tipo)
        {
            string path;
            switch (tipo)
            {
                case CertificadoTipo.ECnpjString:
                    return Convert.FromBase64String(ECnpj);

                case CertificadoTipo.ECpfString:
                    return Convert.FromBase64String(ECpf);

                case CertificadoTipo.FileECpfValido:
                    path = GetFilePath("Certificados/Validos/cpf.pdf");
                    break;

                case CertificadoTipo.FileECpfExpirado:
                    path = GetFilePath("Certificados/Expirados/cpf.pdf");
                    break;

                case CertificadoTipo.ArquivoTeste:
                    path = GetFilePath("Certificados/Outros/teste.pdf");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null);
            }

            var reader = new PdfReader(path);
            var acroFields = reader.AcroFields;
            var signatures = acroFields.GetSignatureNames();
            var signature = signatures[0]!.ToString();
            var pkcs7 = acroFields.VerifySignature(signature);
            return pkcs7.SigningCertificate.GetEncoded();
        }

        private static string GetFilePath(string fileName)
        {
            var assemblyDir = AssemblyDirectory;
            return Path.Combine(
                assemblyDir,
                "Context",
                fileName
                );
        }

        private static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase!);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path)!;
            }
        }

        public enum CertificadoTipo
        {
            ECpfString,
            ECnpjString,
            FileECpfValido,
            FileECpfExpirado,
            ArquivoTeste
        }
    }
}