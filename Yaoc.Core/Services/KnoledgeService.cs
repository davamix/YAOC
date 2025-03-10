using Yaoc.Core.Data.ChromaDb;
using Yaoc.Core.Entities.ChromaDb;
using Yaoc.Core.Models;

namespace Yaoc.Core.Services;

public interface IKnowledgeService {
    Task<KnowledgeCollection> SaveCollection(KnowledgeCollection collection);
    Task<List<KnowledgeCollection>> GetCollections();
    Task DeleteCollection(string name);
}

public class KnoledgeService : IKnowledgeService {
    private readonly IChromaDbProvider _chromaDbProvider;

    public KnoledgeService(IChromaDbProvider chromaDbProvider) {
        _chromaDbProvider = chromaDbProvider;
    }

    public async Task<KnowledgeCollection> SaveCollection(KnowledgeCollection collection) {
        var entity = new Collection {
            Id = collection.Id,
            Name = GetEntityName(collection.Name),
            Metadata = new Dictionary<string, object> { { "Name", collection.Name } }
        };

        if (string.IsNullOrEmpty(collection.Id)) {
            var createResponse = await _chromaDbProvider.CreateCollection(entity);

            return CreateKnowledgeCollection(createResponse.Collection);
        }

        await _chromaDbProvider.UpdateCollection(entity);

        return collection;
    }

    public async Task<List<KnowledgeCollection>> GetCollections() {
        var response = await _chromaDbProvider.GetCollections();

        return response.Collections.Select(CreateKnowledgeCollection).ToList();
    }

    public async Task DeleteCollection(string name) {
        var entity = new Collection {
            Name = GetEntityName(name)
        };

        await _chromaDbProvider.DeleteCollection(entity);
    }

    private string GetEntityName(string name) =>
        name?
        .Replace(" ", "_")
        .Replace("/", "_");

    private KnowledgeCollection CreateKnowledgeCollection(Collection collection) =>
        new KnowledgeCollection {
            Id = collection.Id,
            Name = collection.Metadata.TryGetValue("Name", out object? value) ? value.ToString() : collection.Name
        };
}
