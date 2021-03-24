# Prodest.Certificados.ICPBrasil

Biblioteca para manipulação de certificados digitais ICP-Brasil em .NET

baseado no código disponível em: https://github.com/pbozzi/certificado-net

## Instalação

```
PM> Install-Package Prodest.Certificados.ICPBrasil
```

## Uso básico

```csharp
using Prodest.Certificado.ICPBrasil.Certificados;
using System.Security.Cryptography.X509Certificates;

X509Certificate2 certificado;
var options = new CertificadoDigitalOptions();

var icpBrasil = CertificadoDigital.Processar(certificado, options);
```

## Opções

```csharp
var options = new CertificadoDigitalOptions(){
    ValidarCadeia = false, // Valor default true
    ValidarRevogacao = false, // Valor default true
    ValidarRaizConfiavel = false // Valor default true
};
```

Caso você não queira validar a cadeia do certificado sendo analisado, você pode usar a opção **ValidarCadeia**. Essa opção só deve ser desligada quando está sendo usado um certificado não IcpBrasil e queremos analisar se a estrutura de e-Cpf e e-Cnpj está funcionando de maneira correta. As outras opções não são avaliadas caso essa validação seja desligada.

A opção de **ValidarRevogacao** quando desligada usa um RevocationMode = X509RevocationMode.NoCheck e portanto não verifica se os certificados da cadeia sendo analisada foram revogados. Quando a opção está ligada o RevocationFlag é X509RevocationFlag.EntireChain.

A última opção **ValidarRaizConfiavel** verifica se a raiz inicial é um dos certificados raiz do ICP-Brasil. Esse certificado deve estar instalado na máquina que está fazendo essa verificação.