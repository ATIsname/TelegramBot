using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramSocialogicalTestBot.Model
{
    [Serializable]
    public class Test : IEquatable<Test>
    {
        private int id;
        private string nameOfTest;
        private List<Question> questions;

        public int ID { get => id; set => id = value; }

        public string NameOfTest { get => nameOfTest; set => nameOfTest = value; }

        public List<Question> Questions { get => questions; set => questions = value; }

        public Test(string nameOfTest, List<Question> questions, int id)
        {
            this.id = id;
            NameOfTest = nameOfTest;
            Questions = questions;
        }

        public Test() { }

        public bool Equals(Test other)
        {
            return other.id == id || other.NameOfTest == NameOfTest;
        }
    }
}
