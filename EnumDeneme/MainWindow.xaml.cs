using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
//using static System.Net.Mime.MediaTypeNames;



namespace EnumDeneme
{
    
    public class MyData
    {
        public string Key { get; set; }
        public List<string> Values { get; set; }
    }

    public partial class MainWindow : Window
    {
        public List<ComboBox> comboBoxs = new List<ComboBox>();
        public Dictionary<string, ComboBox> comboLabel = new Dictionary<string, ComboBox>();
        public List<Label> labels = new List<Label>();
        public string csText = string.Empty;
        Dictionary<string, List<string>> enums;

        string buttonContent = string.Empty;
        List<TextBox> textBoxes = new List<TextBox>();
        List<Button> buttons = new List<Button>();

        List<string> Log = new List<string>();

        List<string> matchedEnums = new List<string>();

        List<string> comboList = new List<string>();

        Dictionary<string, string> comboTextItem = new Dictionary<string, string>();

        List<string> comboText;
        List<string> comboNames;

        Window enumWindow;
        Window kaydetWindow;
        Button clickedButton;
        ScrollViewer scrollViewer;

        List<ComboBox> updatedComboBoxs;

        Dictionary<string, string> gridItem = new Dictionary<string, string>();

        string newPath;
        public MainWindow()
        {
            InitializeComponent();
            FileNameTextBox.Text = "C:\\Users\\PC_4232\\Desktop\\Mehmet\\newEnums.cs";
            viewEnums();
        }

        public void viewEnums(string path = null) 
        {
            comboBoxs.Clear();
            labels.Clear();
            stackPanel.Children.Clear();
            Log.Clear();

            if (path == null)
            {
                csText = File.ReadAllText("C:\\Users\\PC_4232\\Desktop\\Mehmet\\newenum1.cs");
            }
            else
            {
                csText = File.ReadAllText(path);
            }

            comboList = new List<string>();
            
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;

            enums = ProcessEnumCode(csText);

            foreach (var key in enums.Keys)
            {
                comboList.Add(key);
            }

            foreach (var kvp in enums)
            {

                StackPanel comboTextBoxPanel = new StackPanel();
                comboTextBoxPanel.Orientation = Orientation.Horizontal;

                Button enumButton = new Button();
                enumButton.Background = Brushes.DimGray;
                enumButton.Foreground = Brushes.White;
                enumButton.Content = "     " + kvp.Key + "     ";
                enumButton.FontWeight = FontWeights.Bold;   
                enumButton.Name = kvp.Key;
                buttons.Add(enumButton);
                enumButton.FontSize = 16;
                enumButton.Width = 200;
                enumButton.Margin = new Thickness(0, 10, 0, 0);

                enumButton.Click += enumButtonClick;
                enumButton.MouseLeave += buttonMouseLeave;
                enumButton.MouseEnter += buttonMouseMove;
                //enumButton.MouseMove += buttonMouseMove;

                TextBox enumTextBox = new TextBox();
                enumTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                enumTextBox.TextWrapping = TextWrapping.NoWrap;
                enumTextBox.Name = kvp.Key;
                enumTextBox.FontWeight = FontWeights.Bold;
                enumTextBox.Width = 200;
                
                textBoxes.Add(enumTextBox);

                comboTextBoxPanel.Margin = new Thickness(0, 10, 0, 0);
                stackPanel.Children.Add(enumButton);

                buttons.Add(enumButton);

            }


            Button KaydetButton = new Button();
            KaydetButton.Content = "Kaydet";
            KaydetButton.Background = Brushes.DimGray;
            KaydetButton.Foreground = Brushes.White;
            KaydetButton.FontWeight = FontWeights.Bold;
            KaydetButton.FontSize = 13;
            KaydetButton.Margin = new Thickness(0, 20, 0, 0);
            KaydetButton.Click += KaydetClick;
            KaydetButton.Width = 100;
            stackPanel.Children.Add(KaydetButton);
            buttons.Add(KaydetButton);
        }

        private void buttonMouseLeave(object sender, MouseEventArgs e)
        {          
            Button button = sender as Button;

            button.Background = Brushes.DimGray;
            button.Width = 200;
            button.Foreground = Brushes.White;

        }

        private void buttonMouseMove(object sender, MouseEventArgs e)
        {
            Button button = (Button) sender;
            button.Width = 220;
            button.Foreground = Brushes.Black;
        }

        private void enumButtonClick(object sender, RoutedEventArgs e)
        {
            updatedComboBoxs = new List<ComboBox>();
            enumWindow = new Window();
            enumWindow.Width = 400;
            enumWindow.Height = 400;
            enumWindow.Background = Brushes.DimGray;

            foreach(Button button in buttons)
            {
                button.Visibility = Visibility.Collapsed;
            }
            

            StackPanel enumStack = new StackPanel();

            enumStack.VerticalAlignment = VerticalAlignment.Center;

            clickedButton = (Button)sender;
            clickedButton.IsEnabled = false;
            clickedButton.MouseLeave -= buttonMouseLeave;

            clickedButton.Visibility = Visibility.Visible;

            buttonContent = clickedButton.Content.ToString().Trim();
            List<string> Value = enums[buttonContent];

            int index = 0;
            foreach (var value in Value)
            {
                StackPanel valueAndComboPanel = new StackPanel();
                valueAndComboPanel.Background = Brushes.LightBlue;
                valueAndComboPanel.Orientation = Orientation.Horizontal;
                valueAndComboPanel.HorizontalAlignment = HorizontalAlignment.Center;

                Label valueLabel = new Label();
                valueLabel.Content = value;
                valueLabel.Width = 150;                
                valueLabel.FontWeight = FontWeights.Bold;

                ComboBox comboBox = new ComboBox();
                comboBox.Foreground = Brushes.Black;
                comboBox.FontWeight = FontWeights.Bold;
                comboBox.Width = 150;
                comboBox.Name = value;


                comboBox.SelectionChanged += comboBoxItemControl;

                if (comboBoxs.Count>0)
                {
                    foreach (ComboBox box in comboBoxs)
                    {
                        if (box.Name == comboBox.Name)
                        {
                            //comboBoxs.Remove(box);
                            updatedComboBoxs.Add(box);
                        }
                    }
                }

                comboBoxs.Add(comboBox);
                labels.Add(valueLabel);

                //comboLabel.Add(valueLabel.Content.ToString(), comboBox);

                foreach (var comboValue in comboList)
                {
                    if (buttonContent != comboValue)
                    {
                        comboBox.Items.Add(comboValue);
                    }
                }

                if (!comboBox.Items.Contains(string.Empty)) comboBox.Items.Add(string.Empty);

                valueAndComboPanel.Children.Add(valueLabel);
                valueAndComboPanel.Children.Add(comboBox);

                enumStack.Children.Add(valueAndComboPanel);

                if (comboTextItem.Keys.Contains(comboBox.Name))
                {
                    comboBox.Items.Add(comboTextItem[comboBox.Name]);
                    comboBox.SelectedItem = comboTextItem[comboBox.Name];
                }
                else
                {
                    comboTextItem[comboBox.Name] = "";
                }
                              
                index += 1;
            }
            

            StackPanel ButtonOkandBack = new StackPanel();
            ButtonOkandBack.Orientation = Orientation.Horizontal;
            ButtonOkandBack.Margin = new Thickness(0,20,0,0);

            Button back = new Button();

            back.Click += backButtonClick;
            back.Margin = new Thickness(0,0, 0, 0);

            Image image = new Image();
            var source = new BitmapImage(new Uri(@"C:\Users\PC_4232\source\repos\EnumDeneme\EnumDeneme\backButton-removebg-preview.png"));
            source.Freeze();
            image.Source = source;
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = image.Source;
            brush.Stretch = Stretch.UniformToFill;    
            
            back.Width = 30;
            back.Height = 30;
            
            back.MouseEnter += backMouseEnter;
            back.MouseLeave += backMouseLive;
            back.Background = brush;

            Button enumOK = new Button();
            enumOK.Content = "OK";
            enumOK.Width = 40;
            enumOK.Click += enumOKClick;
            enumOK.Margin = new Thickness(100, 0, 0, 0);
            enumOK.FontWeight = FontWeights.Bold;

            ButtonOkandBack.Children.Add(back);
            ButtonOkandBack.Children.Add(enumOK);

            enumStack.Children.Add(ButtonOkandBack);
            //enumStack.Children.Add(enumOK);


            scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = enumStack;

            scrollViewer.Margin = clickedButton.Margin;

            stackPanel.Children.Add(scrollViewer);

            //enumWindow.Content = scrollViewer;
            //enumWindow.Show();            
        }

        private void backMouseLive(object sender, MouseEventArgs e)
        {
            Button back = sender as Button;
            back.Content = "";

            Image image = new Image();
            var source = new BitmapImage(new Uri(@"C:\Users\PC_4232\source\repos\EnumDeneme\EnumDeneme\backButton-removebg-preview.png"));
            source.Freeze();
            image.Source = source;
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = image.Source;
            brush.Stretch = Stretch.UniformToFill;

            
        }

        private void backMouseEnter(object sender, MouseEventArgs e)
        {
            Button back = sender as Button;
            back.Foreground = Brushes.Black;
            back.FontWeight = FontWeights.Bold;
            back.Content = "Geri";
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (ComboBox combo in updatedComboBoxs)
            {
                comboBoxs.Remove(combo);
            }

            if (comboText != null) comboText.Clear();
            if (comboNames != null) comboNames.Clear();


            scrollViewer.Visibility = Visibility.Collapsed;
            clickedButton.MouseLeave += buttonMouseLeave;
            clickedButton.Width = 200;
            clickedButton.Foreground = Brushes.White;

            foreach (Button button in buttons)
            {
                button.Visibility = Visibility.Visible;
                button.IsEnabled = true;
            }
        }

        private void comboBoxItemControl(object sender, SelectionChangedEventArgs e)
        {
            ComboBox selectedBox = (ComboBox)sender;

            var duplicateItems = selectedBox.Items.Cast<string>()
                .GroupBy(item => item)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);

            foreach (var item in duplicateItems)
            {
                selectedBox.Items.Remove(item);
            }

            if (selectedBox.SelectedItem != null)
            {
                string selectedItem = selectedBox.SelectedItem.ToString();
                if (selectedItem != "")
                {
                    foreach (ComboBox box in comboBoxs)
                    {

                        for (int i = 0; i < buttons.Count; i++)
                        {
                            if (buttons[i].Name == selectedBox.Text) buttons[i].IsEnabled = true;
                        }

                        if (box.Name != selectedBox.Name) box.Items.Remove(selectedItem);
                        if (!box.Items.Contains(selectedBox.Text))
                        {
                            box.Items.Remove("");
                            box.Items.Add(selectedBox.Text);
                            comboList.Add(selectedBox.Text);
                            box.Items.Add("");
                        }                       
                    }
                }
                else
                {
                    
                    string addItem = selectedBox.Text;
                    //MessageBox.Show(addItem);
                    foreach (ComboBox box in comboBoxs)
                    {
                        
                        box.Items.Add(addItem);
                        if (box.Items.Contains(""))
                        {
                            box.Items.Remove("");
                        }
                        
                        box.Items.Add("");
                    }
                    
                }
            }

            comboList = comboList.Distinct().ToList();
        }

        private void enumOKClick(object sender, RoutedEventArgs e)
        {
            if (gridItem != null)
            {
                foreach (ComboBox combo in comboBoxs)
                {
                    if (gridItem.Keys.Contains(clickedButton.Content.ToString().Trim() + "." + combo.Name))
                    {
                        gridItem.Remove(clickedButton.Content.ToString().Trim() + "." + combo.Name);
                        gridItem[clickedButton.Content.ToString().Trim() + "." + combo.Name] = combo.Text;
                    }
                    else
                    {
                        gridItem[clickedButton.Content.ToString().Trim() + "." + combo.Name] = combo.Text;
                    }
                }
            }


            foreach (ComboBox combo in updatedComboBoxs)
            {
                //MessageBox.Show(combo.Name);
                comboBoxs.Remove(combo);
            }


            List<string> items = new List<string>();

            foreach (ComboBox combo in comboBoxs)
            {
                if (combo.Text != "")
                {
                    items.Add(combo.Text);
                }
            }

            foreach (ComboBox combo in comboBoxs)
            {
                foreach(string text in comboList)
                {
                    if (!items.Contains(text))
                    {
                        combo.Items.Add(text);
                    } 
                }
            }



            scrollViewer.Visibility = Visibility.Collapsed;

            foreach (Button button in buttons)
            {
                button.Visibility = Visibility.Visible;
            }


            clickedButton.Background = Brushes.White;
            clickedButton.Foreground = Brushes.Black;

            comboText = new List<string>();
            comboNames = new List<string>();

            foreach (ComboBox combo in comboBoxs)
            {
                //MessageBox.Show(combo.Text);

                comboTextItem[combo.Name] = combo.Text;
                comboText.Add(combo.Text);
                comboNames.Add(combo.Name);
            }
           
            foreach (var text in comboText)
            {

                if (text != "" & comboText.Where(x => x.Equals(text)).Count() > 2)
                {
                    MessageBox.Show(text + " elemanı birden fazla enum ile eşleşemez.");
                    clickedButton.IsEnabled = true;
                    return;
                }
            }
            

            for(int i = 0; i < buttons.Count; i++)
            {

                foreach (var text in comboText)
                {
                    if (text != "")
                    {
                        if (buttons[i].Name == text) 
                        {
                            //MessageBox.Show(text);
                            buttons[i].IsEnabled = false;
                            buttons[i].Foreground = Brushes.Black;
                            //stackPanel.Children.Remove(buttons[i]);
                        } 
                        comboList.Remove(text);
                    }
                        
                }
                
            }

            foreach(TextBox textBox in textBoxes)
            {
                if (textBox.Name == buttonContent)
                {
                    for (int i = 0; i < comboText.Count; i++)
                    {
                        if (comboText[i] != "" & !matchedEnums.Contains(comboNames[i]))
                        {
                            matchedEnums.Add(comboNames[i]);
                            string log = comboText[i] + " => " + textBox.Name + "." + comboNames[i] + "  ";
                            textBox.Text += log;
                            Log.Add(log);                            
                        }
                    }
                }
            }
            clickedButton.IsEnabled = true;
            //clickedButton.Width = 200;
            //clickedButton.MouseLeave += buttonMouseLeave;
            //clickedButton.MouseMove -= buttonMouseMove;
            enumWindow.Close();
        }

        public Dictionary<string, List<string>> ProcessEnumCode(string enumCode)
        {
            Dictionary<string, List<string>> enums = new Dictionary<string, List<string>>();

            var syntaxTree = CSharpSyntaxTree.ParseText(enumCode);
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location)
            };

            var compilation = CSharpCompilation.Create("DynamicEnumCompilation")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    MessageBox.Show("Derleme hatası:\n" + string.Join("\n", result.Diagnostics));
                    return enums;
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());

                //List<string> enumOutput = new List<string>();

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsEnum)
                    {
                        List<string> enumValue = new List<string>();
                        foreach (var value in Enum.GetValues(type))
                        {

                            enumValue.Add(value.ToString());
                        }

                        enums.Add(type.Name, enumValue);
                    }
                }

                return enums;
            }
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();

            if (result == true)
            {
                FileNameTextBox.Text = openFileDlg.FileName;               
            }
        }

        private void OKClick(object sender, RoutedEventArgs e)
        {
            viewEnums(FileNameTextBox.Text);
        }

        private void KaydetClick(object sender, RoutedEventArgs e)
        {

            foreach (ComboBox combo in updatedComboBoxs)
            {
                comboBoxs.Remove(combo);
            }

            kaydetWindow = new Window();

            kaydetWindow.Width = 640;
            kaydetWindow.Height = 480;

            Color color = (Color)ColorConverter.ConvertFromString("#333333");
            SolidColorBrush backgroundBrush = new SolidColorBrush(color);

            kaydetWindow.Background = backgroundBrush;

            kaydetWindow.Closed += kaydetWindowClosed;

            StackPanel kaydetStack = new StackPanel();

            kaydetStack.VerticalAlignment = VerticalAlignment.Center;
            kaydetStack.HorizontalAlignment = HorizontalAlignment.Center;
            kaydetStack.Margin = new Thickness(50,0,50,0);

            Label logLabel = new Label();
            logLabel.Content = "ENUM EŞLEŞMELERİ";
            logLabel.Foreground = Brushes.White;
            logLabel.Background = backgroundBrush;
            logLabel.HorizontalAlignment = HorizontalAlignment.Center;
            logLabel.FontSize = 18;
            logLabel.FontWeight = FontWeights.Bold; 
            logLabel.Margin = new Thickness(0, 0, 0, 20);
            kaydetStack.Children.Add(logLabel);

            TextBox kaydetTextBox = new TextBox();
            kaydetTextBox.Width = kaydetWindow.Width - 50;
            kaydetTextBox.Height = kaydetWindow.Height - 150;
            kaydetTextBox.Margin = new Thickness(0, 0, 0, 10);
            kaydetTextBox.FontWeight = FontWeights.Bold;

            foreach(var log in Log)
            {
                kaydetTextBox.Text += log + "\r\n";
            }

            //kaydetStack.Children.Add(kaydetTextBox);

            //------------------ Data Grid ----------------
            
            DataGrid matchGrid = new DataGrid();
            matchGrid.HorizontalAlignment = HorizontalAlignment.Center;
            matchGrid.VerticalAlignment = VerticalAlignment.Center;
            matchGrid.AutoGenerateColumns = false;
            matchGrid.AlternatingRowBackground = Brushes.LightGray;

            DataGridTextColumn enumName = new DataGridTextColumn {
                Header = "Enum İsmi",
                Binding = new Binding("Value"),
            };
            DataGridTextColumn enumValue = new DataGridTextColumn { 
                Header = "Eşleştiği Enum",
                Binding = new Binding("Key")
            };

            enumName.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            enumValue.Width = new DataGridLength(1*1, DataGridLengthUnitType.Star);

            enumName.HeaderStyle = new Style(typeof(DataGridColumnHeader));
            enumName.HeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, Brushes.LightGray));
            enumName.HeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontWeightProperty, FontWeights.Bold));
            enumName.HeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Center)); // Sütun başlığının yatay hizalama
            enumName.HeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch)); // Sütun başlığının genişliği


            enumValue.HeaderStyle = new Style(typeof(DataGridColumnHeader));
            enumValue.HeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, Brushes.LightGray));
            enumValue.HeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontWeightProperty, FontWeights.Bold));
            enumValue.HeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Center)); // Sütun başlığının yatay hizalama
            enumValue.HeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch)); // Sütun başlığının genişliği


            matchGrid.Columns.Add(enumName);
            matchGrid.Columns.Add(enumValue);   

            

            Button OkKaydetLog = new Button();
            OkKaydetLog.Content = "OK";
            OkKaydetLog.Width = 50;
            OkKaydetLog.Margin = new Thickness(20);
            OkKaydetLog.Click += OkKaydetLog_Click;

            kaydetWindow.Content = kaydetStack;


            string newCsText = csText;
            List<string> comboText = new List<string>();

            if (gridItem != null)
            {
                for (int i = 0; i < gridItem.Count; i++)
                {
                    if (gridItem.Values.ElementAt(i) == "")
                    {
                        gridItem.Remove(gridItem.Keys.ElementAt(i));
                    }
                }
            }

            matchGrid.ItemsSource = gridItem.ToList();
            kaydetStack.Children.Add(matchGrid);
            kaydetStack.Children.Add(OkKaydetLog);

            foreach (ComboBox combo in comboBoxs)
            {
                comboText.Add(combo.Text);
            }

            foreach (var text in comboText)
            {

                if (text != "" & comboText.Where(x => x.Equals(text)).Count() > 1)
                {

                    MessageBox.Show(text + " elemanı birden fazla enum ile eşleşemez.");
                    return;
                }
            }

            for (int i = 0; i < comboBoxs.Count; i++)
            {
                if (comboBoxs[i].Text != "")
                {
                    newCsText = newCsText.Replace("enum " + comboBoxs[i].Text, "enum " + comboBoxs[i].Name);
                }
            }            

            string path = FileNameTextBox.Text;
            newPath = path.Substring(0, path.LastIndexOf("\\")) + "\\" + "new" + path.Substring(path.LastIndexOf('\\') + 1);
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
                File.WriteAllText(newPath, newCsText);
            }
            else
            {
                File.WriteAllText(newPath, newCsText);
            }

            kaydetWindow.Show();


        }

        private void OkKaydetLog_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(newPath + " Dosyası Kaydedildi.");
            kaydetWindow.Close();
        }

        private void kaydetWindowClosed(object sender, EventArgs e)
        {
            //MessageBox.Show(newPath + " Dosyası Kaydedildi.");
        }
    }
}
