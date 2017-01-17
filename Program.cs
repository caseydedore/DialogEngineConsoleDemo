using DataReader;
using DialogEngine.Model;
using System;
using System.IO;

namespace DialogEngineConsoleDemo
{
    public class Program
    {
        private FileReader fileReader = new FileReader();
        private XmlDataReader xmlReader = new XmlDataReader();


        public static void Main(string[] args)
        {
            var program = new Program();

            var data = program.LoadData(); 
        }

        private Conversation LoadData()
        {
            var fileData = fileReader.Load(Path.GetFullPath("Data"), "conversation.xml");
            var xmlData = xmlReader.Deserialize<Conversation>(fileData);

            return xmlData;
        }
    }
}
