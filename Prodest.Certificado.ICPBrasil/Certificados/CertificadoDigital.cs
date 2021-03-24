using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Prodest.Certificado.ICPBrasil.Certificados
{
    public class CertificadoDigitalOptions
    {
        public bool ValidarCadeia { get; set; } = true;

        public bool ValidarRevogacao { get; set; } = true;

        public bool ValidarRaizConfiavel { get; set; } = true;
    }

    public sealed class CertificadoDigital
    {
        public static CertificadoDigital Processar(X509Certificate2 certificado, CertificadoDigitalOptions options)
        {
            _ = certificado ?? throw new ArgumentNullException(nameof(certificado));

            return new CertificadoDigital(certificado, options);
        }

        public static CertificadoDigital Processar(byte[] buffer, CertificadoDigitalOptions options)
        {
            _ = buffer ?? throw new ArgumentNullException(nameof(buffer));

            using var certificado = new X509Certificate2(buffer);
            return new CertificadoDigital(certificado, options);
        }

        private const string RaizIcpBrasil = "Autoridade Certificadora Raiz Brasileira";

        private const string OidNomeAlternativoSujeito = "2.5.29.17";
        private const string OidPfDadosTitular = "2.16.76.1.3.1";
        private const string OidPjNomeResponsavel = "2.16.76.1.3.2";
        private const string OidPjCnpj = "2.16.76.1.3.3";
        private const string OidPjDadosResponsavel = "2.16.76.1.3.4";
        private const string OidPjInss = "2.16.76.1.3.7";
        private const string OidPjNomeEmpresarial = "2.16.76.1.3.8";
        //private const string OidPfTituloEleitor = "2.16.76.1.3.5"
        //private const string OidPfInss = "2.16.76.1.3.6"
        //private const string OidPfRegistroIdentidadeCivil = "2.16.76.1.3.9"
        //private const string OidPfRegistroServidorRh = "2.16.76.1.3.11"

        public TipoCertificado TipoCertificado { get; }
        public bool IcpBrasil { get; private set; }
        public bool CadeiaValida { get; private set; }
        public PessoaFisica? PessoaFisica { get; }
        public PessoaJuridica? PessoaJuridica { get; }
        public string? RawCertDataString { get; }
        public bool Erro { get; private set; }
        public string? ErroMensagem { get; private set; }

        private CertificadoDigital(X509Certificate2 certificado, CertificadoDigitalOptions options)
        {
            Erro = false;
            TipoCertificado = TipoCertificado.Invalido;
            try
            {
                RawCertDataString = certificado.GetRawCertDataString();
            }
            catch
            {
                Erro = true;
                ErroMensagem = CertificadoException.GetErrorMessage(CertificadoException.CertificadoExceptionTipo.CertificadoInvalido);
                return;
            }

            if (options.ValidarCadeia && CadeiaInvalida(certificado, options)) return;

            TipoCertificado = ObterTipo(certificado);
            switch (TipoCertificado)
            {
                case TipoCertificado.ECpf:
                    PessoaFisica = ObterDadosPessoaFisica(certificado);
                    break;

                case TipoCertificado.ECnpj:
                    var (pessoaJuridica, pessoaFisica) = ObterDadosPessoaJuridica(certificado);
                    PessoaJuridica = pessoaJuridica;
                    PessoaFisica = pessoaFisica;
                    break;
            }
        }

        private bool CadeiaInvalida(X509Certificate2 certificado, CertificadoDigitalOptions options)
        {
            using var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var raizesIcpBrasil = store.Certificates.Find(X509FindType.FindByIssuerName, RaizIcpBrasil, true);

            using (var chain = new X509Chain())
            {
                if (options.ValidarRevogacao)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                }
                else
                {
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                }

                if (chain.Build(certificado))
                {
                    CadeiaValida = true;
                }
                else if (chain.ChainStatus.Any(x => x.Status == X509ChainStatusFlags.NotTimeValid) && options.ValidarRevogacao)
                {
                    ErroMensagem = CertificadoException.GetErrorMessage(CertificadoException.CertificadoExceptionTipo
                        .CertificadoExpirado);
                    Erro = true;
                    return true;
                }
                else if (
                    (options.ValidarRaizConfiavel && chain.ChainStatus.Any(x => x.Status != X509ChainStatusFlags.NotTimeValid))
                    || (!options.ValidarRaizConfiavel && chain.ChainStatus.Any(x =>
                        x.Status != X509ChainStatusFlags.NotTimeValid
                        && x.Status != X509ChainStatusFlags.UntrustedRoot)))
                {
                    ErroMensagem = CertificadoException.GetErrorMessage(CertificadoException.CertificadoExceptionTipo
                        .CadeiaInvalida);
                    Erro = true;
                    return true;
                }

                var certificadoRaiz = chain.ChainElements[^1].Certificate;
                IcpBrasil = raizesIcpBrasil.Contains(certificadoRaiz);
            }

            if (IcpBrasil) return false;
            ErroMensagem = CertificadoException.GetErrorMessage(CertificadoException.CertificadoExceptionTipo
                .NaoEhIcpBrasil);
            Erro = true;
            return true;
        }

        private static TipoCertificado ObterTipo(X509Certificate2 certificado)
        {
            foreach (var extension in certificado.Extensions)
            {
                if (extension.Oid.Value != OidNomeAlternativoSujeito) continue;
                if (extension.Format(false).Contains(OidPfDadosTitular, StringComparison.Ordinal))
                    return TipoCertificado.ECpf;
                if (extension.Format(false).Contains(OidPjDadosResponsavel, StringComparison.Ordinal))
                    return TipoCertificado.ECnpj;
            }

            return TipoCertificado.Outro;
        }

        private static PessoaFisica ObterDadosPessoaFisica(X509Certificate2 certificado)
        {
            try
            {
                var nomeTitular = ObterNomeCn(certificado);
                foreach (var ext in certificado.Extensions)
                {
                    if (ext.Oid.Value != OidNomeAlternativoSujeito) continue;
                    return ObterDadosPessoa(nomeTitular, ext);
                }
            }
            catch (Exception ex)
            {
                throw new CertificadoException(CertificadoException.CertificadoExceptionTipo.ErroObterDadosPessoaFisica, ex);
            }
            throw new CertificadoException(CertificadoException.CertificadoExceptionTipo.ErroObterDadosPessoaFisica);
        }

        private static PessoaFisica ObterDadosPessoa(string nomeTitular, X509Extension ext)
        {
            var extensao = ext.RawData;
            var helper = new Asn1Helper(ref extensao);
            var dadosTitular = string.Empty;
            var email = string.Empty;

            for (var i = 0; i < helper.TagList.Count; i++)
            {
                if (helper.TagList[i].TagId == TagId.Rfc822Name)
                {
                    email = helper.TagList[i].Format(extensao);
                }
                else if (helper.TagList[i].TagId == TagId.ObjectIdentifier)
                {
                    var oid = helper.TagList[i].Format(extensao);
                    if (oid != OidPfDadosTitular) continue;
                    do
                    {
                        i++;
                    } while ((i < helper.TagList.Count) &&
                             ((helper.TagList[i].TagId != TagId.OctetString) &&
                              (helper.TagList[i].TagId != TagId.Utf8String) &&
                              (helper.TagList[i].TagId != TagId.PrintableString)));

                    if (i < helper.TagList.Count)
                    {
                        dadosTitular = helper.TagList[i].Format(extensao);
                    }
                }
            }
            return new PessoaFisica(nomeTitular, dadosTitular, email);
        }

        private static string ObterNomeCn(X509Certificate2 certificado)
        {
            var ini = certificado.Subject.IndexOf("CN=", StringComparison.Ordinal) + 3;
            var meio = certificado.Subject.IndexOf(":", ini, StringComparison.Ordinal);
            if (meio != -1)
            {
                return certificado.Subject[ini..meio];
            }
            else
            {
                var fim = certificado.Subject.IndexOf(", ", ini, StringComparison.Ordinal) - 1;
                return certificado.Subject.Substring(ini, fim - ini + 1);
            }
        }

        private static (PessoaJuridica pessoaJuridica, PessoaFisica pessoaFisica) ObterDadosPessoaJuridica(X509Certificate2 certificado)
        {
            try
            {
                var razaoSocialCn = ObterNomeCn(certificado);

                foreach (var ext in certificado.Extensions)
                {
                    if (ext.Oid.Value != OidNomeAlternativoSujeito) continue;
                    return ObterDadosEmpresa(razaoSocialCn, ext);
                }
            }
            catch (Exception ex)
            {
                throw new CertificadoException(CertificadoException.CertificadoExceptionTipo.ErroObterDadosPessoaJuridica, ex);
            }
            throw new CertificadoException(CertificadoException.CertificadoExceptionTipo.ErroObterDadosPessoaJuridica);
        }

        private static (PessoaJuridica pessoaJuridica, PessoaFisica pessoaFisica) ObterDadosEmpresa(string razaoSocialCn, X509Extension ext)
        {
            var cnpj = string.Empty;
            var inss = string.Empty;
            var razaoSocial = string.Empty;
            var nomeResponsavel = string.Empty;
            var dadosResponsavel = string.Empty;
            var email = string.Empty;

            var extensao = ext.RawData;
            var helper = new Asn1Helper(ref extensao);
            for (var i = 0; i < helper.TagList.Count; i++)
            {
                if (helper.TagList[i].TagId == TagId.Rfc822Name)
                {
                    email = helper.TagList[i].Format(extensao);
                    continue;
                }

                if (helper.TagList[i].TagId != TagId.ObjectIdentifier) continue;
                var oid = helper.TagList[i].Format(extensao);
                do
                {
                    i++;
                } while (i < helper.TagList.Count
                         && helper.TagList[i].TagId != TagId.OctetString
                         && helper.TagList[i].TagId != TagId.Utf8String
                         && helper.TagList[i].TagId != TagId.PrintableString);
                SetarInformacao(ref cnpj, ref inss, ref razaoSocial, ref nomeResponsavel, ref dadosResponsavel, helper.TagList[i].Format(extensao), oid);
            }
            if (string.IsNullOrEmpty(razaoSocial))
            {
                razaoSocial = razaoSocialCn;
            }
            return (new PessoaJuridica(cnpj, inss, razaoSocial), new PessoaFisica(nomeResponsavel, dadosResponsavel, email));
        }

        private static void SetarInformacao(ref string cnpj, ref string inss, ref string razaoSocial, ref string nomeResponsavel, ref string dadosResponsavel, string informacao, string oid)
        {
            if (oid.Equals(OidPjCnpj, StringComparison.Ordinal))
                cnpj = informacao;
            else if (oid.Equals(OidPjInss, StringComparison.Ordinal))
                inss = informacao;
            else if (oid.Equals(OidPjNomeEmpresarial, StringComparison.Ordinal))
                razaoSocial = informacao;
            else if (oid.Equals(OidPjNomeResponsavel, StringComparison.Ordinal))
                nomeResponsavel = informacao;
            else if (oid.Equals(OidPjDadosResponsavel, StringComparison.Ordinal))
                dadosResponsavel = informacao;
        }
    }

    public enum TipoCertificado
    {
        ECpf,
        ECnpj,
        Outro,
        Invalido
    }
}