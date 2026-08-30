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

## Usage

```csharp
using Soenneker.Maf.Pool.OpenAI.Azure;
using Soenneker.Maf.Pool.Abstract;

await pool.AddAzureOpenAI(
    poolId: "chat",
    key: "azure-primary",
    deploymentName: "chat-production",
    apiKey: configuration["AZURE_OPENAI_API_KEY"]!,
    endpoint: configuration["AZURE_OPENAI_ENDPOINT"]!,
    rpm: 60,
    instructions: "Answer concisely.",
    cancellationToken: cancellationToken);

(AIAgent? agent, IMafPoolEntry? entry) =
    await pool.GetAvailable("chat", cancellationToken);
```

Use the Azure resource endpoint, such as `https://my-resource.openai.azure.com`, and the Azure deployment name—not the underlying model name.

## What you get

- `MafPoolAzureOpenAIExtension` — Provides Azure OpenAI-specific registration extensions for `IMafPool`, enabling integration via Microsoft Agent Framework.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `MafPoolAzureOpenAIExtension.AddAzureOpenAI(pool, poolId, key, deploymentName, apiKey, endpoint, rps, rpm, rpd, tokensPerDay, instructions, cancellationToken)` | Registers an Azure OpenAI model in the agent pool with optional rate/token limits. | A task that completes when the azure openai addition is complete. |
| `MafPoolAzureOpenAIExtension.RemoveAzureOpenAI(pool, poolId, key, cancellationToken)` | Unregisters an Azure OpenAI model from the agent pool and removes the associated cache entry. | True if the entry existed and was removed; false if it was not present. |

## Practical notes

- The agent is created lazily and reused until its entry is removed.
- Store the API key in a secret provider; the pool retains it in the entry options while the entry is registered.
- Omitted instructions default to `You are a helpful assistant.`
- Checkout consumes one request from the configured quota. `tokensPerDay` is not reconciled against Azure's actual token usage.
