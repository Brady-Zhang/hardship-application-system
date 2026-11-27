# Hardship Application System

[![CI](https://github.com/Brady-Zhang/hardship-application-system/actions/workflows/ci.yml/badge.svg)](https://github.com/Brady-Zhang/hardship-application-system/actions/workflows/ci.yml)

A small full-stack application for recording and updating financial hardship
applications: a React/TypeScript front end, an ASP.NET Core Web API, and a
relational database.

> **Status:** scaffold. The domain, API and UI arrive in the branches that follow.

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) (`dotnet --version`)
- [Node.js 22+](https://nodejs.org) (`node --version`)

Nothing else — no Docker, no database server to install. See
[Design decisions](#design-decisions) for why.

## Running it

Two terminals.

```bash
# 1. API — http://localhost:5080 (OpenAPI at /openapi/v1.json)
dotnet run --project src/HardshipApp.Api
```

```bash
# 2. Web — http://localhost:5173
cd web
npm install
npm run dev
```

The Vite dev server proxies `/api` to the API, so the browser only ever talks to
one origin and CORS never enters the picture.

## Running the tests

```bash
dotnet test                                      # everything
dotnet test tests/HardshipApp.UnitTests          # fast, no database
dotnet test tests/HardshipApp.IntegrationTests   # the API over a real database
```

The test projects are split by **kind of test**, not by which project they cover:
the unit tests run in milliseconds with no infrastructure, the integration tests
stand up the API and a real database. Naming them after the production project
would have hidden that difference behind `*.Tests`.

```bash
cd web
npm run typecheck              # tsc, no emit
npm test                       # Vitest + React Testing Library
```

CI runs exactly these on every push and pull request.

## Project structure

```
src/
  HardshipApp.Models/         entities and domain types     (no dependencies)
  HardshipApp.Repositories/   EF Core DbContext, data access  → Models
  HardshipApp.Services/       business rules                  → Models, Repositories
  HardshipApp.Api/            controllers, DTOs, validation, error handling → all of the above
tests/
  HardshipApp.UnitTests/        business rules, validators, mapping — no I/O
  HardshipApp.IntegrationTests/ the API end to end, over a real database
web/                          React + TypeScript (Vite)
```

The layering is enforced by project references rather than by convention:
`HardshipApp.Repositories` cannot reference `HardshipApp.Api`, so the dependency
direction cannot rot as the code grows.

## Design decisions

_Filled in as the implementation lands. Already settled:_

- **The frontend's API types are generated from the API's OpenAPI document**, not
  hand-written, so the contract exists once. `npm run typecheck` in CI fails if a
  change to a DTO is not reflected at the call sites.
- **Warnings are errors in CI** (`-warnaserror`, `tsc`, `oxlint`). An unused
  variable or a nullable violation should not need a review comment.
- **TypeScript is pinned to 5.x** in `web`: `openapi-typescript` currently peers on
  `typescript@^5`, and the generated contract matters more than being on the newest
  compiler.
- **The vulnerable transitive `Microsoft.OpenApi` the project template pulled in is
  pinned forward** to a patched version — the template ships 2.0.0, which carries a
  known high-severity advisory.

## What I'd add for production

_To be written up with the final submission._
