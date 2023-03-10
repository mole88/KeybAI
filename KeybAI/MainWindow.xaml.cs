using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using KeybAI.Classes;

namespace KeybAI
{
    public partial class MainWindow : Window
    {
        public Window statWindow;
        public Window customTextWindow;

        TextLoader text;
        Stopwatch sw = new();
        Mistakes mistakes = new();
        string folderPath = @"Texts\EngTexts";
        bool isCorrect = true;

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            text = new(folderPath);
            UpdateLines();

            MainTextBox.Focus();
        }
        private async void MainTextBox_Changed(object sender, RoutedEventArgs e)
        {
            if (MainTextBox.Text.Length > FirstStringLabel.Text.Length)
                return;
            if (MainTextBox.Text == FirstStringLabel.Text.Remove(MainTextBox.Text.Length))
            {
                MainTextBox.Background = Brushes.White;
                isCorrect = true;
                sw.Start();
            }
            else
            {
                if (isCorrect)
                {
                    MainTextBox.Background = Brushes.Gray;
                    mistakes.lineMissCount++;
                    mistakes.AddMistake(FirstStringLabel.Text[MainTextBox.Text.Length - 1].ToString());
                    isCorrect = false;
                }
                    
                if (AutoBcspCheckBox.IsChecked == true)
                {
                    await Task.Delay(100);
                    MainTextBox.Text = FirstStringLabel.Text.Remove(MainTextBox.Text.Length - 1);
                    MainTextBox.CaretIndex = MainTextBox.Text.Length;
                }
            }

            if (MainTextBox.Text == string.Empty)
            {
                mistakes.lineMissCount = 0;
                sw.Reset();
            }

            if (MainTextBox.Text == FirstStringLabel.Text)
            {
                sw.Stop();
                Timed();
                //mistakes.ShowMistakes();
                if (text.GetLinesCount() == 0)
                {
                    MainTextBox.Text = string.Empty;
                    text = new(folderPath);
                    ClearForNewText();
                }
                else
                {
                    UpdateLines();
                    MainTextBox.Text = string.Empty;
                }
            }
        }
        private void UpdateLines()
        {
            do{
                FirstStringLabel.Text = SecondStringLabel.Text;
                SecondStringLabel.Text = ThirdStringLabel.Text;
                ThirdStringLabel.Text = text.GetLinesCount() != 0 ? text.GetLine() : string.Empty;
            } while (FirstStringLabel.Text == string.Empty);
        }
        private void Timed()
        {
            int charsInLine = FirstStringLabel.Text.Length;
            int wordsInLine = FirstStringLabel.Text.Count(c => c == ' ');

            int charsPerMin = (int)Math.Round(charsInLine / sw.Elapsed.TotalMinutes);
            int wordsPerMin = (int)Math.Round(wordsInLine / sw.Elapsed.TotalMinutes);

            SaveResult(charsInLine, wordsInLine, charsPerMin, wordsPerMin);

            DisplayResults(charsPerMin, wordsPerMin, Properties.Settings.Default.bestCharsPerMin,
                Properties.Settings.Default.bestWordsPerMin, Properties.Settings.Default.bestMisPerLine);
        }
        private void SaveResult(int charsInLine, int wordsInLine, int charsPerMin, int wordsPerMin)
        {
            if (charsPerMin > Properties.Settings.Default.bestCharsPerMin)
                Properties.Settings.Default.bestCharsPerMin = charsPerMin;

            if (wordsPerMin > Properties.Settings.Default.bestWordsPerMin)
                Properties.Settings.Default.bestWordsPerMin = wordsPerMin;

            if (mistakes.lineMissCount < Properties.Settings.Default.bestMisPerLine)
                Properties.Settings.Default.bestMisPerLine = mistakes.lineMissCount;

            Properties.Settings.Default.totalChars += (ulong)charsInLine;
            Properties.Settings.Default.totalWords += (ulong)wordsInLine;
            Properties.Settings.Default.totalMis += (ulong)mistakes.lineMissCount;
            Properties.Settings.Default.totalLines++;
            Properties.Settings.Default.totalTime += sw.Elapsed;

            Properties.Settings.Default.Save();
        }
        private void DisplayResults(int cPerMin, int wPerMin, int bChPerMin, int bWoPerMin, int bMisPerLine)
        {
            CharsPerMinLabel.Text = cPerMin.ToString() + " chars/min";
            WordsPerMinLabel.Text = wPerMin.ToString() + " words/min";
            MisPerLineLabel.Text = mistakes.lineMissCount + " mis/line";

            BestCharsPerMinLabel.Text = bChPerMin.ToString() + " chars/min";
            BestWordsPerMinLabel.Text = bWoPerMin.ToString() + " words/min";
            BestMisPerLineLabel.Text = bMisPerLine.ToString() + " mis/line";
        }
        private void LoadSettings()
        {
            BestCharsPerMinLabel.Text = Properties.Settings.Default.bestCharsPerMin.ToString() + " chars/min";
            BestWordsPerMinLabel.Text = Properties.Settings.Default.bestWordsPerMin.ToString() + " words/min";
            BestMisPerLineLabel.Text = Properties.Settings.Default.bestMisPerLine.ToString() + " mis/line";
            AutoBcspCheckBox.IsChecked = Properties.Settings.Default.autoBcsp;

            if (Properties.Settings.Default.kBoardLang == "Rus")
            {
                ChangeLangButton_Click(new object(), new RoutedEventArgs());
                folderPath = @"Texts\EngTexts";
            }
            else
                folderPath = @"Texts\RusTexts";
        }
        private void ClearForNewText()
        {
            MainTextBox.Text = string.Empty;
            FirstStringLabel.Text = string.Empty;
            SecondStringLabel.Text = string.Empty;
            ThirdStringLabel.Text = string.Empty;
            UpdateLines();
            MainTextBox.Focus();
        }
        private void CustomTextButton_Click(object sender, RoutedEventArgs e)
        {
            customTextWindow = new CustomTextWindow { Owner = this };
            customTextWindow.ShowDialog();

            if (CustomTexts.SelectedText != null)
            {
                text = new(path:CustomTexts.SelectedText.Path);
                ClearForNewText();
                MainTextBox.Focus();
            }
        }
        private void NewTextButton_Click(object sender, RoutedEventArgs e)
        {
            text = new(folderPath);
            ClearForNewText();
        }
        private void StatButton_Click(object sender, RoutedEventArgs e)
        {
            statWindow = new StatisticsWindow(mistakes) { Owner = this };
            statWindow.ShowDialog();
        }
        private void ChangeLangButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeLangButton.Content.ToString() == "Rus")
            {
                EngImage.Visibility = Visibility.Collapsed;
                RusImage.Visibility = Visibility.Visible;
                ChangeLangButton.Content = "Eng";
                folderPath = @"Texts\RusTexts";
            }
            else
            {
                EngImage.Visibility = Visibility.Visible;
                RusImage.Visibility = Visibility.Collapsed;
                ChangeLangButton.Content = "Rus";
                folderPath = @"Texts\EngTexts";
            }
            MainTextBox.Focus();
            HideKeybButton.Content = "Hide";

            text = new(folderPath);
            ClearForNewText();
        }
        private void HideKeybButton_Click(object sender, RoutedEventArgs e)
        {

            if (HideKeybButton.Content.ToString() == "Hide")
            {
                HideKeybButton.Content = "Show";
                RusImage.Visibility = Visibility.Hidden;
                EngImage.Visibility = Visibility.Hidden;
            }
            else
            {
                HideKeybButton.Content = "Hide";
                RusImage.Visibility = Visibility.Visible;
                EngImage.Visibility = Visibility.Visible;
            }
            MainTextBox.Focus();
        }
        private void AutoBcspCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MainTextBox.Text = string.Empty;
            MainTextBox.Focus();
        }
        protected override void OnClosed(EventArgs e)
        {
            Properties.Settings.Default.autoBcsp = (bool)AutoBcspCheckBox.IsChecked;
            Properties.Settings.Default.kBoardLang = ChangeLangButton.Content.ToString();
            Properties.Settings.Default.Save();
            mistakes.SaveMistakes();
            base.OnClosed(e);
        }
    }
}
