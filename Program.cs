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

            Console.WriteLine("Data Processed.");
            Console.WriteLine("Use the number keys to advance the following conversation.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            var userInput = new ConsoleKeyInfo();

            ConversationResult result = null;
            var action = new ConversationAction();

            result = program.director.Start();

            while(userInput.Key != ConsoleKey.Escape && result.Status == ConversationStatus.Active)
            {
                Console.Clear();

                Console.WriteLine(result.CurrentActor.Name + ":");

                var index = 1;
                foreach(var s in result.Statements)
                {
                    Console.WriteLine(index++ + ": " + s.Dialog);
                }

                userInput = Console.ReadKey();
                var input = program.GetChoiceOrDefault(userInput.KeyChar.ToString()) - 1;

                if (!program.CheckRange(input, 0, result.Statements.Count - 1)) input = 0;

                action = new ConversationAction();
                action.ChosenStatement = result.Statements[input];
                action.CurrentStatementLink = result.CurrentStatementLink;

                result = program.director.Advance(action);
            }
        }

        private Conversation LoadData()
        {
            var conversation = new Conversation();

            var conversationData = fileReader.Load(Path.GetFullPath("Data"), "conversation.xml");
            conversation = xmlReader.Deserialize<Conversation>(conversationData);

            var statementData = fileReader.Load(Path.GetFullPath("Data"), "statements.xml");
            var statements = xmlReader.Deserialize<Conversation>(statementData);

            conversation.Statements = statements.Statements;

            return conversation;
        }

        private int GetChoiceOrDefault(string choice, int defaultNumber = 1)
        {
            var convertedChoice = defaultNumber;

            try
            {
                convertedChoice = Convert.ToInt32(choice);
            }
            catch
            {
                convertedChoice = defaultNumber;
            }

            return convertedChoice;
        }

        private bool CheckRange(int number, int min, int max)
        {
            return (number <= max && number >= min);
        }
    }
}
