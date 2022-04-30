using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.IO;
using Telegram.Bot.Types;
using TelegramSocialogicalTestBot.Model;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using System.Runtime.Serialization.Formatters.Binary;

namespace TelegramSocialogicalTestBot
{
    internal class TelegramBotHelper
    {
        //TODO: Отчищать память
        private const string MAIN_PATH = @"X:\Работа\TelegramSocialogicalTestBot\TelegramSocialogicalTestBot\TestData\";
        private const string USERS_JSON_PATH = @"X:\Работа\TelegramSocialogicalTestBot\TelegramSocialogicalTestBot\TestData\users.json";
        private const string TESTS_JSON_PATH = @"X:\Работа\TelegramSocialogicalTestBot\TelegramSocialogicalTestBot\SocialogicalTests\tests.json";
        private string _token;
        private TelegramBotClient _client;

        public TelegramBotHelper(string token)
        {
            _token = token;
        }

        internal async Task GetUpdates()
        {
            try
            {
                _client = new Telegram.Bot.TelegramBotClient(_token);
                var me = await _client.GetMeAsync();
                if (me != null && !string.IsNullOrEmpty(me.Username))
                {
                    string message;
                    int offset = 0;
                    while (true)
                    {
                        try
                        {
                            var updates = await _client.GetUpdatesAsync(offset);
                            if (updates != null && updates.Count() > 0)
                            {
                                foreach (var update in updates)
                                {
                                    TestObject testObject = await GetTestObjectFromFile(USERS_JSON_PATH, update.Message.Chat.Id);
                                    if (testObject == null)
                                    {
                                        await AddTestObjectToFile(USERS_JSON_PATH, update.Message.Chat.Id);
                                        testObject = await GetTestObjectFromFile(USERS_JSON_PATH, update.Message.Chat.Id);
                                    }
                                    message = await GetMessage(update, testObject);
                                    await _client.SendTextMessageAsync(update.Message.Chat.Id, message);
                                    offset = update.Id + 1;
                                    testObject.LastBotMessage = message;
                                    await UpdateTestObjectInFile(USERS_JSON_PATH, testObject);
                                    message = "";
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        Thread.Sleep(1);
                    }
                }
                Console.WriteLine("me is not set");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private async Task<string> GetMessage(Update update, TestObject testObject)
        {
            Test test;
            string message = "";
            string updateMessage = update.Message.Text;
            string lastBotMessage = testObject.LastBotMessage;
            if (string.IsNullOrEmpty(lastBotMessage))
            {
                message = "Добрый день! Прежде, чем приступить к опросу, пожалуйста, введите данные о себе. \n";
                lastBotMessage = "";
            }
            if (String.IsNullOrEmpty(testObject.Name))
            {
                message += "Введите ваше Ф.И.О. (на русском языке)\n";
                if (lastBotMessage.Contains(message) || lastBotMessage.Contains("Ф.И.О. должно быть написано кирилицей\n")
                    || lastBotMessage.Contains("Ф.И.О. должно быть больше 2 букв\n"))
                {
                    Regex regex = new Regex(@"([А-ЯЁ][а-яё]+[\-\s]?){3,}");
                    Match match = regex.Match(updateMessage);
                    if (updateMessage.Trim().Length >= 3)
                    {
                        if (match.Success)
                        {
                            testObject.Name = updateMessage;
                        }
                        else
                        {
                            message = "Ф.И.О. должно быть написано кирилицей\n";
                            return message;
                        }
                    }
                    else
                    {
                        message = "Ф.И.О. должно быть больше 2 букв\n";
                        return message;
                    }

                }
                else
                {
                    return message;
                }

            }
            if (String.IsNullOrEmpty(testObject.Email))
            {
                message = "Введите вашу электронную почту";
                if (lastBotMessage.Contains(message))
                {
                    Regex regex = new Regex(@"[a-zA-Z1-9\-\._]+@[a-z1-9]+(.[a-z1-9]+){1,}");
                    Match match = regex.Match(updateMessage);
                    if (match.Success)
                    {
                        testObject.Email = updateMessage;
                    }
                    else
                    {
                        message = "Электронная почта введена некорректно";
                        return message;
                    }
                }
                else
                {
                    return message;
                }
            }
            //dasddddddddddddddd
            List<Test> tests = await GetTestsFromFile();
            AnsweredTest startedTest = GetStartedTestOrAnsweredTests(testObject, tests);
            if (startedTest == null)
            {
                //if (lastBotMessage.Contains("Введите номер опроса:") || 
                //    lastBotMessage.Contains("Теста с таким номером не существует.") ||
                //    lastBotMessage.Contains("Номер теста должен состоять из цифр."))
                //{
                    int id = 0;
                    if (Int32.TryParse(updateMessage, out id))
                    {
                        test = tests.FirstOrDefault(o => o.ID == id);
                        if (test == null)
                        {
                            message = "Теста с таким номером не существует.";
                            return message;
                        }
                        if (testObject.AnsweredTests.FirstOrDefault(o => o.TestID == id) == null)
                        {
                            SetAnswers(test, testObject);
                            testObject.AnsweredTests.FirstOrDefault(o => o.TestID == id).TestState = TestStateEnum.NotAnswered;
                        }
                        message = test.NameOfTest + "\n";
                        message += test.Questions[0].QuestionContent;
                        return message;
                    }
                    else
                    {
                        message = "Номер теста должен состоять из цифр.";
                        return message;
                    }

                }
                if (tests.Count == 0)
                {
                    message = "Приносим свои извинения, опросы сейчас не доступны. Попробуйте написать нам позже.";
                    return message;
                }
                if (tests.Count == 1)
                {
                    test = tests.First();
                    SetAnswers(test, testObject);
                    message = test.NameOfTest + "\n";
                    message += test.Questions[0].QuestionContent;
                    return message;
                }
                if (tests.Count > 1)
                {
                    int inputID;
                    if (int.TryParse(update.Message.Text, out inputID))
                    {
                        test = tests.First(o => o.ID == inputID);
                        if (test != null)
                        {
                            return test.Questions[0].QuestionContent;
                        }
                    }
                    message = "Некорректно введены данные \n";
                }
                else
                {
                    message += GetStringMessageWithListOfTests(tests);
                }
                return message;
            }
            else
            {
                test = tests.FirstOrDefault(o => o.ID == startedTest.TestID);
                Answer answer = startedTest.Answers.FirstOrDefault(o => !o.IsAnswered);
                if (test == null)
                {
                    Console.WriteLine("Error: 01");
                    return "Что-то пошло не так, обратитесь в службу поддержки.";
                }
                if (answer.AnswerID == 0)
                {
                    Console.WriteLine("Error: 02");
                    return "Что-то пошло не так, обратитесь в службу поддержки.";
                }
                string dir = MAIN_PATH + update.Message.Chat.Id.ToString() + "\\" + test.ID + "_" + test.NameOfTest + "\\";
                MessageHandler handler;
                if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
                string filePath = dir + answer.AnswerID;
                string[] files = Directory.GetFiles(dir);
                string answerFilePath = dir + files.FirstOrDefault(o => o.Contains(answer.AnswerID.ToString() + "."));
                if (System.IO.File.Exists(answerFilePath))
                {
                    System.IO.File.Delete(answerFilePath);
                }
                switch (update.Message.Type)
                {
                    case Telegram.Bot.Types.Enums.MessageType.Voice:
                        filePath += ".ogg";
                        handler = new MessageHandler(dir, filePath, _client, update.Message.Voice.FileId);
                        await handler.SetMessageFile();
                        break;
                    case Telegram.Bot.Types.Enums.MessageType.Text:
                        filePath += ".txt";
                        handler = new MessageHandler(dir, filePath, _client, update.Message.Text);
                        await handler.SetTxtMessageFile();
                        break;
                    case Telegram.Bot.Types.Enums.MessageType.Video:
                        filePath += ".mp4";
                        handler = new MessageHandler(dir, filePath, _client, update.Message.Video.FileId);
                        await handler.SetMessageFile();
                        break;
                    default:
                        message = "Приносим свои извинения, введенный формат сообщения не поддерживается.\n"
                            + testObject.LastBotMessage;
                        return message;
                }
                answer.AnswerConetnt = filePath;
                answer.IsAnswered = true;
                if (!startedTest.Answers.Any(o => !o.IsAnswered))
                {
                    testObject.AnsweredTests.Find(o => o.TestID == startedTest.TestID).TestState = TestStateEnum.Answered;
                    SetIsAnsweredToFalseInStartedTest(startedTest);
                    return "Спасибо за ваши ответы! Они очень важны для нас! \n" + GetStringMessageWithListOfTests(tests);
                }
                else
                {
                    int idOfAnswer = startedTest.Answers.FirstOrDefault(o => !o.IsAnswered).AnswerID;
                    message = test.Questions.FirstOrDefault(o => o.ID == idOfAnswer).QuestionContent;
                }
                return message;
            }
        }

        private static void SetIsAnsweredToFalseInStartedTest(AnsweredTest startedTest)
        {
            foreach (var item in startedTest.Answers)
            {
                item.IsAnswered = false;
            }
        }

        private void SetAnswers(Test test, TestObject testObject)
        {
            List<Answer> answers = new List<Answer>();
            foreach (var item in test.Questions)
            {
                answers.Add(new Answer(item.ID, item.QuestionContent, false));
            }
            testObject.AnsweredTests.Add(new AnsweredTest(test.ID, answers, TestStateEnum.Started));
        }

        private string GetStringMessageWithListOfTests(List<Test> tests)
        {
            string message = "Введите номер опроса: \n";
            for (int i = 0; i < tests.Count; i++)
            {
                message += tests[i].ID + ". " + tests[i] + "\n";
            }
            return message;
        }

        private AnsweredTest GetStartedTestOrAnsweredTests(TestObject testObject, List<Test> tests)
        {
            return testObject.AnsweredTests.FirstOrDefault(o =>
            o.TestState == TestStateEnum.Started || o.TestState == TestStateEnum.Answered);
        }

        private async Task<List<Test>> GetTestsFromFile()
        {

            string path = TESTS_JSON_PATH;
            List<Test> tests = new List<Test>();
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                tests = await JsonSerializer.DeserializeAsync<List<Test>>(fs);
            }
            return tests;
        }

        private async Task<TestObject> GetTestObjectFromFile(string path, long id)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                List<TestObject> testObjects = await JsonSerializer.DeserializeAsync<List<TestObject>>(fs);
                return testObjects.Where(o => o.ID == id).FirstOrDefault();
            }
        }

        private async Task UpdateTestObjectInFile(string path, TestObject testObject)
        {
            List<TestObject> testObjects;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                testObjects = await JsonSerializer.DeserializeAsync<List<TestObject>>(fs);
            }
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                testObjects.FirstOrDefault(o => o.ID == testObject.ID).Update(testObject);
                await JsonSerializer.SerializeAsync<List<TestObject>>(fs, testObjects);
            }
        }

        private async Task AddTestObjectToFile(string path, long id)
        {
            List<TestObject> testObjects;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                testObjects = await JsonSerializer.DeserializeAsync<List<TestObject>>(fs);
            }
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                testObjects.Add(new TestObject(id));
                await JsonSerializer.SerializeAsync<List<TestObject>>(fs, testObjects);
            }
        }
    }
}

