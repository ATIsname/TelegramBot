using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramSocialogicalTestBot.Model
{
    [Serializable]
    public class AnsweredTest
    {
        private int testID;
        private List<Answer> answers;
        private TestStateEnum testState;

        public int TestID { get => testID; set => testID = value; }
        public List<Answer> Answers { get => answers; set => answers = value; }
        public TestStateEnum TestState { get => testState; set => testState = value; }

        public AnsweredTest(int testID)
        {
            TestID = testID;
            Answers = new List<Answer>();
            TestState = TestStateEnum.NotAnswered;
        }


        public AnsweredTest(int testID, List<Answer> answers, TestStateEnum testState)
        {
            TestID = testID;
            Answers = answers;
            TestState = testState;
        }

        public AnsweredTest() { }
    }
}
