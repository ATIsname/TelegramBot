using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramSocialogicalTestBot.Model
{
    [Serializable]
    public class Answer
    {
        private int answerID;
        private string answerConetnt;
        bool isAnswered;

        public int AnswerID { get => answerID; set => answerID = value; }
        public string AnswerConetnt { get => answerConetnt; set => answerConetnt = value; }
        public bool IsAnswered { get => isAnswered; set => isAnswered = value; }

        public Answer(int answerID, string answerConetnt, bool isAnswered)
        {
            AnswerID = answerID;
            AnswerConetnt = answerConetnt;
            IsAnswered = isAnswered;
        }

        public Answer()
        {

        }
    }
}
