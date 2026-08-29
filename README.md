[![](https://img.shields.io/nuget/v/soenneker.maf.pool.openai.azure.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.maf.pool.openai.azure/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.maf.pool.openai.azure/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.maf.pool.openai.azure/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.maf.pool.openai.azure.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.maf.pool.openai.azure/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.maf.pool.openai.azure/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.maf.pool.openai.azure/actions/workflows/codeql.yml)

# Soenneker.Maf.Pool.OpenAI.Azure

Provides Azure OpenAI-specific registration extensions for `IMafPool`, enabling integration via Microsoft Agent Framework.

## Install

```bash
dotnet add package Soenneker.Maf.Pool.OpenAI.Azure
```

## Quick start

```csharp
using Soenneker.Maf.Pool.OpenAI.Azure;

IMafPool pool = /* obtain from your application */;
await pool.AddAzureOpenAI("value", "value", "value", "value", "value", default);
```

Registers an Azure OpenAI model in the agent pool with optional rate/token limits.

## What you get

- `MafPoolAzureOpenAIExtension` — Provides Azure OpenAI-specific registration extensions for `IMafPool`, enabling integration via Microsoft Agent Framework.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `MafPoolAzureOpenAIExtension.AddAzureOpenAI(pool, poolId, key, deploymentName, apiKey, endpoint, rps, rpm, rpd, tokensPerDay, instructions, cancellationToken)` | Registers an Azure OpenAI model in the agent pool with optional rate/token limits. | A task that completes when the azure openai addition is complete. |
| `MafPoolAzureOpenAIExtension.RemoveAzureOpenAI(pool, poolId, key, cancellationToken)` | Unregisters an Azure OpenAI model from the agent pool and removes the associated cache entry. | True if the entry existed and was removed; false if it was not present. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
