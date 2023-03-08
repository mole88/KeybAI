using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text.Json;
using System.IO;

namespace KeybAI.Classes
{
    internal class Mistakes
    {
        private Dictionary<string, int> mistakeLetters = new Dictionary<string, int>();
        public int lineMissCount { get; set; }

        public Mistakes()
        {
            LoadMistakes();
        }
        public void AddMistake(string curLetter)
        {
            curLetter = curLetter.ToLower();
            if (!mistakeLetters.ContainsKey(curLetter))
                mistakeLetters.Add(curLetter, 1);
            else
                mistakeLetters[curLetter]++;
        }
        public Dictionary<string, int> GetMistakesLetters() => mistakeLetters;

        public void ShowMistakes()
        {
            string s = "";

            foreach (var pair in mistakeLetters.OrderByDescending(x => x.Value))
                s += pair.Key + " " + pair.Value + "\n";
            MessageBox.Show(s);
        }
        public void SaveMistakes()
        {
            string json = JsonSerializer.Serialize(mistakeLetters);
            using StreamWriter sw = new("Mistakes.json");
            sw.Write(json);
        }
        /*public static void DeleteMistakes()
        {
            MessageBox.Show(File.Exists("Mistakes.json").ToString());
            File.Delete("Mistakes.json");
        }*/
        private void LoadMistakes()
        {
            if (File.Exists("Mistakes.json"))
            {
                string json;
                using StreamReader sr = new("Mistakes.json");
                json = sr.ReadToEnd();

                mistakeLetters = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
            }
        }
    }
}
