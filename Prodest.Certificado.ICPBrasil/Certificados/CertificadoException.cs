using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Prodest.Certificado.ICPBrasil.Certificados
{
    [Serializable]
    public class CertificadoException : Exception
    {
        private static readonly IDictionary<CertificadoExceptionTipo, string> Hash = new Dictionary<CertificadoExceptionTipo, string>();
        public CertificadoExceptionTipo? TipoErro { get; }

        public enum CertificadoExceptionTipo
        {
            NaoEhCpf,
            NaoEhCnpj,
            PessoaFisicaInvalida,
            PessoaJuridicaInvalida,
            ErroObterDadosPessoaFisica,
            ErroObterDadosPessoaJuridica,
            CertificadoExpirado,
            CertificadoInvalido,
            CadeiaInvalida,
            NaoEhIcpBrasil,
            NaoTemPessoaFisica
        }

        static CertificadoException()
        {
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.NaoEhCpf, "O certificado informado não é um e-CPF"));
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.NaoEhCnpj, "O certificado informado não é um e-CNPJ"));
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.PessoaFisicaInvalida, "Pessoa Física inválida"));
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.PessoaJuridicaInvalida, "Pessoa Jurídica inválida"));
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.ErroObterDadosPessoaFisica, "ErroMensagem ao obter dados da Pessoa Física"));
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.ErroObterDadosPessoaJuridica, "ErroMensagem ao Obter dados da Pessoa Jurídica"));
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.CertificadoExpirado, "Certificado está expirado ou ainda não é válido"));
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.CertificadoInvalido, "Certificado inválido"));
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.CadeiaInvalida, "Cadeia inválida"));
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.NaoEhIcpBrasil, "Não é ICP Brasil"));
            Hash.Add(new KeyValuePair<CertificadoExceptionTipo, string>(CertificadoExceptionTipo.NaoTemPessoaFisica, "Certificado não tem pessoa física"));
        }

        public CertificadoException()
        {
        }

        public CertificadoException(string message) : base(message)
        {
        }

        public CertificadoException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CertificadoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CertificadoException(CertificadoExceptionTipo tipoErro) : base(Hash[tipoErro])
        {
            TipoErro = tipoErro;
        }

        public CertificadoException(CertificadoExceptionTipo tipoErro, Exception ex) : base(Hash[tipoErro], ex)
        {
            TipoErro = tipoErro;
        }

        public static string GetErrorMessage(CertificadoExceptionTipo tipoErro)
        {
            return Hash[tipoErro];
        }
    }
}