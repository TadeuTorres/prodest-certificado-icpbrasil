using System;

namespace Prodest.Certificado.ICPBrasil.Certificados
{
    public sealed class PessoaJuridica
    {
        public string Cnpj { get; }
        public string RazaoSocial { get; }
        public string Inss { get; }

        public PessoaJuridica(string cnpj, string inss, string razaoSocial)
        {
            try
            {
                if (string.IsNullOrEmpty(cnpj) || string.IsNullOrEmpty(razaoSocial))
                    throw new CertificadoException(CertificadoException.CertificadoExceptionTipo.PessoaJuridicaInvalida);

                Cnpj = cnpj;
                Inss = inss;
                RazaoSocial = razaoSocial;
            }
            catch (Exception ex)
            {
                throw new CertificadoException(CertificadoException.CertificadoExceptionTipo.PessoaJuridicaInvalida, ex);
            }
        }
    }
}