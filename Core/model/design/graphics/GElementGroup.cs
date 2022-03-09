using System;
using System.Collections.Generic;

namespace Core.model.design.graphics
{
    public class GElementGroup : GElement
    {
        public IDictionary<Int32, GElement> ElementStorage { get; private set; }

        public GElementGroup()
        {
            ElementStorage = new Dictionary<Int32, GElement>();
        }

        public void AddElement(GElement element)
        {
            if (element != null)
            {
                ElementStorage[element.ID] = element;
            }
        }

        public void DeleteElement(GElement element)
        {
            if (element != null && ElementStorage.ContainsKey(element.ID))
            {
                ElementStorage.Remove(element.ID);
            }
        }
    }
}
