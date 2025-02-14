using Yaoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaoc.Data;

public interface IStorageProvider
{
    Task SaveConversation(Conversation conversation);
    Task SaveConversations(IEnumerable<Conversation> conversations);
    Task<IEnumerable<Conversation>> LoadConversations();
}
