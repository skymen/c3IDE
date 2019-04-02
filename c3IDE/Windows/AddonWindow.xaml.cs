﻿using c3IDE.DataAccess;
using c3IDE.Models;
using c3IDE.Utilities.CodeCompletion;
using c3IDE.Windows.Interfaces;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using c3IDE.Managers;
using c3IDE.Templates;
using c3IDE.Utilities.Extentions;
using c3IDE.Utilities.Helpers;
using c3IDE.Utilities.Search;
using c3IDE.Utilities.SyntaxHighlighting;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace c3IDE.Windows
{
    /// <summary>
    /// Interaction logic for AddonWindow.xaml
    /// </summary>
    public partial class AddonWindow : UserControl, IWindow
    {
        public string DisplayName { get; set; } = "Addon";
        private CompletionWindow completionWindow;
        private Dictionary<string, ThirdPartyFile> _files;
        private ThirdPartyFile _selectedFile;
 
        /// <summary>
        /// addon widnow constructor, setup event handeling for auto completion, and setup properties for the editors 
        /// </summary>
        public AddonWindow()
        {
            InitializeComponent();

            AddonTextEditor.TextArea.TextEntering += AddonTextEditor_TextEntering;
            AddonTextEditor.TextArea.TextEntered += AddonTextEditor_TextEntered;

            AddonTextEditor.Options.EnableEmailHyperlinks = false;
            AddonTextEditor.Options.EnableHyperlinks = false;
            FileTextEditor.Options.EnableEmailHyperlinks = false;
            FileTextEditor.Options.EnableHyperlinks = false;
        }

        /// <summary>
        /// handles when the addon widnow becomes the main window
        /// </summary>
        public void OnEnter()
        {
            ThemeManager.SetupTextEditor(AddonTextEditor, Syntax.Json);
            ThemeManager.SetupTextEditor(FileTextEditor, Syntax.Javascript);

            if (AddonManager.CurrentAddon != null)
            {
                AddonTextEditor.Text = AddonManager.CurrentAddon.AddonJson;
                _files = AddonManager.CurrentAddon.ThirdPartyFiles;
                FileListBox.ItemsSource = _files;

                if (_files.Any())
                {
                    FileListBox.SelectedIndex = 0;
                    _selectedFile = ((KeyValuePair<string, ThirdPartyFile>)FileListBox.SelectedItem).Value;
                    FileTextEditor.Text = _selectedFile.Content;
                }
            }
            else
            {
                AddonTextEditor.Text = string.Empty;
                FileListBox.ItemsSource = null;
                FileTextEditor.Text = string.Empty;
            }
        }

        /// <summary>
        /// handles when the addon window is no longer the main window
        /// </summary>
        public void OnExit()
        {
            if (AddonManager.CurrentAddon != null)
            {
                if (_selectedFile != null)
                {
                    _selectedFile.Content = FileTextEditor.Text;
                    _files[_selectedFile.FileName] = _selectedFile;
                }

                AddonManager.CurrentAddon.ThirdPartyFiles = _files;
                AddonManager.CurrentAddon.AddonJson = AddonTextEditor.Text;
                AddonManager.SaveCurrentAddon();
            }
        }

        /// <summary>
        /// clears all the input on the page
        /// </summary>
        public void Clear()
        {
            _files = new Dictionary<string, ThirdPartyFile>();
            _selectedFile = null;
            FileListBox.ItemsSource = null;
            FileTextEditor.Text = string.Empty;
            AddonTextEditor.Text = string.Empty;
        }

        /// <summary>
        /// handles the text entered event for addon.json window, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddonTextEditor_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Text)) return;
            var allTokens = JavascriptParser.Insatnce.ParseJsonDocument(AddonTextEditor.Text);

            if (!TextEditorHelper.Insatnce.MatchSymbol(AddonTextEditor, e.Text))
            {
                //figure out word segment
                var segment = AddonTextEditor.TextArea.GetCurrentWord();
                if (segment == null) return;

                //get string from segment
                var text = AddonTextEditor.Document.GetText(segment);
                if (string.IsNullOrWhiteSpace(text)) return;

                //filter completion list by string
                var data = CodeCompletionFactory.Insatnce.GetCompletionData(allTokens, "addon_json").Where(x => x.Text.ToLower().StartsWith(text.ToLower())).ToList(); ;
                if (data.Any())
                {
                    ShowCompletion(AddonTextEditor.TextArea, data);
                }
            }
        }

        /// <summary>
        /// this handles when to insert the value being used by the autocompletion window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddonTextEditor_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        /// <summary>
        /// this handles the text editor shortcuts to insert data with tab, and to handel the find and replace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextEditor_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && completionWindow != null && completionWindow.CompletionList.SelectedItem == null)
            {
                e.Handled = true;
                completionWindow.CompletionList.ListBox.SelectedIndex = 0;
                completionWindow.CompletionList.RequestInsertion(EventArgs.Empty);
            }
            else if (e.Key == Key.F1)
            {
                Searcher.Insatnce.UpdateFileIndex("addon.json", AddonTextEditor.Text, ApplicationWindows.AddonWindow);
                var editor = ((TextEditor)sender);
                var text = editor.SelectedText;
                Searcher.Insatnce.GlobalFind(text, this);
            }
        }

        /// <summary>
        /// this builds the autocompletion window and displays it
        /// </summary>
        /// <param name="textArea"></param>
        /// <param name="completionList"></param>
        private void ShowCompletion(TextArea textArea, List<GenericCompletionItem> completionList)
        {
            //if any data matches show completion list
            completionWindow = new CompletionWindow(textArea)
            {
                //overwrite color due to global style
                Foreground = new SolidColorBrush(Colors.Black)
            };
            var completionData = completionWindow.CompletionList.CompletionData;
            CodeCompletionDecorator.Insatnce.Decorate(ref completionData, completionList); ;
            completionWindow.Width = 250;
            completionWindow.CompletionList.ListBox.Items.SortDescriptions.Add(new SortDescription("Type", ListSortDirection.Ascending));

            completionWindow.Show();
            completionWindow.Closed += delegate { completionWindow = null; };
        }

        /// <summary>
        /// adds a new third party file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddFile_OnClick(object sender, RoutedEventArgs e)
        {
            var filename = await WindowManager.ShowInputDialog("New File Name?", "please enter the name for the new javascript file", "javascriptFile.js");
            if (string.IsNullOrWhiteSpace(filename)) return;

            var tpf = new ThirdPartyFile
            {
                Content = string.Empty,
                FileName = filename,
                PluginTemplate = TemplateHelper.ThirdPartyFile(filename),
                C3Folder = true,
                C2Folder = false,
                Rootfolder = false
            };

            _files.Add(tpf.FileName, tpf);

            try
            {
                var addon = JObject.Parse(AddonTextEditor.Text);
                var fileList = JArray.Parse(addon["file-list"].ToString());

                if (tpf.C3Folder) fileList.Add("c3runtime/" + tpf.FileName);
                if (tpf.C2Folder) fileList.Add("c2runtime/" + tpf.FileName);
                if (tpf.Rootfolder) fileList.Add(tpf.FileName);

                addon["file-list"] = fileList;
                AddonTextEditor.Text = addon.ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                LogManager.AddErrorLog(ex);
                NotificationManager.PublishErrorNotification($"error parseing json, addon.json not updated => {ex.Message}");
            }
           
            AddonManager.CurrentAddon.ThirdPartyFiles = _files;
            FileListBox.ItemsSource = _files;
            FileListBox.Items.Refresh();
        }

        /// <summary>
        /// removes the selected file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (FileListBox.SelectedIndex == -1)
            {
                NotificationManager.PublishErrorNotification("failed to remove file, no file selected");
                return;
            }

            _selectedFile = ((KeyValuePair<string, ThirdPartyFile>)FileListBox.SelectedItem).Value;
            if (_selectedFile != null)
            {
                _files.Remove(_selectedFile.FileName);
                FileListBox.ItemsSource = _files;
                FileListBox.Items.Refresh();

                AddonManager.CurrentAddon.ThirdPartyFiles = _files;
                AddonManager.SaveCurrentAddon();
                AddonManager.LoadAllAddons();

                //clear editors
                FileTextEditor.Text = string.Empty;

                try
                {
                    var addon = JObject.Parse(AddonTextEditor.Text);
                    var fileList = JArray.Parse(addon["file-list"].ToString());

                    //remove all checks
                    foreach (var item in fileList.Children().ToList())
                    {
                        if (item.ToString().Equals("c3runtime/" + _selectedFile.FileName) ||
                            item.ToString().Equals("c2runtime/" + _selectedFile.FileName) ||
                            item.ToString().Equals(_selectedFile.FileName))
                        {
                            fileList.Remove(item);
                        }
                    }

                    addon["file-list"] = fileList;
                    AddonTextEditor.Text = addon.ToString(Formatting.Indented);
                }
                catch (Exception ex)
                {
                    LogManager.AddErrorLog(ex);
                    NotificationManager.PublishErrorNotification($"error parseing json, addon.json not updated => {ex.Message}");
                }

                _selectedFile = null;
                C3RuntimeFolder.IsChecked = false;
                C2RuntimeFolder.IsChecked = false;
                RootFolder.IsChecked = false;
            }
            else
            {
                NotificationManager.PublishErrorNotification("failed to remove action, no 3rd party files selected");
            }
        }

        /// <summary>
        /// create file copy effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileListBox_OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// imports a thrird party file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileListBox_OnDrop(object sender, DragEventArgs e)
        {
            try
            {
                var file = ((string[])e.Data.GetData(DataFormats.FileDrop))?.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(file))
                {
                    var info = new FileInfo(file);
                    var filename = info.Name;
                    var content = File.ReadAllText(file);
                    var bytes = File.ReadAllBytes(file);

                    _files.Add(filename, new ThirdPartyFile
                    {
                        Content = content,
                        FileName = filename,
                        PluginTemplate = TemplateHelper.ThirdPartyFile(filename),
                        Bytes = bytes,
                        Extention = info.Extension.ToLower()
                    });

                    switch (info.Extension)
                    {
                        case ".js":
                        case ".css":
                        case ".html":
                        case ".json":
                        case ".xml":
                        case ".txt":
                            AddonTextEditor.Text = FormatHelper.Insatnce.Json(AddonTextEditor.Text.Replace(@"file-list"": [", $@"file-list"": [
        ""c3runtime/{filename}"","));
                            break;
                    }

                    CodeCompletionFactory.Insatnce.PopulateUserDefinedTokens(filename, content);
                    FileListBox.ItemsSource = _files;
                    FileListBox.Items.Refresh();
                }
            }
            catch (Exception exception)
            {
                LogManager.AddErrorLog(exception);
                NotificationManager.PublishErrorNotification($"error adding third party file, {exception.Message}");
            }
        }

        /// <summary>
        /// changes the active thrird party file and displays the content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileListBox.SelectedIndex == -1)
            {
                //ignore
                return;
            }

            //save current selection
            if (_selectedFile != null)
            {
                _selectedFile.Content = FileTextEditor.Text;
                _files[_selectedFile.FileName] = _selectedFile;
                AddonManager.CurrentAddon.ThirdPartyFiles = _files;
            }

            //load new selection
            _selectedFile = ((KeyValuePair<string, ThirdPartyFile>)FileListBox.SelectedItem).Value;
            C2RuntimeFolder.IsChecked = _selectedFile.C2Folder;
            C3RuntimeFolder.IsChecked = _selectedFile.C3Folder;
            RootFolder.IsChecked = _selectedFile.Rootfolder;
            FileTextEditor.Text = _selectedFile.Content;
        }

        /// <summary>
        /// apply json formatting to addon.json
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormatJsonMenu_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
            AddonTextEditor.Text = AddonTextEditor.Text = FormatHelper.Insatnce.Json(AddonTextEditor.Text);
            }
            catch (Exception ex)
            {
                LogManager.AddErrorLog(ex);
                NotificationManager.PublishErrorNotification($"failed to format json => {ex.Message}");
            }

        }

        //todo: should we allow formatting thrid party files? if so we need to do it by extention and have different formatting strageties
        /// <summary>
        /// format thrid party file as javascript
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormatJavascriptMenu_OnClick(object sender, RoutedEventArgs e)
        {
            FileTextEditor.Text = FormatHelper.Insatnce.Json(FileTextEditor.Text);
        }

        private void PathCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (FileListBox.SelectedIndex == -1)
            {
                return;
            }

            _selectedFile = ((KeyValuePair<string, ThirdPartyFile>)FileListBox.SelectedItem).Value;
            if (_selectedFile != null)
            {
                _selectedFile.C3Folder = C3RuntimeFolder.IsChecked != null && C3RuntimeFolder.IsChecked.Value;
                _selectedFile.C2Folder = C2RuntimeFolder.IsChecked != null && C2RuntimeFolder.IsChecked.Value;
                _selectedFile.Rootfolder = RootFolder.IsChecked != null && RootFolder.IsChecked.Value;

                //update addon.json
                try
                {
                    var addon = JObject.Parse(AddonTextEditor.Text);
                    var fileList = JArray.Parse(addon["file-list"].ToString());

                    //remove all checks
                    foreach (var item in fileList.Children().ToList())
                    {
                        if (item.ToString().Equals("c3runtime/" + _selectedFile.FileName) ||
                            item.ToString().Equals("c2runtime/" + _selectedFile.FileName) ||
                            item.ToString().Equals(_selectedFile.FileName))
                        {
                            fileList.Remove(item);
                        }
                    }

                    if (_selectedFile.C3Folder) fileList.Add("c3runtime/" + _selectedFile.FileName);
                    if (_selectedFile.C2Folder) fileList.Add("c2runtime/" + _selectedFile.FileName);
                    if (_selectedFile.Rootfolder) fileList.Add(_selectedFile.FileName);

                    addon["file-list"] = fileList;
                    AddonTextEditor.Text = addon.ToString(Formatting.Indented);
                }
                catch (Exception ex)
                {
                    LogManager.AddErrorLog(ex);
                    NotificationManager.PublishErrorNotification($"error parseing json, addon.json not updated => {ex.Message}");
                }
            }
        }
    }
}