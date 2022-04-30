using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TelegramSocialogicalTestBot.Model
{
    [Serializable]
    public class TestObject
    {
        private long id;
        private string name;
        private string email;
        private List<AnsweredTest> answeredTests;
        private string lastBotMessage;

        public long ID { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string LastBotMessage { get => lastBotMessage; set => lastBotMessage = value; }
        public List<AnsweredTest> AnsweredTests { get => answeredTests; set => answeredTests = value; }

        public TestObject(string name, string email, string lastBotMessage, List<AnsweredTest> answeredTests)
        {
            Name = name;
            Email = email;
            LastBotMessage = lastBotMessage;
            AnsweredTests = answeredTests;
        }

        public TestObject(long id, string name, string email, List<AnsweredTest> tests)
        {
            this.id = id;
            Name = name;
            Email = email;
            AnsweredTests = tests;
        }

        public TestObject(long id)
        {
            this.id = id;
            Name = name;
            Email = email;
            AnsweredTests = new List<AnsweredTest>();
        }

        public TestObject() { }

        public void Update(TestObject testObject)
        {
            ID = testObject.ID;
            Name = testObject.Name;
            Email = testObject.Email;
            LastBotMessage = testObject.LastBotMessage;
            AnsweredTests = testObject.AnsweredTests;
        }

        public bool Equals(TestObject other)
        {
            return other.id == id;
        }
    }
}
