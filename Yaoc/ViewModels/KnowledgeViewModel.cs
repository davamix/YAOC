using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Yaoc.Core.Models;
using Yaoc.Core.Services;

namespace Yaoc.ViewModels;

public partial class KnowledgeViewModel : BaseViewModel {
    private readonly IKnowledgeService _knowledgeService;

    public ObservableCollection<KnowledgeCollection> Collections { get; set; } = new();
    public ObservableCollection<string> Documents { get; set; } = new();

    [ObservableProperty]
    private KnowledgeCollection _selectedCollection;

    public KnowledgeViewModel(IKnowledgeService knowledgeService) {
        _knowledgeService = knowledgeService;

        //AddCollections();
        AddDocuments();

        Task.WhenAll(
            Task.Run(LoadCollections));
    }

    private async Task LoadCollections() {
        Collections.Clear();

        //Collections.Add(CreateTestCollection());
        //Collections.Add(CreateTestCollection());
        //Collections.Add(CreateTestCollection());


        var collections = await _knowledgeService.GetCollections();

        foreach (var c in collections) {
            Collections.Add(c);
        }
    }

    private KnowledgeCollection CreateTestCollection() {
        Random random = new Random();
        return new KnowledgeCollection {
            Id = Guid.NewGuid().ToString(),
            Name = $"Collection {random.Next(50)}"
        };
    }


    private void AddDocuments() {
        Documents.Add("Document A");
        Documents.Add("Document B");
        Documents.Add("Document C");
    }

    [RelayCommand]
    private async Task CreateCollection() {
        var collection = new KnowledgeCollection {
            Name = DateTime.Now.ToShortDateString()            
        };

        var newCollection = await _knowledgeService.SaveCollection(collection);

        if (newCollection != null) {
            Collections.Add(newCollection);
        }
    }

    [RelayCommand]
    private async Task ChangeCollectionName(string collectionName) {
        var toChange = Collections.First(x => x.Id == SelectedCollection.Id);
        toChange.Name = collectionName;

        _ = await _knowledgeService.SaveCollection(toChange);
    }

    [RelayCommand]
    private async Task DeleteCollection(KnowledgeCollection collection) {
        await _knowledgeService.DeleteCollection(collection.Name);

        Collections.Remove(collection);
    }
}
