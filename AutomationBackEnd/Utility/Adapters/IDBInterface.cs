using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Utility.Adapters
{

    interface IDBInterface
    {
        DataTable getQueryAsDataTable(String xsQuery);
    }
}
