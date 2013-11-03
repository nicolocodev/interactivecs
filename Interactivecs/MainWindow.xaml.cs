using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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
        private readonly Session _session;
        private string _currentDirectory;
        private FlowDocument _flowDocument;
        private object _result;

        public MainWindow()
        {
            InitializeComponent();
            var scriptEngine = new ScriptEngine();
            _session = scriptEngine.CreateSession();
            _session.AddReference("System");
            _session.AddReference("System.Core");
            Loaded += MainWindowsLoaded;
        }

        private void MainWindowsLoaded(object sender, RoutedEventArgs e)
        {
            _currentDirectory = ">";
            _flowDocument = new FlowDocument();
            _flowDocument.Blocks.Add(new Paragraph(new Run(_currentDirectory)));
            richTextBoxPrincipal.Document = _flowDocument;
            var observable = Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(
                handler => richTextBoxPrincipal.PreviewKeyDown += handler,
                handler => richTextBoxPrincipal.PreviewKeyDown -= handler,
                Scheduler.Default)
                .Where(x => x.EventArgs.Key == Key.Back || x.EventArgs.Key == Key.Enter || x.EventArgs.Key == Key.Left);

            var subscriber = observable.Subscribe(x => RichTextBoxPreviewKeyDown(x.EventArgs));
        }

        private void RichTextBoxPreviewKeyDown(KeyEventArgs e)
        {
            var start = richTextBoxPrincipal.CaretPosition;
            var text = start.GetTextInRun(LogicalDirection.Backward);
            switch (e.Key)
            {
                case Key.Back:
                    if (text == _currentDirectory)
                        e.Handled = true;
                    break;
                case Key.Enter:
                    string currentCode = richTextBoxPrincipal.CaretPosition.GetTextInRun(LogicalDirection.Backward);
                    ProccessCommand(currentCode);
                    e.Handled = true;
                    break;
                case Key.Left:
                    if (text == _currentDirectory || text == string.Empty)
                        e.Handled = true;
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
                var run = new Run(errorException.Message) { Foreground = new SolidColorBrush(Colors.Red) };
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