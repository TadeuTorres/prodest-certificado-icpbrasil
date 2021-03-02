# Prodest.HealthChecks

Biblioteca para configurações de health check dos sistemas no ambiente PRODEST.

**ASP.NET Core** versões suportadas: 5.0 e 3.1

# Seções

## HealthChecks

- [Health Checks](#Health-Checks)
- [Health Checks Push Results](#HealthCheck-push-results)
- [Arquivo de Health](#Arquivo-de-health)
- [Variável de ambiente](#Variavel-de-ambiente)

## Manutenção

- [Modulo de Manutenção](#Modulo-de-manutencao)

## Health Checks

Explicação de como configurar o módulo de healthchekc da Microsoft e como "plugar" nessa biblioteca

> Nós suportamos netcoreapp 3.1. Por favor use os pacotes das versões 3.1.X para apontar para diferentes versões.

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

Cada registro de HealthCheck também suporta name, tags, failure status e outros parâmetros
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

## Variável de ambiente

Explicar como funciona o arquivo de health.

## Modulo de Manutenção

> Optionally, Qualer comentário opcional

**Importante:** Blá blá blá.

