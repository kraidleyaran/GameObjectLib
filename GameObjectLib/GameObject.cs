using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Messaging;
using System.Windows.Media;
using System.Drawing;
using BinaryImageLib;

namespace GameObjectLib
{
    [Serializable]
    public class GameObject
    {
        private readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>();
        private BinaryImage EditorImage;
        public GameObject(string name)
        {
            Name = name;
        }

        public GameObject(string name, List<Property> propList)
        {
            Name = name;
            AddManyProperty(propList);
        }

        public string Name { get; set; }
        public BinaryImage Image { get { return EditorImage; } }

        public bool DoesPropertyExist(string propName)
        {
            return _properties.ContainsKey(propName);
        }


        public bool AddProperty(Property property)
        {
            bool propertyExist = DoesPropertyExist(property.Name);
            if (propertyExist)
            {
                return false;
            }
            _properties.Add(property.Name, property.CloneProperty());
            return true;
        }

        public Dictionary<string, bool> AddManyProperty(List<Property> propList)
        {
            Dictionary<string, bool> returnResponse = propList.ToDictionary(property => property.Name, AddProperty);
            return returnResponse;
        }

        public Dictionary<string, bool> RemoveManyProperty(List<string> propList)
        {
            Dictionary<string, bool> returnResponse = propList.ToDictionary(propertyName => propertyName, RemoveProperty);
            return returnResponse;
        }

        public bool RemoveProperty(string propName)
        {
            bool propertyExist = DoesPropertyExist(propName);
            if (!propertyExist)
            {
                return false;
            }
            _properties.Remove(propName);
            return true;
        }

        public Property GetProperty(string propName)
        {
            bool propertyExist = DoesPropertyExist(propName);
            if (!propertyExist)
            {
                return new Property(PropertyType.Error, propName, "Property does not exist");
            }
            Property property;
            _properties.TryGetValue(propName, out property);
            // ReSharper disable once PossibleNullReferenceException -- Already check for null reference and return if with specific string if so - see line 46
            return property;
        }

        public bool SetProperty(string propName, string propValue, PropertyType propType)
        {
            bool propertyExist = DoesPropertyExist(propName);
            if (!propertyExist)
            {
                return false;
            }

            _properties[propName].Value = propValue;
            _properties[propName].Type = propType;
            return true;
        }

        public Response ReceiveMessage(Message newMessage)
        {
            bool doesPropertyExist = DoesPropertyExist(newMessage.Property);
            PropertyType messageType = PropertyType.String;
            switch (newMessage.PropType)
            {
                case PropType.Bool:
                    messageType = PropertyType.Bool;
                    break;
                case PropType.Number:
                    messageType = PropertyType.Number;
                    break;
                case PropType.String:
                    messageType = PropertyType.String;
                    break;
            }

            switch (newMessage.Action)
            {
                case MessageAction.Add:
                    return doesPropertyExist ? new Response(false, newMessage.Property, "Property name already exists",PropType.Error, Name) : new Response(AddProperty(new Property(messageType, newMessage.Property, newMessage.Value)), newMessage.Property, newMessage.Value, newMessage.PropType, Name);

                case MessageAction.Get:
                    if (!doesPropertyExist)
                    {
                        return new Response(false, newMessage.Property, "Property name does not exist",PropType.Error, Name);
                    }
                    Property getProperty = GetProperty(newMessage.Property);
                   
                    return new Response(true, getProperty.Name,getProperty.Value,ConvertPropertyType(getProperty.Type), Name);

                case MessageAction.Set:
                    return !doesPropertyExist ? new Response(false, newMessage.Property, "Property name does not exist", PropType.Error, Name) : new Response(SetProperty(newMessage.Property, newMessage.Value, messageType), newMessage.Property, newMessage.Value, newMessage.PropType, Name);

                case MessageAction.Remove:
                    return !doesPropertyExist ? new Response(false, newMessage.Property, "Property name does not exist", PropType.Error, Name) : new Response(RemoveProperty(newMessage.Property), newMessage.Property, "none", PropType.Bool, Name);
            }
            return new Response(false, "Error", "Error", PropType.Error, Name);
        }

        private PropType ConvertPropertyType(PropertyType propertyType)
        {
            switch (propertyType)
            {
                case PropertyType.Bool:
                    return PropType.Bool;

                case PropertyType.Number:
                    return PropType.Number;

                case PropertyType.String:
                    return PropType.String;

                case PropertyType.Error:
                    return PropType.Error;

            }

            return PropType.Error;
        }

        public GameObject CloneGameObject()
        {
            GameObject newGameObject = new GameObject(Name);
            foreach (KeyValuePair<string, Property> prop in _properties)
            {
                newGameObject.AddProperty(prop.Value);
            }
            if (Image != null)
            {
                newGameObject.SetImage(Image.ConvertBinaryToImage());
            }

            return newGameObject;
        }

        public Dictionary<string, Property> GetAllProperties()
        {
            return _properties;
        }

        public void SetImage(Image image)
        {
            EditorImage = new BinaryImage(image);
        }

        public bool IsImageSet()
        {
            return EditorImage != null;
        }

        public int GetPropertyCount()
        {
            return _properties.Count();
        }
    }
}
