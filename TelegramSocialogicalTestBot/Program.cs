using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.RegularExpressions;
using TelegramSocialogicalTestBot.Model;

namespace TelegramSocialogicalTestBot
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //string path = @"X:\Работа\TelegramSocialogicalTestBot\TelegramSocialogicalTestBot\TestData\users.json";
            //List<TestObject> testObjects = new List<TestObject>() { new TestObject(213213, null, null, new List<AnsweredTest>()) };
            //using (FileStream fs = new FileStream(path, FileMode.Create))
            //{
            //    await JsonSerializer.SerializeAsync<List<TestObject>>(fs, testObjects);
            //}
            //string path = @"X:\Работа\TelegramSocialogicalTestBot\TelegramSocialogicalTestBot\TestData\tests.json";
            //List<Test> tests = new List<Test>() { new Test("First", new List<Question>() { new Question("FirstQuestion", 0)}, 0) };
            //using (FileStream fs = new FileStream(path, FileMode.Create))
            //{
            //    await JsonSerializer.SerializeAsync<List<Test>>(fs, tests);
            //}
            TelegramBotHelper telegramBotHelper = new TelegramBotHelper(token: "1737195701:AAEToBhRYiSKo5NgPC7BGvVhAvtZw00TQZI");
            await telegramBotHelper.GetUpdates();
        }
    }
}
