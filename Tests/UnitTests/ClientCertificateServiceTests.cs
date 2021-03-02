namespace UnitTests
{
    //    public class ClientCertificateServiceTests
    //    {
    //        [Theory, AutoData]
    //        public async Task GuardarCertificadoInfo_DadosValidos_DeveSalvarNoCache(
    //            string cpf
    //            , CertificadoProprietarioInfo certificadoProprietarioInfo)
    //        {
    //            // arrange
    //            var cacheProvider = Substitute.For<IDistributedCacheProvider>();
    //            var sut = new ClientCertificateService(cacheProvider, null!);

    //            // act
    //            await sut.GuardarCertificadoInfo(cpf, certificadoProprietarioInfo);

    //            // assert
    //#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    //            cacheProvider.Received().SetKeyAsync(cpf, Arg.Any<string>(), Arg.Any<TimeSpan>(), Arg.Any<string>());
    //#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    //        }

    //        [Theory, AutoData]
    //        public void GuardarCertificadoInfo_InfoNulo_ShouldThrow(
    //            string cpf)
    //        {
    //            // arrange
    //            var sut = new ClientCertificateService(null!, null!);

    //            // act
    //            Func<Task> act = async () => await sut.GuardarCertificadoInfo(cpf, null!);

    //            // assert
    //            act.Should().Throw<ArgumentNullException>();
    //        }

    //        [Theory]
    //        [InlineAutoData(null)]
    //        [InlineAutoData("")]
    //        public void GuardarCertificadoInfo_CpfNulo_ShouldThrow(
    //            string cpf
    //            , CertificadoProprietarioInfo certificadoProprietarioInfo)
    //        {
    //            // arrange
    //            var sut = new ClientCertificateService(null!, null!);

    //            // act
    //            Func<Task> act = async () => await sut.GuardarCertificadoInfo(cpf, certificadoProprietarioInfo);

    //            // assert
    //            act.Should().Throw<ArgumentException>();
    //        }

    //        [Theory, AutoData]
    //        public async Task ObterPessoaFisicaInfo_CpfExiste_DeveTrazerInfoDoCache(
    //            string cpf
    //            , CertificadoProprietarioInfo certificadoProprietarioInfo)
    //        {
    //            // arrange
    //            var cacheProvider = Substitute.For<IDistributedCacheProvider>();
    //            cacheProvider.GetKeyAsync(cpf, Arg.Any<string>())
    //                .Returns(JsonConvert.SerializeObject(certificadoProprietarioInfo));
    //            var sut = new ClientCertificateService(cacheProvider, null!);

    //            // act
    //            var resultado = await sut.ObterCertificadoProprietarioInfo(cpf);

    //            // assert
    //            resultado.Should().BeEquivalentTo(certificadoProprietarioInfo);
    //        }

    //        [Theory]
    //        [InlineData(null)]
    //        [InlineData("")]
    //        public void ObterPessoaFisicaInfo_CpfNulo_ShouldThrow(
    //            string cpf)
    //        {
    //            // arrange
    //            var sut = new ClientCertificateService(null!, null!);

    //            // act
    //            Func<Task> act = async () => await sut.ObterCertificadoProprietarioInfo(cpf);

    //            // assert
    //            act.Should().Throw<ArgumentException>();
    //        }

    //        [Fact]
    //        public async Task ObterPessoaFisicaInfo_ECnpjValido_DeveRetornarInfo()
    //        {
    //            // arrange
    //            using var certificado = new X509Certificate2(SiteReceitaPath);
    //            var cacheProvider = Substitute.For<IDistributedCacheProvider>();
    //            var certificateDateTimeService = Substitute.For<ICertificateDateTimeService>();
    //            certificateDateTimeService.GetUtcNow().Returns(new DateTime(2020, 06, 06));
    //            var sut = new ClientCertificateService(cacheProvider, certificateDateTimeService);

    //            // act
    //            var resultado = await sut.ObterCertificadoProprietarioInfo(certificado);

    //            // assert
    //            resultado.Should().NotBeNull();
    //            resultado.Cnpj.Should().NotBeNullOrEmpty();
    //            resultado.RazaoSocial.Should().NotBeNullOrEmpty();
    //        }

    //        [Fact]
    //        public async Task ObterPessoaFisicaInfo_ECpfValido_DeveRetornarInfo()
    //        {
    //            // arrange
    //            using var certificado = new X509Certificate2(ObterCertificado(CertificadoTipo.ECpf));
    //            var cacheProvider = Substitute.For<IDistributedCacheProvider>();
    //            var certificateDateTimeService = Substitute.For<ICertificateDateTimeService>();
    //            certificateDateTimeService.GetUtcNow().Returns(new DateTime(2019, 06, 06));
    //            var sut = new ClientCertificateService(cacheProvider, certificateDateTimeService);

    //            // act
    //            var resultado = await sut.ObterCertificadoProprietarioInfo(certificado);

    //            // assert
    //            resultado.Should().NotBeNull();
    //            resultado.Cnpj.Should().BeNullOrEmpty();
    //            resultado.RazaoSocial.Should().BeNullOrEmpty();
    //        }

    //        [Fact]
    //        public void ObterPessoaFisicaInfo_CertificadoNaoIcp_ShouldThrow()
    //        {
    //            // arrange
    //            using var certificado = new X509Certificate2(SelfSignedPath, SelfSignedPassword, X509KeyStorageFlags.EphemeralKeySet);
    //            var cacheProvider = Substitute.For<IDistributedCacheProvider>();
    //            var certificateDateTimeService = Substitute.For<ICertificateDateTimeService>();
    //            certificateDateTimeService.GetUtcNow().Returns(new DateTime(2019, 06, 06));
    //            var sut = new ClientCertificateService(cacheProvider, certificateDateTimeService);

    //            // act
    //            // ReSharper disable once AccessToDisposedClosure
    //            Func<Task> act = async () => await sut.ObterCertificadoProprietarioInfo(certificado);

    //            // assert
    //            act.Should().Throw<CertificadoException>()
    //                .And.TipoErro.Should().Be(CertificadoException.CertificadoExceptionTipo.CadeiaInvalida);
    //        }

    //        [Fact]
    //        public void ObterPessoaFisicaInfo_CertificadoExpirado_ShouldThrow()
    //        {
    //            // arrange
    //            using var certificado = new X509Certificate2(ObterCertificado(CertificadoTipo.ECpf));
    //            var cacheProvider = Substitute.For<IDistributedCacheProvider>();
    //            var certificateDateTimeService = Substitute.For<ICertificateDateTimeService>();
    //            certificateDateTimeService.GetUtcNow().Returns(new DateTime(2025, 06, 06));
    //            var sut = new ClientCertificateService(cacheProvider, certificateDateTimeService);

    //            // act
    //            // ReSharper disable once AccessToDisposedClosure
    //            Func<Task> act = async () => await sut.ObterCertificadoProprietarioInfo(certificado);

    //            // assert
    //            act.Should().Throw<CertificadoException>()
    //                .And.TipoErro.Should().Be(CertificadoException.CertificadoExceptionTipo.CertificadoExpirado);
    //        }
    //    }
}