# Prodest.Certificados.ICPBrasil

Biblioteca para manipula��o de certificados digitais ICP-Brasil em .NET

baseado no c�digo dispon�vel em: https://github.com/pbozzi/certificado-net

## Instala��o

```
PM> Install-Package Prodest.Certificados.ICPBrasil
```

## Uso b�sico

```csharp
using Prodest.Certificado.ICPBrasil.Certificados;
using System.Security.Cryptography.X509Certificates;

X509Certificate2 certificado;
var options = new CertificadoDigitalOptions();

var icpBrasil = CertificadoDigital.Processar(certificado, options);
```

## Op��es

```csharp
var options = new CertificadoDigitalOptions(){
    ValidarCadeia = false, // Valor default true
    ValidarRevogacao = false, // Valor default true
    ValidarRaizConfiavel = false // Valor default true
};
```

Caso voc� n�o queira validar a cadeia do certificado sendo analisado, voc� pode usar a op��o **ValidarCadeia**. Essa op��o s� deve ser desligada quando est� sendo usado um certificado n�o IcpBrasil e queremos analisar se a estrutura de e-Cpf e e-Cnpj est� funcionando de maneira correta. As outras op��es n�o s�o avaliadas caso essa valida��o seja desligada.

A op��o de **ValidarRevogacao** quando desligada usa um RevocationMode = X509RevocationMode.NoCheck e portanto n�o verifica se os certificados da cadeia sendo analisada foram revogados. Quando a op��o est� ligada o RevocationFlag � X509RevocationFlag.EntireChain.

A �ltima op��o **ValidarRaizConfiavel** verifica se a raiz inicial � um dos certificados raiz do ICP-Brasil. Esse certificado deve estar instalado na m�quina que est� fazendo essa verifica��o.