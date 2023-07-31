using System;

namespace Database
{
    public class IsDbField : Attribute
    {
        public IsDbField(bool isTableKey = false, bool isJsonObject = false)
        {
            IsTableKey = isTableKey;
            IsJsonObject = isJsonObject;
        }

        public bool IsTableKey { get; }

        public bool IsJsonObject { get; }
    }
}
