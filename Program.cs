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

            program.SetTextColorToDefault();
            Console.WriteLine("Data Processed.");
            Console.WriteLine("Use the number keys to advance the following conversation when the 'User' (you) is presented with conversation options.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            var userInput = new ConsoleKeyInfo();

            ConversationResult result = null;
            var action = new ConversationAction();

            result = program.director.Start();

            while(userInput.Key != ConsoleKey.Escape && result.Status == ConversationStatus.Active)
            {
                var isUserTurn = result.Statements.Count > 1;
                Console.Clear();
                program.SetUserTitleTextColorBasedOnTurn(isUserTurn);
                Console.WriteLine(result.CurrentActor.Name + ":");
                program.SetTextColorBasedOnTurn(isUserTurn);

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

        private void SetTextColorToDefault()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void SetUserTitleTextColorBasedOnTurn(bool isUserTurn)
        {
            if (isUserTurn)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
        }

        private void SetTextColorBasedOnTurn(bool isUserTurn)
        {
            if (isUserTurn)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
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
