# Aktabook

[![Build and run unit and integration tests](https://github.com/oboukli/aktabook/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/oboukli/aktabook/actions/workflows/build-and-test.yml)
[![Code style check](https://github.com/oboukli/aktabook/actions/workflows/code-style-check.yml/badge.svg)](https://github.com/oboukli/aktabook/actions/workflows/code-style-check.yml)
[![CodeQL analysis](https://github.com/oboukli/aktabook/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/oboukli/aktabook/actions/workflows/codeql-analysis.yml)

[![Build Status](https://dev.azure.com/omarboukli/Aktabook/_apis/build/status/oboukli.aktabook?branchName=main)](https://dev.azure.com/omarboukli/Aktabook/_build/latest?definitionId=4&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=oboukli_aktabook&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=oboukli_aktabook)

An experimental work-in-progress book data aggregator API.

## Happy flow

The following sequence diagram shows the (work-in-progress)
basic happy flow of Aktabook.

```mermaid
sequenceDiagram
autonumber

actor client as Client

participant api as Aktabook Public API Service
participant msgbroker as Message Broker (RbbitMQ)
participant msgproc as Message Processor (.NET)
participant database as Database (SQL Server)

participant openlibrary as Open Library
Note right of openlibrary: External API

client->>api: POST Request book by ISBN
api-->>client: CREATED Request followup ID

api->>msgbroker: Request book information

msgbroker->msgproc: Read message
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

## Show cases

- C# 10
- C# Script
- .NET 6.0
- .NET Tools
- ASP.NET Core
- EF Core
- NServiceBus
- SQL Server client
- RabbitMQ client
- GitHub Actions
- Strongly-typed approach
- Testing with a production-grade database engine
- Unit testing
- Integration testing
- MediatR
- FluentValidation
- FluentAssertions
- Code test coverage
- SonarScanner
- Service health checks
- Locked mode NuGet
- Clean Architecture
- Modern code style
- OpenTelemetry
- Serilog
- Open Library API
- Conventional Commits
- DevSkim
- OpenApi/Swaggar
- Markdown
- OpenAPI analyzers

## License

This software is released under an [MIT-style license](LICENSE).
Copyright © 2022 Omar Boukli-Hacene.

SPDX license identifier: MIT.

---

Made for the joy of it 🐳
