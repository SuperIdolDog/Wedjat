using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Helper
{
    public static class WorkOrderQueryHelper
    {
        public static event Action TriggerWorkOrderQuery;

        public static void PublishTriggerWorkOrderQuery()
        {
            TriggerWorkOrderQuery?.Invoke();
        }
    }
}
