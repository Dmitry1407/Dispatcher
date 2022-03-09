using System;

namespace Core.model.core
{
    public class Parent
    {
        public Int32 ParentID { get; set; }
        public ParentType ParentType { get; set; }

        public Parent(Int32 parentID, ParentType parentType)
        {
            ParentID = parentID;
            ParentType = parentType;
        }
    }
}
