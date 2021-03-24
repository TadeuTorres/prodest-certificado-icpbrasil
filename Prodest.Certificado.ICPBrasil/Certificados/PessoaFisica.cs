using System;
using System.Globalization;

namespace Prodest.Certificado.ICPBrasil.Certificados
{
    public sealed class PessoaFisica
    {
        public string Nome { get; }
        public DateTime DataNascimento { get; }
        public string Cpf { get; }
        public string Rg { get; }
        public string OrgaoExpedidor { get; }
        public string Email { get; }

        // http://www.iti.gov.br/images/repositorio/legislacao/documentos-principais/DOC-ICP-04_-_Versao_6.3_-_REQUISITOS_MINIMOS_PARA_P.C.pdf
        // http://www.receita.fazenda.gov.br/acsrf/LeiautedeCertificadosdaSRF.pdf
        public PessoaFisica(string nome, string dados, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(dados))
                    throw new CertificadoException(CertificadoException.CertificadoExceptionTipo.PessoaFisicaInvalida);

                Nome = nome;
                if (DateTime.TryParseExact(dados.Substring(0, 8), "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataNascimento))
                    DataNascimento = dataNascimento;
                Cpf = dados.Substring(8, 11);
                var rgTemp = dados.Substring(30, 15).TrimStart(new[] { '0' });
                if (!string.IsNullOrEmpty(rgTemp))
                {
                    Rg = rgTemp;
                    OrgaoExpedidor = dados[45..];
                }
                else
                {
                    Rg = string.Empty;
                    OrgaoExpedidor = string.Empty;
                }
                Email = email;
            }
            catch (Exception ex)
            {
                throw new CertificadoException(CertificadoException.CertificadoExceptionTipo.PessoaFisicaInvalida, ex);
            }
        }
    }
}