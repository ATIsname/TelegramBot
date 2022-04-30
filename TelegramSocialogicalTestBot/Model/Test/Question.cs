using System;
using System.Collections.Generic;

namespace TelegramSocialogicalTestBot.Model
{
    [Serializable]
    public class Question : IEquatable<Question>
    {
        private int id;
        private string nameOfQuestion;

        public int ID { get => id; set => id = value; }
        public string QuestionContent { get => nameOfQuestion; set => nameOfQuestion = value; }

        public Question(string nameOfQuestion, int id)
        {
            QuestionContent = nameOfQuestion;
            this.id = id;
        }
        public Question() { }

        public bool Equals(Question other)
        {
            return other.QuestionContent == QuestionContent;
        }
    }
}