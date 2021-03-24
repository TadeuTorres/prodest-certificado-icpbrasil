using FluentAssertions;
using Prodest.Certificado.ICPBrasil.Certificados;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnitTests.Context;
using Xunit;
using static UnitTests.Context.CertificadoDigitalTestsContext;

namespace UnitTests
{
    public class CertificadoDigitalTests
    {
        [Fact]
        public void CertificadoDigital_ComECnpjEmBytes_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.ECnpjString);
            var options = new CertificadoDigitalOptions()
            {
                ValidarCadeia = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
            result.Erro.Should().BeFalse();
        }

        [Fact]
        public void CertificadoDigital_ComECpfEmBytes_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.ECpfString);
            var options = new CertificadoDigitalOptions();

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
            result.Erro.Should().BeFalse();
        }

        [Fact]
        public void CertificadoDigital_ComECpfValido_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.FileECpfValido);
            var options = new CertificadoDigitalOptions();

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
            result.Erro.Should().BeFalse();
        }

        [Fact]
        public void CertificadoDigital_ComECpfSemValidarCadeia_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.FileECpfValido);
            var options = new CertificadoDigitalOptions()
            {
                ValidarCadeia = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
            result.Erro.Should().BeFalse();
        }

        [Fact]
        public void CertificadoDigital_ComECpfSemValidarRevogacao_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.FileECpfValido);
            var options = new CertificadoDigitalOptions()
            {
                ValidarRevogacao = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
            result.Erro.Should().BeFalse();
        }

        [Fact]
        public void CertificadoDigital_ComECnpjValido_DeveFuncionar()
        {
            // arrange
            using var certificado = new X509Certificate2(CnpjCerPath);
            var options = new CertificadoDigitalOptions()
            {
                ValidarCadeia = true
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
            result.Erro.Should().BeFalse();
        }

        [Fact]
        public void CertificadoDigital_ComECpfExpiradoSemValidarExpiracao_DeveFuncionar()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.FileECpfExpirado);
            var options = new CertificadoDigitalOptions()
            {
                ValidarRevogacao = false
            };

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.Should().NotBeNull();
            result.Erro.Should().BeFalse();
        }

        [Fact]
        public void CertificadoDigital_CertificadoNaoIcpSemValidarCadeia_DeveFuncionar()
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
            result.Erro.Should().BeFalse();
            result.CadeiaValida.Should().BeFalse();
            result.IcpBrasil.Should().BeFalse();
            result.PessoaFisica.Should().BeNull();
            result.PessoaJuridica.Should().BeNull();
            result.RawCertDataString.Should().BeEquivalentTo(certificado.GetRawCertDataString());
        }

        #region Erros

        [Fact]
        public void CertificadoDigital_CertificadoInvalido_DeveRetornarTipoInvalido()
        {
            // arrange
            using var certificado = new X509Certificate2();
            var options = new CertificadoDigitalOptions();

            // act
            var result = CertificadoDigital.Processar(certificado!, options);

            // assert
            result.TipoCertificado.Should().Be(TipoCertificado.Invalido);
            result.Erro.Should().BeTrue();
            result.ErroMensagem.Should().NotBeNullOrEmpty();
            result.PessoaJuridica.Should().BeNull();
            result.PessoaFisica.Should().BeNull();
            result.RawCertDataString.Should().BeNull();
            result.CadeiaValida.Should().BeFalse();
            result.IcpBrasil.Should().BeFalse();
        }

        [Fact]
        public void CertificadoDigital_ComECpfExpirado_DeveRetornarComErro()
        {
            // arrange
            var certificado = ObterCertificado(CertificadoTipo.FileECpfExpirado);
            var options = new CertificadoDigitalOptions();

            // act
            var result = CertificadoDigital.Processar(certificado, options);

            // assert
            result.TipoCertificado.Should().Be(TipoCertificado.Invalido);
            result.Erro.Should().BeTrue();
            result.ErroMensagem.Should().Be(CertificadoException.GetErrorMessage(CertificadoException.CertificadoExceptionTipo.CertificadoExpirado));
            result.PessoaJuridica.Should().BeNull();
            result.PessoaFisica.Should().BeNull();
            result.CadeiaValida.Should().BeFalse();
            result.IcpBrasil.Should().BeFalse();
        }

        [Fact]
        public void CertificadoDigital_CertificadoNaoIcp_DeveRetornarComErro()
        {
            // arrange
            using var certificado = new X509Certificate2(SelfSignedPath, SelfSignedPassword, X509KeyStorageFlags.EphemeralKeySet);
            var options = new CertificadoDigitalOptions();

            // act
            var result = CertificadoDigital.Processar(certificado!, options);

            // assert
            result.TipoCertificado.Should().Be(TipoCertificado.Invalido);
            result.Erro.Should().BeTrue();
            result.ErroMensagem.Should().NotBeNullOrEmpty();
            result.PessoaJuridica.Should().BeNull();
            result.PessoaFisica.Should().BeNull();
            result.RawCertDataString.Should().NotBeNullOrEmpty();
            result.CadeiaValida.Should().BeFalse();
            result.IcpBrasil.Should().BeFalse();
        }

        [Fact]
        public void CertificadoDigital_SemCertificadoBuffer_ShouldThrow()
        {
            var options = new CertificadoDigitalOptions();

            // act
            Action act = () => CertificadoDigital.Processar(buffer: null!, options);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .And.Message.Should().Contain("buffer");
        }

        [Fact]
        public void CertificadoDigital_SemCertificado_ShouldThrow()
        {
            var options = new CertificadoDigitalOptions();

            // act
            Action act = () => CertificadoDigital.Processar(certificado: null!, options);

            // assert
            act.Should().Throw<ArgumentNullException>()
                .And.Message.Should().Contain("certificado");
        }

        #endregion Erros

        [Fact]
        public void CertificadoDigital_ComListaTesteValidos_DeveFuncionar()
        {
            // arrange
            var (_, validos, _) = CertificadoDigitalTestsContext.GetListaParaValidar();
            var options = new CertificadoDigitalOptions();

            foreach (var file in validos)
            {
                if (file.EndsWith(".pdf"))
                {
                    var buffer = ObterCertificadoFromPdf(file);
                    using var certificado = new X509Certificate2(buffer);
                    // act
                    var result = CertificadoDigital.Processar(certificado, options);
                    // assert
                    result.Should().NotBeNull();
                    result.Erro.Should().BeFalse();
                }
                else
                {
                    using var certificado = new X509Certificate2(file);
                    // act
                    var result = CertificadoDigital.Processar(certificado, options);
                    // assert
                    result.Should().NotBeNull();
                    result.Erro.Should().BeFalse();
                }
            }
        }

        [Fact]
        public void CertificadoDigital_ComListaTesteExpiradosSemValidarRevogacao_DeveFuncionar()
        {
            // arrange
            var (expirados, _, _) = CertificadoDigitalTestsContext.GetListaParaValidar();
            var options = new CertificadoDigitalOptions { ValidarRevogacao = false };

            foreach (var file in expirados)
            {
                if (file.EndsWith(".pdf"))
                {
                    var buffer = ObterCertificadoFromPdf(file);
                    using var certificado = new X509Certificate2(buffer);
                    // act
                    var result = CertificadoDigital.Processar(certificado, options);
                    // assert
                    result.Should().NotBeNull();
                    result.Erro.Should().BeFalse();
                }
                else
                {
                    using var certificado = new X509Certificate2(file);
                    // act
                    var result = CertificadoDigital.Processar(certificado, options);
                    // assert
                    result.Should().NotBeNull();
                    result.Erro.Should().BeFalse();
                }
            }
        }

        [Fact]
        public void CertificadoDigital_ComListaTesteExpirados_DeveDarErro()
        {
            // arrange
            var (expirados, _, _) = CertificadoDigitalTestsContext.GetListaParaValidar();
            var options = new CertificadoDigitalOptions();

            foreach (var file in expirados)
            {
                if (file.EndsWith(".pdf"))
                {
                    var buffer = ObterCertificadoFromPdf(file);
                    using var certificado = new X509Certificate2(buffer);
                    // act
                    var result = CertificadoDigital.Processar(certificado, options);
                    // assert
                    result.Should().NotBeNull();
                    result.Erro.Should().BeTrue();
                }
                else
                {
                    using var certificado = new X509Certificate2(file);
                    // act
                    var result = CertificadoDigital.Processar(certificado, options);
                    // assert
                    result.Should().NotBeNull();
                    result.Erro.Should().BeTrue();
                }
            }
        }

        [Fact]
        public void CertificadoDigital_ComListaTesteInvalidosSemValidarCadeia_DeveFuncionar()
        {
            // arrange
            var (_, _, invalidos) = CertificadoDigitalTestsContext.GetListaParaValidar();
            var options = new CertificadoDigitalOptions { ValidarCadeia = false };

            foreach (var file in invalidos)
            {
                if (!file.EndsWith(".pfx")) continue;
                using var certificado = new X509Certificate2(file, PfxPassword, X509KeyStorageFlags.EphemeralKeySet);
                // act
                var result = CertificadoDigital.Processar(certificado, options);
                // assert
                result.Should().NotBeNull();
                result.Erro.Should().BeFalse();
            }
        }

        [Fact]
        public void CertificadoDigital_ComListaTesteInvalidos_DeveDarErro()
        {
            // arrange
            var (_, _, invalidos) = CertificadoDigitalTestsContext.GetListaParaValidar();
            var options = new CertificadoDigitalOptions { ValidarRevogacao = false, ValidarRaizConfiavel = false };

            foreach (var file in invalidos)
            {
                if (!file.EndsWith(".pfx")) continue;
                using var certificado = new X509Certificate2(file, PfxPassword, X509KeyStorageFlags.EphemeralKeySet);
                // act
                var result = CertificadoDigital.Processar(certificado, options);
                // assert
                result.Should().NotBeNull();
                result.Erro.Should().BeTrue();
            }
        }
    }
}