using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ReadMemoryOfWow
{

    public class CheatEngineFile {

        public static void DisplayCheatTable(CheatTable cheatTable)
        {
            Console.WriteLine("Cheat Entries:");

            foreach (var cheatEntry in cheatTable.CheatEntries.CheatEntry)
            {
                Console.WriteLine($"ID: {cheatEntry.ID}");
                Console.WriteLine($"Description: {cheatEntry.Description}");
                Console.WriteLine($"VariableType: {cheatEntry.VariableType}");
                Console.WriteLine($"Address: {cheatEntry.Address}");
                Console.WriteLine();
            }
        }
        public static void Load(string realtiveFilePath, out bool found, out CheatTable foundTable) {
            XmlSerializer serializer = new XmlSerializer(typeof(CheatTable));

            foundTable = null;
            found = false;
            using (FileStream fs = new FileStream(realtiveFilePath, FileMode.Open))
            {
                foundTable = (CheatTable)serializer.Deserialize(fs);
                found = true;
            }

        }
    }


    [XmlRoot(ElementName = "CheatTable")]
    public class CheatTable
    {
        [XmlElement(ElementName = "CheatEntries")]
        public CheatEntries CheatEntries { get; set; }
    }

    public class CheatEntries
    {
        [XmlElement(ElementName = "CheatEntry")]
        public List<CheatEntry> CheatEntry { get; set; }
    }

    public class CheatEntry
    {
        [XmlElement(ElementName = "ID")]
        public int ID { get; set; }

        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "VariableType")]
        public string VariableType { get; set; }

        [XmlElement(ElementName = "Address")]
        public string Address { get; set; }
    }

}
