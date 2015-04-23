using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.DynamicDataItem
{
    class RowItemComposite : StringItemComposite
    {
        public override void Add(object xoItem)
        {
            // If the type is correct
            if (xoItem.GetType() == typeof(DataItemComposite))
            {
                if (this.Schema != null)
                {
                    // Pass the schema down
                    ((DataItemComposite)xoItem).Schema = this.Schema;
                }
                else
                {
                    throw new Exception("No Schema initialised");
                }
            }

            base.Add(xoItem);
        }
    }
}
