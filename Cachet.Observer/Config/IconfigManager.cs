using System;
using System.Collections.Generic;
using System.Text;

namespace CachetObserver.Config
{
    public interface IConfigManager
    {
        Configuration Configuration { get; }
    }
}
