using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CodeCoverageAnalyserWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        Dictionary<string, List<CoverageRangeInfo>> coverageMapDic = new Dictionary<string, List<CoverageRangeInfo>>();

        public MainWindow()
        {
            InitializeComponent();

            // Handle Ctrl+O shortcut manually
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;            
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
            {
                e.Handled = true;
                MenuOpen_Click(sender, e);
            }
        }

        private void LoadCoverageXml(string filePath)
        {
            try
            {
                Dictionary<int, string> sourceFileMap = new Dictionary<int, string> ();
                
                coverageMapDic.Clear();

                FileTreeView.Items.Clear();
                CodeDisplay.Text = "";

                //Loading the XML File
                var loadDocument = XDocument.Load(filePath);
                
                // Parse source files
                var sourceFiles = loadDocument.Descendants("source_file");
                foreach (var sourceFile in sourceFiles)
                {
                    int sourceFileId = Convert.ToInt32(sourceFile.Attribute("id")?.Value);
                    string path = sourceFile.Attribute("path")?.Value;
                    string binFolder = AppDomain.CurrentDomain.BaseDirectory;
                    string actualPath = path.Replace ("BinFolder", binFolder);

                    sourceFileMap.Add(sourceFileId, actualPath);
                    coverageMapDic.Add(actualPath, new List<CoverageRangeInfo>());                    
                }

                // Parse ranges
                var ranges = loadDocument.Descendants("range");
                foreach (var range in ranges)
                {
                    int sourceId = Convert.ToInt32(range.Attribute("source_id")?.Value);
                    
                    var filePathKey = sourceFileMap[sourceId];
                    bool isCover = range.Attribute("covered")?.Value == "yes";
                    int startLine = Convert.ToInt32(range.Attribute("start_line")?.Value);
                    int endLine = Convert.ToInt32(range.Attribute("end_line")?.Value);
                    int startColumn = Convert.ToInt32(range.Attribute("start_column")?.Value);
                    int endColumn = Convert.ToInt32(range.Attribute("end_column")?.Value);

                    var covarageRange = new CoverageRangeInfo
                    {
                        StartLine = startLine,
                        EndLine = endLine,
                        IsCovered = isCover,
                        StartColumn = startColumn,
                        EndColumn = endColumn
                    };

                    coverageMapDic[filePathKey].Add(covarageRange);
                }
                
                foreach (var path in sourceFileMap.Values)
                {
                    AddFileToTree(path, FileTreeView);
                }

                // Expand all folders on load
                foreach (var item in FileTreeView.Items.OfType<TreeViewItem>())
                {
                    item.ExpandSubtree();
                }                

                GenerateTitleFromXml (loadDocument);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
        

        private void AddFileToTree(string filePath, ItemsControl parent)
        {
            var parts = filePath.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            AddNode(parts, 0, parent, filePath);
        }

        private void AddNode(string[] parts, int index, ItemsControl parent, string fullPath)
        {
            if (index >= parts.Length) return;

            // Check if node with this name exists
            var existingNode = parent.Items
                .OfType<TreeViewItem>()
                .FirstOrDefault(i => (string)i.Header == parts[index]);

            if (existingNode == null)
            {
                var newNode = new TreeViewItem { Header = parts[index] };

                // If last part (file), set Tag to full path for loading file on click
                if (index == parts.Length - 1)
                    newNode.Tag = fullPath;

                parent.Items.Add(newNode);
                existingNode = newNode;
            }

            // Recurse into next part (folder/file)
            AddNode(parts, index + 1, existingNode, fullPath);
        }


        private void GenerateTitleFromXml(XDocument doc)
        {
            var module = doc.Descendants("module").First();
            
            if (module == null)
            {
                Title = $"Code coverage analyzer: 0/0 blocks covered: 0.00%";
            }                

            int linesCovered = Convert.ToInt32(module.Attribute("lines_covered")?.Value ?? "0");
            int linesNotCovered = Convert.ToInt32 (module.Attribute("lines_not_covered")?.Value ?? "0");
            int total = linesCovered + linesNotCovered;
            var percentOfLines = total == 0 ? 0 : (linesCovered * 100.0 / total);

            Title= $"Code coverage analyzer: {linesCovered}/{total} blocks covered: {percentOfLines:F2}%";
        }

        private void FileTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (e.NewValue is TreeViewItem item && item.Tag is string filePath)
                {
                    if (File.Exists (filePath))
                    {
                        var lines = File.ReadAllLines (filePath);
                        CodeDisplay.Text = string.Join ("\n", lines);

                        ShowFileWithCoverage (filePath);
                    }
                    else
                    {
                        CodeDisplay.Text = $"File not found: {filePath}";
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error loading file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }           
        }

        private void ShowFileWithCoverage(string filePath)
        {
            CodeDisplay.Text = "";
            ColumnDisplay.Text = "";

            if (!File.Exists(filePath)) 
                return;

            var lines = File.ReadAllLines(filePath);
            var highlights = coverageMapDic.ContainsKey (filePath) ? coverageMapDic[filePath] : new List<CoverageRangeInfo> ();

            for (int i = 0; i < lines.Length; i++)
            {
                int lineNumber = i + 1;
                string lineText = lines[i];

                ColumnDisplay.Text += lineNumber.ToString().PadLeft(3) + " :\n";

                var lineRanges = highlights.Where(r => r.StartLine == lineNumber).ToList();

                if (lineRanges.Count == 0)
                {
                    // No coverage info for this line, add entire line as transparent
                    CodeDisplay.Inlines.Add(new Run(lineText)
                    {
                        Background = Brushes.Transparent,
                        FontFamily = new FontFamily("Consolas")
                    });
                }
                else
                {
                    int currentPos = 0;

                    foreach (var range in lineRanges)
                    {
                        int startCol = range.StartColumn - 1;
                        if (startCol < 0)
                            startCol = 0;

                        int endCol = range.EndColumn - 1;
                        if (endCol >= lineText.Length)
                            endCol = lineText.Length - 1;

                        if (endCol == -1) 
                        {
                            return;
                        }

                        // Add unhighlighted text before coverage range
                        if (startCol > currentPos)
                        {
                            CodeDisplay.Inlines.Add(new Run(lineText.Substring(currentPos, startCol - currentPos))
                            {
                                Background = Brushes.Transparent,
                                FontFamily = new FontFamily("Consolas")
                            });
                        }

                        // Add highlighted text for the coverage range
                        if (endCol >= startCol)
                        {
                            CodeDisplay.Inlines.Add(new Run(lineText.Substring(startCol, endCol - startCol + 1))
                            {
                                Background = range.IsCovered ? Brushes.LightBlue : Brushes.Orange,
                                FontFamily = new FontFamily("Consolas")
                            });
                        }

                        currentPos = endCol + 1;
                    }

                    // Add any remaining unhighlighted text after last range
                    if (currentPos < lineText.Length)
                    {
                        CodeDisplay.Inlines.Add(new Run(lineText.Substring(currentPos))
                        {
                            Background = Brushes.Transparent,
                            FontFamily = new FontFamily("Consolas")
                        });
                    }
                }

                CodeDisplay.Inlines.Add(new LineBreak());
            }
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Coverage XML (*.xml)|*.xml"
            };

            if (dialog.ShowDialog() == true)
            {
                LoadCoverageXml(dialog.FileName);
            }
        }

        private void MenuClear_Click(object sender, RoutedEventArgs e)
        {
            FileTreeView.Items.Clear();
            CodeDisplay.Text = "";
            ColumnDisplay.Text = "";

            Title = "Code coverage analyzer: 0/0 blocks covered: 0.00%";
        }
    }
}
