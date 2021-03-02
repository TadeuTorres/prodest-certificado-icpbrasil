# Prodest.HealthChecks

Biblioteca para configura��es de health check dos sistemas no ambiente PRODEST.

**ASP.NET Core** vers�es suportadas: 5.0 e 3.1

# Se��es

## HealthChecks

- [Health Checks](#Health-Checks)
- [Health Checks Push Results](#HealthCheck-push-results)
- [Arquivo de Health](#Arquivo-de-health)
- [Vari�vel de ambiente](#Variavel-de-ambiente)

## Manuten��o

- [Modulo de Manuten��o](#Modulo-de-manutencao)

## Health Checks

Explica��o de como configurar o m�dulo de healthchekc da Microsoft e como "plugar" nessa biblioteca

> N�s suportamos netcoreapp 3.1. Por favor use os pacotes das vers�es 3.1.X para apontar para diferentes vers�es.

```PowerShell
Install-Package Prodest.HealthChecks
```

Once the package is installed you can add the HealthCheck using the **AddXXX** IServiceCollection extension methods.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHealthChecks()
        .AddSqlServer(Configuration["Data:ConnectionStrings:Sql"])
        .AddProdest();
}
```

Cada registro de HealthCheck tamb�m suporta name, tags, failure status e outros par�metros
    opcionais.

```csharp
public void ConfigureServices(IServiceCollection services)
{
      services.AddHealthChecks()
          .AddSqlServer(
              connectionString: Configuration["Data:ConnectionStrings:Sql"],
              healthQuery: "SELECT 1;",
              name: "sql",
              failureStatus: HealthStatus.Degraded,
              tags: new string[] { "db", "sql", "sqlserver" });
}
```

## HealthCheck push results

Explicar como funciona o modelo de push dessa biblioteca

```csharp
services.AddHealthChecks()
        .AddSqlServer(connectionString: Configuration["Data:ConnectionStrings:Sample"])
        .AddCheck<RandomHealthCheck>("random")
        .AddProdestHealthChecks()
        .AddPush("redis?");
```

## Arquivo de Health

Explicar como funciona o arquivo de health.

## Vari�vel de ambiente

Explicar como funciona o arquivo de health.

## Modulo de Manuten��o

> Optionally, Qualer coment�rio opcional

**Importante:** Bl� bl� bl�.

