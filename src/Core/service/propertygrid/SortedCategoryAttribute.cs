using System;
using System.ComponentModel;

namespace Core.service.propertygrid
{
   public class SortedCategoryAttribute : CategoryAttribute
    {
        private const char NonPrintableChar = '\t';

        public SortedCategoryAttribute(string category, ushort categoryPos, ushort totalCategories)
            : base(category.PadLeft(category.Length + (totalCategories - categoryPos),
                        SortedCategoryAttribute.NonPrintableChar))
        {
        }
    }
}
