using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoverageAlgorithm
{
    public class Collection
    {
        readonly string filePath;

        public List<CoverageSpaceData> Graphics { get; }

        public Collection(string path)
        {
            Graphics = new List<CoverageSpaceData>();
            filePath = path;

            if (!File.Exists(filePath))
            {
                Save();
                return;
            }

            string jsonString = File.ReadAllText(filePath);
            Graphics = JsonConvert.DeserializeObject<List<CoverageSpaceData>>(jsonString);
            //Graphics.Sort((x, y) => x.Name.CompareTo(y.Name));
        }



        public void Save()
        {
            string jsonString = JsonConvert.SerializeObject(Graphics, Formatting.Indented);
            File.WriteAllText(filePath, jsonString);
        }

        public void Export(string path)
        {
            int line = 0;
            string[] lines = new string[Graphics.Count + 1];

            lines[line] = "const int GRAPHICS_COUNT = " + Graphics.Count + ";";
            line++;
            for (int i = 0; i < Graphics.Count; i++, line++)
            {
                //lines[line] = "const int " + Graphics[i].Name.Replace(" ", string.Empty) + "[" + Graphics[i].Size.Item2 + "] = { " + Graphics[i].Code + " };";
            }

            File.WriteAllLines(path, lines);
        }
    }
}
