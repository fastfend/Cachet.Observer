using System;
using System.Collections.Generic;
using System.Text;

namespace CachetObserver
{
    interface ICachetObserverService
    {
        bool KeyVerifed { get; }
        bool Online { get; }
        void RunChecks();
        void SetMetrcs();
        void ReportIncidents();
    }
}
