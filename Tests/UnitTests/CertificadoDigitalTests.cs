using FluentAssertions;
using Prodest.Certificado.ICPBrasil.Certificados;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Xunit;
using static UnitTests.Context.CertificadoDigitalTestsContext;

namespace UnitTests
{
    public class CertificadoDigitalTests
    {
        [Fact]
        public void FactoryCertificadoDigital_ComECnpjEmBytes_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.ECnpj);
            var options = new CertificadoDigitalOptions()
            {
                ValidarCadeia = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void FactoryCertificadoDigital_ComECpf_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.ECpf);
            var options = new CertificadoDigitalOptions();

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void FactoryCertificadoDigital_ComECpfAles_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.FileAles);
            var options = new CertificadoDigitalOptions();

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void FactoryCertificadoDigital_ComECpfSemValidarCadeia_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.FileAles);
            var options = new CertificadoDigitalOptions()
            {
                ValidarCadeia = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void FactoryCertificadoDigital_ComECpfSemValidarRevogacao_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.FileAles);
            var options = new CertificadoDigitalOptions()
            {
                ValidarRevogacao = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void FactoryCertificadoDigital_ComECnpjEmCertificado_DeveFuncionar()
        {
            // arrange
            var certificadoBuffer = ObterCertificado(CertificadoTipo.ECnpj);
            using var certificado = new X509Certificate2(certificadoBuffer);
            var options = new CertificadoDigitalOptions()
            {
                ValidarCadeia = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void FactoryCertificadoDigital_ComECpfExpiradoSemValidarExpiracao_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.FileTrtExpirado);
            var options = new CertificadoDigitalOptions()
            {
                ValidarCadeia = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void FactoryCertificadoDigital_CertificadoNaoIcpSemValidarCadeia_DeveFuncionar()
        {
            // arrange
            File.Exists(SelfSignedPath).Should().BeTrue();
            using var certificado = new X509Certificate2(SelfSignedPath, SelfSignedPassword, X509KeyStorageFlags.EphemeralKeySet);
            var options = new CertificadoDigitalOptions()
            {
                ValidarCadeia = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado!, options);

            // assert
            result.TipoCertificado.Should().Be(TipoCertificado.Outro);
            result.CadeiaValida.Should().BeFalse();
            result.IcpBrasil.Should().BeFalse();
            result.PessoaFisica.Should().BeNull();
            result.PessoaJuridica.Should().BeNull();
            result.RawCertDataString.Should().BeEquivalentTo(certificado.GetRawCertDataString());
        }

        [Fact]
        public void FactoryCertificadoDigital_CertificadoInfoconv_DeveFuncionar()
        {
            // arrange
            using var certificado = new X509Certificate2(InfoconvPath);
            var options = new CertificadoDigitalOptions();

            // act
            var result = CertificadoDigital.Processar(certificado!, options);

            // assert
            result.Should().NotBeNull();
        }

        #region Exceptions

        [Fact]
        public void FactoryCertificadoDigital_ComECpfExpirado_NaoDeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.FileTrtExpirado);
            var options = new CertificadoDigitalOptions();

            // act
            Action act = () => CertificadoDigital.Processar(certificado, options);

            // assert
            act.Should().Throw<CertificadoException>()
                .And.TipoErro.Should().Be(CertificadoException.CertificadoExceptionTipo.CertificadoExpirado);
        }

        [Fact]
        public void FactoryCertificadoDigital_CertificadoNaoIcp_ShouldThrow()
        {
            // arrange
            using var certificado = new X509Certificate2(SelfSignedPath, SelfSignedPassword, X509KeyStorageFlags.EphemeralKeySet);
            var options = new CertificadoDigitalOptions();

            // act
            // ReSharper disable once AccessToDisposedClosure
            Action act = () => CertificadoDigital.Processar(certificado!, options);

            // assert
            act.Should().Throw<CertificadoException>()
                .And.TipoErro.Should().Be(CertificadoException.CertificadoExceptionTipo.CadeiaInvalida);
        }

        [Fact]
        public void FactoryCertificadoDigital_CertificadoInvalido_ShouldThrow()
        {
            // arrange
            using var certificado = new X509Certificate2();
            var options = new CertificadoDigitalOptions();

            // act
            // ReSharper disable once AccessToDisposedClosure
            Action act = () => CertificadoDigital.Processar(certificado!, options);

            // assert
            act.Should().Throw<CertificadoException>()
                .And.TipoErro.Should().Be(CertificadoException.CertificadoExceptionTipo.CertificadoInvalido);
        }

        [Fact]
        public void FactoryCertificadoDigital_SemCertificadoBuffer_ShouldThrow()
        {
            var options = new CertificadoDigitalOptions();

            // act
            Action act = () => CertificadoDigital.Processar(buffer: null!, options);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .And.Message.Should().Contain("buffer");
        }

        [Fact]
        public void FactoryCertificadoDigital_SemCertificado_ShouldThrow()
        {
            var options = new CertificadoDigitalOptions();

            // act
            Action act = () => CertificadoDigital.Processar(certificado: null!, options);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .And.Message.Should().Contain("certificado");
        }

        #endregion Exceptions

        [Fact]
        public void FactoryCertificadoDigital_ComArquivoTeste_DeveFuncionar()
        {
            // arrange
            var certificadoBuffer = ObterCertificado(CertificadoTipo.ArquivoTeste);
            using var certificado = new X509Certificate2(certificadoBuffer);
            var options = new CertificadoDigitalOptions()
            {
                ValidarCadeia = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
        }
    }
}