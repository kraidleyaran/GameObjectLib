using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObjectLib;
using Messaging;
using NUnit.Framework;

namespace GameObjectLib_Test
{
    [TestFixture]
    public class GameObjectTest
    {
        [TestCase(TestName = "Adding a property to a GameObject using it's AddProperty() method works correctly")]
        public void AddPropertyToGameObject()
        {
            GameObject newGameObject = new GameObject("new game object");
            Property newProperty = new Property(PropertyType.String, "new property", "a new value");
            Assert.IsTrue(newGameObject.AddProperty(newProperty));
            Assert.IsTrue(newGameObject.DoesPropertyExist(newProperty.Name));
        }

        [TestCase(TestName = "Removing a property from a GameObject using it's RemoveProperty() method works correctly")]
        public void RemovePropertyFromGameObject()
        {
            GameObject newGameObject = new GameObject("new game object");
            Property newProperty = new Property(PropertyType.String, "new property", "a new value");
            newGameObject.AddProperty(newProperty);
            Assert.IsTrue(newGameObject.RemoveProperty(newProperty.Name));
        }

        [TestCase(TestName = "Retrieving a property from a GameObject using it's GetProperty() method works correctly")]
        public void GetPropertyFromGameObject()
        {
            GameObject newGameObject = new GameObject("new game object");
            Property newProperty = new Property(PropertyType.String, "new property", "a new value");
            newGameObject.AddProperty(newProperty);
            Assert.IsTrue(newProperty.Value == newGameObject.GetProperty(newProperty.Name).Value);
        }

        [TestCase(TestName = "Setting a property using a GameObject's SetProperty() method works correctly")]
        public void SetPropertyOnGameObject()
        {
            GameObject newGameObject = new GameObject("new game object");
            Property newProperty = new Property(PropertyType.String, "new property", "a new value");
            newGameObject.AddProperty(newProperty);
            string newPropValue = "even newer value ";
            Assert.IsTrue(newGameObject.SetProperty(newProperty.Name, newPropValue, newProperty.Type ));
            Assert.IsTrue(newGameObject.GetProperty(newProperty.Name).Value == newPropValue);
        }

        [TestCase(TestName = "Receiving an AddProperty() via ReceiveMessage() works correctly")]
        public void ReceiveMessage_AddProperty()
        {
            GameObject newGameObject = new GameObject("new game object");
            Message newMessage = new Message(newGameObject.Name,MessageAction.Add, "new property", "a value", PropType.String);
            Response response = newGameObject.ReceiveMessage(newMessage);
            Assert.IsTrue(response.Status);
            Assert.IsTrue(newGameObject.DoesPropertyExist("new property"));
            Assert.IsTrue("a value" == newGameObject.GetProperty("new property").Value);
        }
        [TestCase(TestName = "Receiving an Invalid AddProperty() via ReceiveMessage() works correctly")]
        public void InvalidReceiveMessage_AddProperty()
        {
            GameObject newGameObject = new GameObject("new game object");
            Property newProperty = new Property(PropertyType.String, "new property", "a new value");
            newGameObject.AddProperty(newProperty);

            Message newMessage = new Message(newGameObject.Name, MessageAction.Add, "new property", "a value", PropType.String);
            Response response = newGameObject.ReceiveMessage(newMessage);

            Assert.IsFalse(response.Status);
            Assert.IsTrue(response.Value == "Property name already exists");
            Assert.IsTrue(response.Property == newProperty.Name);
            Assert.IsTrue(response.PropType == PropType.Error); 
        }


        [TestCase(TestName = "Receiving a Get message using ReceiveMessage() method works correctly")]
        public void ReceiveMessage_GetProperty()
        {
            GameObject newGameObject = new GameObject("new game object");
            Property newProperty = new Property(PropertyType.String, "new property", "a value");
            newGameObject.AddProperty(newProperty);

            Message getMessage =  new Message(newGameObject.Name, MessageAction.Get, newProperty.Name);
            Response response = newGameObject.ReceiveMessage(getMessage);
            Assert.IsTrue(response.Status);
            Assert.IsTrue(response.Property == newProperty.Name);
            Assert.IsTrue(response.Value == newProperty.Value);
            Assert.IsTrue(response.PropType == PropType.String);
        }
        [TestCase(TestName = "Receiving an Invalid Get message using ReceiveMessage() method works correctly")]
        public void InvalidReceiveMessage_GetProperty()
        {
            GameObject newGameObject = new GameObject("new game object");
            Property newProperty = new Property(PropertyType.String, "new property", "a value");
            newGameObject.AddProperty(newProperty);

            Message getMessage = new Message(newGameObject.Name, MessageAction.Get, "wrong name");
            Response response = newGameObject.ReceiveMessage(getMessage);
            Assert.IsFalse(response.Status);
            Assert.IsTrue(response.Value == "Property name does not exist");
            Assert.IsTrue(response.PropType == PropType.Error );

        }

        [TestCase(TestName = "Receiving a Set message using ReceiveMessage() method works correctly")]
        public void ReceiveMessage_SetProperty()
        {
            GameObject newGameObject = new GameObject("new game object");
            Property newProperty = new Property(PropertyType.String, "new property", "a value");
            newGameObject.AddProperty(newProperty);

            Message setMessage = new Message(newGameObject.Name, MessageAction.Set, newProperty.Name,"a new value", PropType.String);
            Response response = newGameObject.ReceiveMessage(setMessage);
            Assert.IsTrue(response.Status);
            Assert.IsTrue(response.Property == newProperty.Name);
            Assert.IsTrue(response.Value != newProperty.Value);
            Assert.IsTrue(response.Value == newGameObject.GetProperty(newProperty.Name).Value);
        }

        [TestCase(TestName = "Receiving an Invalid Set message using the ReceiveMessage() method works correctly")]
        public void InvalidReceiveMessage_SetProperty()
        {
            GameObject newGameObject = new GameObject("new game object");

            Message setMessage = new Message(newGameObject.Name, MessageAction.Set, "a property name that is wrong or doesn't exist", "a new value", PropType.String);
            Response response = newGameObject.ReceiveMessage(setMessage);
            Assert.IsFalse(response.Status);
            Assert.IsTrue(response.Value == "Property name does not exist");
            Assert.IsTrue(response.PropType == PropType.Error);
        }

        [TestCase(TestName = "Receiving a Remove message using ReceiveMessage() method works correctly")]
        public void ReceiveMessage_RemoveProperty()
        {
            GameObject newGameObject = new GameObject("new game object");
            Property newProperty = new Property(PropertyType.String, "new property", "a value");
            newGameObject.AddProperty(newProperty);

            Message removeMessage = new Message(newGameObject.Name,MessageAction.Remove, newProperty.Name);
            Response response = newGameObject.ReceiveMessage(removeMessage);
            Assert.IsTrue(response.Status);
            Assert.IsTrue(response.Property == newProperty.Name);
            Assert.IsTrue(response.Value == "none");
            Assert.IsFalse(newGameObject.DoesPropertyExist(newProperty.Name));

        }

        [TestCase(TestName = "Receiving an Invalid Remove message using ReceiveMessage() method works correctly")]
        public void InvalidReceiveMesssage_RemoveProperty()
        {
            GameObject newGameObject = new GameObject("new game object");
            Property newProperty = new Property(PropertyType.String, "new property", "a value");
            newGameObject.AddProperty(newProperty);

            Message removeMessage = new Message(newGameObject.Name, MessageAction.Remove, "a name this is wrong or missing");
            Response response = newGameObject.ReceiveMessage(removeMessage);
            Assert.IsFalse(response.Status);
            Assert.IsTrue(response.Value == "Property name does not exist");
            Assert.IsTrue(response.PropType == PropType.Error);

        }
    }
}
