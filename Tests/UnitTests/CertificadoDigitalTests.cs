using FluentAssertions;
using NSubstitute;
using Prodest.Certificado.ICPBrasil.Certificados;
using Prodest.Certificado.ICPBrasil.Dates;
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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2021, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService)
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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2021, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService);

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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2021, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService);

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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2021, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService)
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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2021, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService)
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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2021, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService)
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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2019, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService)
            {
                ValidarExpiracao = false,
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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2019, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService)
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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2019, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService)
            {
                ValidarCadeia = false,
                ValidarExpiracao = false
            };

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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2019, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService);

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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2019, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService);

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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            dateTimeService.GetUtcNow().Returns(new DateTime(2019, 06, 06));
            var options = new CertificadoDigitalOptions(dateTimeService);

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
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            var options = new CertificadoDigitalOptions(dateTimeService);

            // act
            Action act = () => CertificadoDigital.Processar(buffer: null!, options);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .And.Message.Should().Contain("buffer");
        }

        [Fact]
        public void FactoryCertificadoDigital_SemCertificado_ShouldThrow()
        {
            var dateTimeService = Substitute.For<ICertificateDateTimeService>();
            var options = new CertificadoDigitalOptions(dateTimeService);

            // act
            Action act = () => CertificadoDigital.Processar(certificado: null!, options);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .And.Message.Should().Contain("certificado");
        }

        [Fact]
        public void CertificadoDigitalOptions_SemIDateTimeService_ShouldThrow()
        {
            // act
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CA1806 // Do not ignore method results
            Action act = () => new CertificadoDigitalOptions(null!);
#pragma warning restore CA1806 // Do not ignore method results

            // assert
            act.Should().Throw<ArgumentNullException>()
                .And.Message.Should().Contain("certificateDateTimeService");
        }

        #endregion Exceptions

        [Fact]
        public void FactoryCertificadoDigital_ComArquivoTeste_DeveFuncionar()
        {
            // arrange
            var certificadoBuffer = ObterCertificado(CertificadoTipo.ArquivoTeste);
            using var certificado = new X509Certificate2(certificadoBuffer);
            var dateTimeService = new CertificateDateTimeService();
            var options = new CertificadoDigitalOptions(dateTimeService)
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