using System;
using System.ClientModel;
using System.Threading;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Soenneker.Maf.Dtos.Options;
using Soenneker.Maf.Pool.Abstract;

namespace Soenneker.Maf.Pool.OpenAI.Azure;

/// <summary>
/// Provides Azure OpenAI-specific registration extensions for <see cref="IMafPool"/>, enabling integration via Microsoft Agent Framework.
/// </summary>
public static class MafPoolAzureOpenAIExtension
{
    /// <summary>
    /// Registers an Azure OpenAI model in the agent pool with optional rate/token limits.
    /// </summary>
    public static ValueTask AddAzureOpenAI(this IMafPool pool, string poolId, string key, string deploymentName, string apiKey, string endpoint,
        int? rps = null, int? rpm = null, int? rpd = null, int? tokensPerDay = null, string? instructions = null,
        CancellationToken cancellationToken = default)
    {
        var options = new MafOptions
        {
            ModelId = deploymentName,
            Endpoint = endpoint,
            ApiKey = apiKey,
            RequestsPerSecond = rps,
            RequestsPerMinute = rpm,
            RequestsPerDay = rpd,
            TokensPerDay = tokensPerDay,
            AgentFactory = (opts, _) =>
            {
                var uri = new Uri(opts.Endpoint!, UriKind.Absolute);
                var azureClient = new AzureOpenAIClient(uri, new ApiKeyCredential(opts.ApiKey!));
                var chatClient = azureClient.GetChatClient(opts.ModelId!);
                IChatClient ichatClient = chatClient.AsIChatClient();
                AIAgent agent = ichatClient.AsAIAgent(instructions: instructions ?? "You are a helpful assistant.", name: opts.ModelId);
                return new ValueTask<AIAgent>(agent);
            }
        };

        return pool.Add(poolId, key, options, cancellationToken);
    }

    /// <summary>
    /// Unregisters an Azure OpenAI model from the agent pool and removes the associated cache entry.
    /// </summary>
    /// <returns>True if the entry existed and was removed; false if it was not present.</returns>
    public static ValueTask<bool> RemoveAzureOpenAI(this IMafPool pool, string poolId, string key, CancellationToken cancellationToken = default)
    {
        return pool.Remove(poolId, key, cancellationToken);
    }
}
