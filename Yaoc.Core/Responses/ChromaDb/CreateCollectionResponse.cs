﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaoc.Core.Entities.ChromaDb;

namespace Yaoc.Core.Responses.ChromaDb;

public class CreateCollectionResponse
{
    public Collection Collection { get; set; } = new();
}
