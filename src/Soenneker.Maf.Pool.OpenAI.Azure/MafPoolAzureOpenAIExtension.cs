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
    /// <param name="pool">Pool that supplies the reusable resource.</param>
    /// <param name="poolId">Identifier of the target pool.</param>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="deploymentName">Name of the deployment to target.</param>
    /// <param name="apiKey">API key used to authenticate the request.</param>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="rps">Optional requests-per-second limit.</param>
    /// <param name="rpm">Optional requests-per-minute limit.</param>
    /// <param name="rpd">Optional requests-per-day limit.</param>
    /// <param name="tokensPerDay">Optional daily token limit.</param>
    /// <param name="instructions">Instructions supplied to the model or processor.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the azure openai addition is complete.</returns>
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
    /// <param name="pool">Pool that supplies the reusable resource.</param>
    /// <param name="poolId">Identifier of the target pool.</param>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>True if the entry existed and was removed; false if it was not present.</returns>
    public static ValueTask<bool> RemoveAzureOpenAI(this IMafPool pool, string poolId, string key, CancellationToken cancellationToken = default)
    {
        return pool.Remove(poolId, key, cancellationToken);
    }
}
