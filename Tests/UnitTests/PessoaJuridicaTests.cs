using AutoFixture.Xunit2;
using FluentAssertions;
using Prodest.Certificado.ICPBrasil.Certificados;
using System;
using Xunit;

namespace UnitTests
{
    public class PessoaJuridicaTests
    {
        [Theory, AutoData]
        public void FactoryPessoaJuridica_DadosValidos_DeveFuncionar(
            string cnpj
            , string inss
            , string razaoSocial)
        {
            // act
            var result = new PessoaJuridica(cnpj, inss, razaoSocial);

            // assert
            result.Should().NotBeNull();
            result.Cnpj.Should().Be(cnpj);
            result.Inss.Should().Be(inss);
            result.RazaoSocial.Should().Be(razaoSocial);
        }

        [Theory]
        [InlineData("", "any")]
        [InlineData("any", "")]
        public void FactoryPessoaJuridica_SemDados_ShouldThrow(
                string cnpj
                , string razaoSocial
            )
        {
            // arrange

            // act
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AccessToDisposedClosure
            Action act = () => new PessoaJuridica(cnpj
                , "qualquerCoisa"
                , razaoSocial
            );

            // assert
            act.Should().Throw<CertificadoException>()
                .And.TipoErro.Should().Be(CertificadoException.CertificadoExceptionTipo.PessoaJuridicaInvalida);
        }
    }
}