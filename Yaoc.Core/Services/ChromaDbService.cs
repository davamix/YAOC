using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaoc.Core.Data.ChromaDb;

namespace Yaoc.Core.Services;

public interface IChromaDbService {
    Task<bool> TestConnection(string chromaDbServerUrl);
}

public class ChromaDbService : IChromaDbService {

    private readonly IChromaDbProvider _chromaDbProvider;
    private readonly IConfiguration _configuration;

    public ChromaDbService(IChromaDbProvider chromaDbProvider,
        IConfiguration configuration) {
        _chromaDbProvider = chromaDbProvider;
        _configuration = configuration;
    }

    public Task<bool> TestConnection(string chromaDbServerUrl) {
        _configuration["AppSettings:ChromaDb:Server"] = chromaDbServerUrl;

        return _chromaDbProvider.TestConnection();
    }
}
