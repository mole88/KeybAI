using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace KeybAI.Classes
{
    internal class TextLoader
    {
        private string text;
        private Queue<string> lines = new Queue<string>();
        private string basicTextsFolderPath;

        public TextLoader(string basicTextsFolderPath = @"Texts\EngTexts", string path = "")
        {
            this.basicTextsFolderPath = basicTextsFolderPath;

            if (!string.IsNullOrEmpty(path))
                LoadText(path);
            else
                RandomTextSelect();

            TextToStrings();
        }
        private void RandomTextSelect()
        {
            Random random = new();
            int fileNumber = random.Next(Directory.GetFiles(basicTextsFolderPath).Length);
            LoadText($@"{basicTextsFolderPath}\Text{fileNumber}.txt");
        }
        private void LoadText(string path)
        {
            try
            {
                string curText;
                using (StreamReader sr = new(path, Encoding.Default))
                    curText = sr.ReadToEnd();

                text = new Regex(@"\s+").Replace(curText, " ");
            }
            catch (Exception)
            {
                text = string.Empty;
            }
        }
        private void TextToStrings()
        {
            string[] words = text.Split();
            int maxLength = 88;
            int lastWord = 0;
            string currentLine = string.Empty;
            for (int i = 0; i < words.Length; i++)
            {
                if ((currentLine + words[i]).Length >= maxLength)
                {
                    lines.Enqueue(currentLine);
                    currentLine = string.Empty;
                }
                currentLine += words[i] + " ";
                lastWord = i;
            }

            currentLine = string.Empty;
            for (int i = lastWord; i < words.Length; i++)
                currentLine += words[i] + " ";

            lines.Enqueue(currentLine);
        }
        public string GetLine() => lines.Dequeue();
        public int GetLinesCount() => lines.Count;
    }
}
