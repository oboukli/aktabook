# Aktabook

[![Build and run unit and integration tests](https://github.com/oboukli/aktabook/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/oboukli/aktabook/actions/workflows/build-and-test.yml)
[![Code style check](https://github.com/oboukli/aktabook/actions/workflows/code-style-check.yml/badge.svg)](https://github.com/oboukli/aktabook/actions/workflows/code-style-check.yml)
[![CodeQL analysis](https://github.com/oboukli/aktabook/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/oboukli/aktabook/actions/workflows/codeql-analysis.yml)

[![Build Status](https://dev.azure.com/omarboukli/Aktabook/_apis/build/status/oboukli.aktabook?branchName=main)](https://dev.azure.com/omarboukli/Aktabook/_build/latest?definitionId=4&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=oboukli_aktabook&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=oboukli_aktabook)

An experimental work-in-progress book data aggregator API,
showcasing ASP.NET Core.

## Happy flow

The following sequence diagram shows the basic happy flow of Aktabook.

```mermaid
sequenceDiagram
autonumber

actor client as Client

participant api as Aktabook Public API Service (ASP.NET Core)
participant msgbroker as Message Broker (RabbitMQ)
participant msgproc as Message Processor (NServiceBus)
participant database as Database (SQL Server)

participant openlibrary as Open Library
Note right of openlibrary: External API

client->>api: POST Request book by ISBN
api-->>client: CREATED Request followup ID

api->>msgbroker: Send book information request message

msgbroker->msgproc: Read book information request message
activate msgproc

par
    msgproc->>database: Store request data
and
    msgproc->>openlibrary: GET book data
    openlibrary-->>msgproc: OK book data
end

msgproc->>database: Store book data
deactivate msgproc
```

## Showcases

The following is a partial list of standards, practices, software,
and ideas leveraged by this project:

- .NET 8.0
- .NET OpenAPI analyzers
- .NET static analyzers
- .NET Tools
- ASP.NET Core
- Azure Pipelines
- C# 12
- C# Script
- Clean Architecture
- Code test coverage with dotnet-coverage
- CodeQL
- Conventional Commits
- DevSkim
- dotnet security-scan
- EditorConfig
- EF Core
- FluentAssertions
- FluentValidation
- GitHub Actions
- Integration testing
- Locked mode NuGet
- Markdown
- Markdownlint
- MediatR
- Mermaid
- Migrations testing
- Modern code style
- NServiceBus
- NSubstitute
- Open Library API
- OpenAPI/Swaggar
- OpenTelemetry
- RabbitMQ
- ReSharper command line tools
- Service health checks
- SonarScanner
- SQL Server
- Strongly-typed approach
- Structured logging with Serilog
- Testing with a production-grade database engine
- Unit testing with xUnit.net
- xUnit.net Analyzers

## License

This software is released under an [MIT-style license](LICENSE).
Copyright © 2024 Omar Boukli-Hacene.

SPDX license identifier: MIT.

---

Made for the joy of it 🐳
