using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Yaoc.ViewModels;

public partial class KnowledgeViewModel : BaseViewModel {

    public ObservableCollection<string> Collections { get; set; } = new();
    public ObservableCollection<string> Documents { get; set; } = new();

    public KnowledgeViewModel() {
        AddCollections();
        AddDocuments();
    }

    private void AddCollections() {
        Collections.Add("Collection A");
        Collections.Add("Collection B");
        Collections.Add("Collection C");
    }

    private void AddDocuments() {
        Documents.Add("Document A");
        Documents.Add("Document B");
        Documents.Add("Document C");
    }
}
