using System;
using System.Collections;
using System.ComponentModel;

namespace Core.service.propertygrid
{
    public class PropertySorter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            // This override returns a list of properties in order
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
            ArrayList orderedProperties = new ArrayList();
            foreach (PropertyDescriptor pd in pdc)
            {
                Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
                if (attribute != null)
                {
                    // If the attribute is found, then create an pair object to hold it
                    PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
                }
                else
                {
                    // If no order attribute is specifed then given it an order of 0
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
                }
            }

            // Perform the actual order using the value PropertyOrderPair classes
            // implementation of IComparable to sort
            orderedProperties.Sort();

            // Build a string list of the ordered names
            ArrayList propertyNames = new ArrayList();
            foreach (PropertyOrderPair pop in orderedProperties)
            {
                propertyNames.Add(pop.Name);
            }

            // Pass in the ordered list for the PropertyDescriptorCollection to sort by
            return pdc.Sort((string[])propertyNames.ToArray(typeof(string)));
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        // Simple attribute to allow the order of a property to be specified
        public int Order { get; private set; }

        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }
    }

    public class PropertyOrderPair : IComparable
    {
        public string Name { get; private set; }
        private int order;

        public PropertyOrderPair(string name, int order)
        {
            this.Name = name;
            this.order = order;
        }

        public int CompareTo(object obj)
        {
            // Sort the pair objects by ordering by order value
            // Equal values get the same rank
            int otherOrder = ((PropertyOrderPair)obj).order;
            if (otherOrder == order)
            {
                // If order not specified, sort by name
                string otherName = ((PropertyOrderPair)obj).Name;
                return string.Compare(Name, otherName);
            }
            else if (otherOrder > order)
            {
                return -1;
            }
            return 1;
        }
    }
}
