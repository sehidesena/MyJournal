using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.SemanticKernel.Memory;

namespace Mentalfull.Services.Ai
{
    public class SimplePineconeMemoryStore : IMemoryStore
    {
        private readonly HttpClient _httpClient;
        private readonly string _host;
        private readonly string _apiKey;

        public SimplePineconeMemoryStore(string host, string apiKey)
        {
            _host = host.TrimEnd('/');
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Api-Key", _apiKey);
        }

        public async Task<string> UpsertAsync(string collectionName, MemoryRecord record, CancellationToken cancellationToken = default)
        {
            var vector = record.Embedding.ToArray();
            var metadata = new Dictionary<string, object>
            {
                { "text", record.Metadata.Text },
                { "description", record.Metadata.Description },
                { "external_source_name", record.Metadata.ExternalSourceName },
                { "id", record.Metadata.Id }
            };

            var payload = new
            {
                vectors = new[]
                {
                    new
                    {
                        id = record.Metadata.Id,
                        values = vector,
                        metadata = metadata
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"{_host}/vectors/upsert", payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            return record.Metadata.Id;
        }

        public async IAsyncEnumerable<string> UpsertBatchAsync(string collectionName, IEnumerable<MemoryRecord> records, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var record in records)
            {
                yield return await UpsertAsync(collectionName, record, cancellationToken);
            }
        }

        public async Task<MemoryRecord?> GetAsync(string collectionName, string key, bool withEmbedding = false, CancellationToken cancellationToken = default)
        {
            return null;
        }

        public async IAsyncEnumerable<MemoryRecord> GetBatchAsync(string collectionName, IEnumerable<string> keys, bool withEmbedding = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            yield break;
        }

        public async Task RemoveAsync(string collectionName, string key, CancellationToken cancellationToken = default)
        {
        }

        public async Task RemoveBatchAsync(string collectionName, IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
        }

        public async IAsyncEnumerable<(MemoryRecord, double)> GetNearestMatchesAsync(
            string collectionName,
            ReadOnlyMemory<float> embedding,
            int limit,
            double minRelevanceScore = 0,
            bool withEmbedding = false,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                vector = embedding.ToArray(),
                topK = limit,
                includeMetadata = true,
                includeValues = withEmbedding
            };

            var response = await _httpClient.PostAsJsonAsync($"{_host}/query", payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            
            var result = JsonSerializer.Deserialize<PineconeQueryResponse>(responseBody);

            if (result?.Matches != null)
            {
                foreach (var match in result.Matches)
                {
                    if (match.Score >= minRelevanceScore)
                    {
                        var metadata = new MemoryRecordMetadata(
                            isReference: true,
                            id: match.Id,
                            text: match.Metadata?.Text ?? "",
                            description: match.Metadata?.Description ?? "",
                            externalSourceName: match.Metadata?.ExternalSourceName ?? "",
                            additionalMetadata: ""
                        );

                        var record = new MemoryRecord(metadata, embedding, null);
                        yield return (record, match.Score);
                    }
                }
            }
        }

        public async Task<(MemoryRecord, double)?> GetNearestMatchAsync(string collectionName, ReadOnlyMemory<float> embedding, double minRelevanceScore = 0, bool withEmbedding = false, CancellationToken cancellationToken = default)
        {
            var results = GetNearestMatchesAsync(collectionName, embedding, 1, minRelevanceScore, withEmbedding, cancellationToken);
            await foreach (var result in results)
            {
                return result;
            }
            return null;
        }

        public Task CreateCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<string> GetCollectionsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            yield return "mentalfull-memory";
        }

        public Task<bool> DoesCollectionExistAsync(string collectionName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        public Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    public class PineconeQueryResponse
    {
        [JsonPropertyName("matches")]
        public List<PineconeMatch> Matches { get; set; }
    }

    public class PineconeMatch
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("metadata")]
        public PineconeMetadata Metadata { get; set; }
    }

    public class PineconeMetadata
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("external_source_name")]
        public string ExternalSourceName { get; set; }
    }
}
