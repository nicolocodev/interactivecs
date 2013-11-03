using Windows.System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteractivecsWin8
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string _currentDirectory;

        public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _currentDirectory = ">";
            richTextBoxPrincipal.Document.SetText(new TextSetOptions(), _currentDirectory);
        }

        private void PrincipalOnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            string currentLine;
            switch (e.Key)
            {
                case VirtualKey.Back:
                    richTextBoxPrincipal.Document.GetText(TextGetOptions.IncludeNumbering, out currentLine);
                    if (currentLine == _currentDirectory)
                        e.Handled = true;
                    break;
                case VirtualKey.Enter:
                    
                    break;
                case VirtualKey.Left:

                    break;
            }
        }
    }
}