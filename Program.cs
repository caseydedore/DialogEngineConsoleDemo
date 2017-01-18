using DataReader;
using DialogEngine.Engine;
using DialogEngine.EngineModel;
using DialogEngine.Model;
using System;
using System.IO;

namespace DialogEngineConsoleDemo
{
    public class Program
    {
        private FileReader fileReader = new FileReader();
        private XmlDataReader xmlReader = new XmlDataReader();

        private ConversationDirector director = null;


        public static void Main(string[] args)
        {
            var program = new Program();

            var data = program.LoadData();

            program.director = new ConversationDirector(data);

            Console.WriteLine("Data Processed. Press enter to continue...");
            Console.ReadLine();

            var userInput = new ConsoleKeyInfo();

            var result = new ConversationResult();
            var action = new ConversationAction();

            result = program.director.Start();

            while(userInput.Key != ConsoleKey.Escape && result.Statements.Count > 0)
            {
                Console.Clear();

                Console.WriteLine(result.CurrentActor.Name + ":");

                var index = 1;
                foreach(var s in result.Statements)
                {
                    Console.WriteLine(index++ + ": " + s.Dialog);
                }

                userInput = Console.ReadKey();

                action = new ConversationAction();
                action.ChosenStatement = result.Statements[0];
                action.CurrentStatementLink = result.CurrentStatementLink;

                result = program.director.Advance(action);
            }
        }

        private Conversation LoadData()
        {
            var fileData = fileReader.Load(Path.GetFullPath("Data"), "conversation.xml");
            var xmlData = xmlReader.Deserialize<Conversation>(fileData);

            return xmlData;
        }
    }
}
