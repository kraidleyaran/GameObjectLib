using System;

namespace GameObjectLib
{
    [Serializable]
    public class Property
    {
        public Property(PropertyType type, string name, string value)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        public Property(PropertyType type, string name)
        {
            Type = type;
            Name = name;
            Value = "";
        }

        public PropertyType Type;
        public string Value;
        public string Name;

        public Property CloneProperty()
        {
            return new Property(Type, Name, Value);
        }
    }
}