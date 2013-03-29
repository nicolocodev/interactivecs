using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Roslyn.Compilers;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace Interactivecs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly string[] _defaultNamespaces;
        private readonly ScriptEngine _scriptEngine;
        private readonly Session _session;
        private string _currentDirectory;
        private FlowDocument _flowDocument;
        private object _result;


        public MainWindow()
        {
            _scriptEngine = new ScriptEngine();
            _session = _scriptEngine.CreateSession();
            _scriptEngine.CreateSession();
            _defaultNamespaces = new[] {"System"};
            foreach (string defaultNamespace in _defaultNamespaces)
                //_session.ImportNamespace(defaultNamespace);
                InitializeComponent();
            Loaded += MainWindowsLoaded;
        }

        private void MainWindowsLoaded(object sender, RoutedEventArgs e)
        {
            _currentDirectory = string.Format("{0}>",
                                              Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            _flowDocument = new FlowDocument();
            _flowDocument.Blocks.Add(new Paragraph(new Run(_currentDirectory)));
            richTextBoxPrincipal.Document = _flowDocument;
        }

        private void RichTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Back:
                    {
                        TextPointer start = richTextBoxPrincipal.CaretPosition;
                        string text = start.GetTextInRun(LogicalDirection.Backward);
                        if (text == _currentDirectory)
                            e.Handled = true;
                    }
                    break;
                case Key.Enter:
                    string currentCode = richTextBoxPrincipal.CaretPosition.GetTextInRun(LogicalDirection.Backward);
                    ProccessCommand(currentCode);
                    e.Handled = true;
                    break;
                case Key.Left:
                    {
                        TextPointer start = richTextBoxPrincipal.CaretPosition;
                        string text = start.GetTextInRun(LogicalDirection.Backward);
                        if (text == _currentDirectory || text == string.Empty)
                            e.Handled = true;
                    }
                    break;
            }
        }

        private void ProccessCommand(string currentCode)
        {
            try
            {
                _result = _session.Execute(currentCode);
            }
            catch (CompilationErrorException errorException)
            {
                var run = new Run(errorException.Message);
                run.Foreground = new SolidColorBrush(Colors.Red);
                _flowDocument.Blocks.Add(new Paragraph(run));
            }
            if (_result == null)
            {
                AddStartContentLine();
                return;
            }
            _flowDocument.Blocks.Add(new Paragraph(new Run(_result.ToString())));
            AddStartContentLine();
        }

        private void AddStartContentLine()
        {
            _flowDocument.Blocks.Add(new Paragraph(new Run(_currentDirectory)));
            richTextBoxPrincipal.Document = _flowDocument;
            richTextBoxPrincipal.CaretPosition = richTextBoxPrincipal.CaretPosition.DocumentEnd;
        }
    }
}