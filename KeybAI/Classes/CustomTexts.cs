using System.Collections.ObjectModel;

namespace KeybAI.Classes
{
    class CustomTexts
    {
        public static ObservableCollection<Text> Texts { get; set; } = new();
        static public Text SelectedText { get; set; }
    }
}
