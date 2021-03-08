using AutoFixture.Xunit2;
using FluentAssertions;
using Prodest.Certificado.ICPBrasil.Certificados;
using System;
using System.Globalization;
using Xunit;

namespace UnitTests
{
    public class PessoaFisicaTests
    {
        [Theory, AutoData]
        public void FactoryPessoaFisica_DadosValidos_DeveFuncionar(string nome, string email)
        {
            // arrange
            const string dados = "270419710170392570000000000000000000001234567SPTCES";

            // act
            var result = new PessoaFisica(nome, dados, email);

            // assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(nome);
            result.DataNascimento.Should().Be(DateTime.ParseExact("27041971", "ddMMyyyy", CultureInfo.InvariantCulture));
            result.Cpf.Should().Be("01703925700");
            result.Email.Should().Be(email);

            result.Rg.Should().Be("1234567");
            result.OrgaoExpedidor.Should().Be("SPTCES");
        }

        [Fact]
        public void FactoryPessoaFisica_SemNome_ShouldThrow()
        {
            // arrange

            // act
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AccessToDisposedClosure
#pragma warning disable CA1806 // Do not ignore method results
            Action act = () => new PessoaFisica(
                string.Empty
                , "qualquerCoisa"
                , "qualquerEmail");
#pragma warning restore CA1806 // Do not ignore method results

            // assert
            act.Should().Throw<CertificadoException>()
                .And.TipoErro.Should().Be(CertificadoException.CertificadoExceptionTipo.PessoaFisicaInvalida);
        }

        [Fact]
        public void FactoryPessoaFisica_SemDados_ShouldThrow()
        {
            // arrange

            // act
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AccessToDisposedClosure
#pragma warning disable CA1806 // Do not ignore method results
            Action act = () => new PessoaFisica(
                "qualquerNome"
                , string.Empty
                , "qualquerEmail");
#pragma warning restore CA1806 // Do not ignore method results

            // assert
            act.Should().Throw<CertificadoException>()
                .And.TipoErro.Should().Be(CertificadoException.CertificadoExceptionTipo.PessoaFisicaInvalida);
        }
    }
}