﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using c3IDE.DataAccess;
using c3IDE.Templates.c3IDE.Templates;
using c3IDE.Windows.Interfaces;
using c3IDE.Models;
using c3IDE.Templates;
using c3IDE.Utilities;
using c3IDE.Utilities.CodeCompletion;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using Action = c3IDE.Models.Action;

namespace c3IDE.Windows
{
    /// <summary>
    /// Interaction logic for ActionWindow.xaml
    /// </summary>
    public partial class ActionWindow : UserControl, IWindow
    {
        //properties
        public string DisplayName { get; set; } = "Actions";
        private Dictionary<string, Action> _actions;
        private Action _selectedAction;
        private CompletionWindow completionWindow;

        //ctor
        public ActionWindow()
        {
            InitializeComponent();

            CodeTextEditor.TextArea.TextEntering += TextEditor_TextEntering;
            CodeTextEditor.TextArea.TextEntered += CodeTextEditor_TextEntered;

            AceTextEditor.TextArea.TextEntering += TextEditor_TextEntering;
            AceTextEditor.TextArea.TextEntered += AceTextEditor_TextEntered;

            LanguageTextEditor.TextArea.TextEntering += TextEditor_TextEntering;
            LanguageTextEditor.TextArea.TextEntered += LanguageTextEditor_TextEntered;
        }

        //editor events
        private void LanguageTextEditor_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Text)) return;
            var allTokens = JavascriptParser.Insatnce.ParseJsonDocument(LanguageTextEditor.Text);

            //add matching closing symbol
            switch (e.Text)
            {
                case "{":
                    LanguageTextEditor.Document.Insert(LanguageTextEditor.TextArea.Caret.Offset, "}");
                    LanguageTextEditor.TextArea.Caret.Offset--;
                    return;

                case "\"":
                    LanguageTextEditor.Document.Insert(LanguageTextEditor.TextArea.Caret.Offset, "\"");
                    LanguageTextEditor.TextArea.Caret.Offset--;
                    return;

                case "[":
                    LanguageTextEditor.Document.Insert(LanguageTextEditor.TextArea.Caret.Offset, "]");
                    LanguageTextEditor.TextArea.Caret.Offset--;
                    return;

                case "(":
                    LanguageTextEditor.Document.Insert(LanguageTextEditor.TextArea.Caret.Offset, ")");
                    LanguageTextEditor.TextArea.Caret.Offset--;
                    return;

                default:
                    //figure out word segment
                    var segment = LanguageTextEditor.TextArea.GetCurrentWord();
                    if (segment == null) return;

                    //get string from segment
                    var text = LanguageTextEditor.Document.GetText(segment);
                    if (string.IsNullOrWhiteSpace(text)) return;

                    //filter completion list by string
                    var data = CodeCompletionFactory.Insatnce.GetCompletionData(allTokens, CodeType.Json).Where(x => x.Text.ToLower().Contains(text)).ToList();
                    if (data.Any())
                    {
                        ShowCompletion(LanguageTextEditor.TextArea, data);
                    }
                    break;
            }
        }

        private void AceTextEditor_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Text)) return;
            var allTokens = JavascriptParser.Insatnce.ParseJsonDocument(AceTextEditor.Text);

            //add matching closing symbol
            switch (e.Text)
            {
                case "{":
                    AceTextEditor.Document.Insert(AceTextEditor.TextArea.Caret.Offset, "}");
                    AceTextEditor.TextArea.Caret.Offset--;
                    return;

                case "\"":
                    AceTextEditor.Document.Insert(AceTextEditor.TextArea.Caret.Offset, "\"");
                    AceTextEditor.TextArea.Caret.Offset--;
                    return;

                case "[":
                    AceTextEditor.Document.Insert(AceTextEditor.TextArea.Caret.Offset, "]");
                    AceTextEditor.TextArea.Caret.Offset--;
                    return;

                case "(":
                    AceTextEditor.Document.Insert(AceTextEditor.TextArea.Caret.Offset, ")");
                    AceTextEditor.TextArea.Caret.Offset--;
                    return;

                default:
                    //figure out word segment
                    var segment = AceTextEditor.TextArea.GetCurrentWord();
                    if (segment == null) return;

                    //get string from segment
                    var text = AceTextEditor.Document.GetText(segment);
                    if (string.IsNullOrWhiteSpace(text)) return;

                    //filter completion list by string
                    var data = CodeCompletionFactory.Insatnce.GetCompletionData(allTokens, CodeType.Json).Where(x => x.Text.ToLower().Contains(text)).ToList();
                    if (data.Any())
                    {
                        ShowCompletion(AceTextEditor.TextArea, data);
                    }
                    break;
            }
        }

        private void CodeTextEditor_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Text)) return;
            var tokenList = JavascriptParser.Insatnce.ParseJavascriptDocument(CodeTextEditor.Text, CodeType.RuntimeJavascript);
            var methodsTokens = JavascriptParser.Insatnce.ParseJavascriptMethodCalls(CodeTextEditor.Text);
            var allTokens = JavascriptParser.Insatnce.DecorateMethodInterfaces(tokenList, methodsTokens, CodeType.RuntimeJavascript);

            //add matching closing symbol
            switch (e.Text)
            {
                case "{":
                    CodeTextEditor.Document.Insert(CodeTextEditor.TextArea.Caret.Offset, "}");
                    CodeTextEditor.TextArea.Caret.Offset--;
                    return;

                case "\"":
                    CodeTextEditor.Document.Insert(CodeTextEditor.TextArea.Caret.Offset, "\"");
                    CodeTextEditor.TextArea.Caret.Offset--;
                    return;

                case "[":
                    CodeTextEditor.Document.Insert(CodeTextEditor.TextArea.Caret.Offset, "]");
                    CodeTextEditor.TextArea.Caret.Offset--;
                    return;

                case "(":
                    CodeTextEditor.Document.Insert(CodeTextEditor.TextArea.Caret.Offset, ")");
                    CodeTextEditor.TextArea.Caret.Offset--;
                    return;
                case ".":
                    var methodsData = CodeCompletionFactory.Insatnce.GetCompletionData(allTokens, CodeType.RuntimeJavascript)
                        .Where(x => x.Type == CompletionType.Methods || x.Type == CompletionType.Modules || x.Type == CompletionType.Misc);
                    ShowCompletion(CodeTextEditor.TextArea, methodsData.ToList());
                    break;
                default:
                    //figure out word segment
                    var segment = CodeTextEditor.TextArea.GetCurrentWord();
                    if (segment == null) return;

                    //get string from segment
                    var text = CodeTextEditor.Document.GetText(segment);
                    if (string.IsNullOrWhiteSpace(text)) return;

                    //filter completion list by string
                    var data = CodeCompletionFactory.Insatnce.GetCompletionData(allTokens, CodeType.RuntimeJavascript).Where(x => x.Text.ToLower().Contains(text)).ToList();
                    if (data.Any())
                    {
                        ShowCompletion(CodeTextEditor.TextArea, data);
                    }
                    break;
            }
        }

        private void TextEditor_TextEntering(object sender, TextCompositionEventArgs e)
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

        private void TextEditor_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && completionWindow != null && completionWindow.CompletionList.SelectedItem == null)
            {
                e.Handled = true;
                completionWindow.CompletionList.ListBox.SelectedIndex = 0;
                completionWindow.CompletionList.RequestInsertion(EventArgs.Empty);
            }
        }

        //completion window
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

        //button clicks 
        private void AddAction_OnClick(object sender, RoutedEventArgs e)
        {
            ActionIdText.Text = "action-id";
            ActionCategoryText.Text = "custom";
            HighlightDropdown.Text = "false";
            DisplayText.Text = "This is the actions display text {0}";
            DescriptionText.Text = "This is the actions description";
            NewActionWindow.IsOpen = true;
        }

        private void RemoveAction_OnClick(object sender, RoutedEventArgs e)
        {
            if (_selectedAction != null)
            {
                _actions.Remove(_selectedAction.Id);
                ActionListBox.ItemsSource = _actions;
                ActionListBox.Items.Refresh();

                //clear editors
                AceTextEditor.Text = string.Empty;
                LanguageTextEditor.Text = string.Empty;
                CodeTextEditor.Text = string.Empty;
                _selectedAction = null;
            }
            else
            {
                AppData.Insatnce.ErrorMessage("failed to remove action, no action selected");
            }
        }

        private void SaveActionButton_Click(object sender, RoutedEventArgs e)
        {
            var id = ActionIdText.Text.ToLower();
            var category = ActionCategoryText.Text;
            var highlight = HighlightDropdown.Text;
            var displayText = DisplayText.Text;
            var desc = DescriptionText.Text;

            if (_actions.ContainsKey(id))
            {
                //TODO: error duplicate id
            }

            var action = new Action
            {
                Id = id.Trim().ToLower(),
                Category = category.Trim().ToLower(),
                Highlight = highlight,
                DisplayText = displayText,
                Description = desc
            };

            action.Ace = TemplateCompiler.Insatnce.CompileTemplates(AppData.Insatnce.CurrentAddon.Template.ActionAces, action);
            action.Language = TemplateCompiler.Insatnce.CompileTemplates(AppData.Insatnce.CurrentAddon.Template.ActionLanguage, action);
            action.Code = TemplateCompiler.Insatnce.CompileTemplates(AppData.Insatnce.CurrentAddon.Template.ActionCode, action);

            _actions.Add(id, action);
            ActionListBox.Items.Refresh();
            ActionListBox.SelectedIndex = _actions.Count - 1;
            NewActionWindow.IsOpen = false;
        }

        private void SaveParamButton_Click(object sender, RoutedEventArgs e)
        {
            var id = ParamIdText.Text.ToLower();
            var type = ParamTypeDropdown.Text;
            var value = ParamValueText.Text;
            var name = ParamNameText.Text;
            var desc = ParamDescText.Text;

            //there is at least one param defined
            if (AceTextEditor.Text.Contains("\"params\": ["))
            {
                //ace param
                var aceTemplate = TemplateHelper.AceParam(id, type, value);
                AceTextEditor.Text = FormatHelper.Insatnce.Json(AceTextEditor.Text.Replace("\"params\": [", aceTemplate));

                //language param
                var langTemplate = TemplateHelper.AceLang(id, type, name, desc);
                LanguageTextEditor.Text = LanguageTextEditor.Text.Replace(@"	""params"": {", langTemplate);

                //code param
                var codeTemplate = TemplateHelper.AceCode(id, _selectedAction.ScriptName);
                CodeTextEditor.Text = CodeTextEditor.Text.Replace($"{_selectedAction.ScriptName}(", codeTemplate);
            }
            //this will be the first param
            else
            {
                //ace param
                var aceTemplate = TemplateHelper.AceParamFirst(id, type, value);
                AceTextEditor.Text = FormatHelper.Insatnce.Json(AceTextEditor.Text.Replace("}", aceTemplate));

                //language param
                var langTemplate = TemplateHelper.AceLangFirst(id, type, name, desc);
                LanguageTextEditor.Text = LanguageTextEditor.Text.Replace(@"""
}", langTemplate);

                //code param
                var codeTemplate = TemplateHelper.AceCodeFirst(id, _selectedAction.ScriptName);
                CodeTextEditor.Text = CodeTextEditor.Text.Replace($"{_selectedAction.ScriptName}()", codeTemplate);
            }

            NewParamWindow.IsOpen = false;
        }

        //window states
        public void OnEnter()
        {
            if (AppData.Insatnce.CurrentAddon != null)
            {
                _actions = AppData.Insatnce.CurrentAddon.Actions;
                ActionListBox.ItemsSource = _actions;

                if (_actions.Any())
                {
                    ActionListBox.SelectedIndex = 0;
                }
            }
            else
            {
                ActionListBox.ItemsSource = null;
                AceTextEditor.Text = string.Empty;
                LanguageTextEditor.Text = string.Empty;
                CodeTextEditor.Text = string.Empty;
            }
          
        }

        public void OnExit()
        {
            if (AppData.Insatnce.CurrentAddon != null)
            {
                //save the current selected action
                if (_selectedAction != null)
                {
                    _selectedAction.Ace = AceTextEditor.Text;
                    _selectedAction.Language = LanguageTextEditor.Text;
                    _selectedAction.Code = CodeTextEditor.Text;
                    _actions[_selectedAction.Id] = _selectedAction;
                }

                AppData.Insatnce.CurrentAddon.Actions = _actions;
                DataAccessFacade.Insatnce.AddonData.Upsert(AppData.Insatnce.CurrentAddon);
            }          
        }

        //list box events
        private void ActionListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActionListBox.SelectedIndex == -1)
            {
                //ignore
                return;
            }

            //save current selection
            if (_selectedAction != null)
            {
                _selectedAction.Ace = AceTextEditor.Text;
                _selectedAction.Language = LanguageTextEditor.Text;
                _selectedAction.Code = CodeTextEditor.Text;
                _actions[_selectedAction.Id] = _selectedAction;
            }

            //load new selection
            _selectedAction = ((KeyValuePair<string, Action>)ActionListBox.SelectedItem).Value;
            AceTextEditor.Text = _selectedAction.Ace;
            LanguageTextEditor.Text = _selectedAction.Language;
            CodeTextEditor.Text = _selectedAction.Code;
        }

        //context menu
        private void InsertNewParam_OnClick(object sender, RoutedEventArgs e)
        {
            if (_selectedAction == null) return;
            ParamIdText.Text = "param-id";
            ParamTypeDropdown.Text = "number";
            ParamValueText.Text = string.Empty;
            ParamNameText.Text = "This is the parameters name";
            ParamDescText.Text = "This is the parameters description";
            NewParamWindow.IsOpen = true;
        }

        private void FormatJavascript_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FormatJsonLang_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FormatJsonAce_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        //view buttons
        private void AceView_OnClick(object sender, RoutedEventArgs e)
        {
            CodePanel.Width = new GridLength(0);
            LangPanel.Width = new GridLength(0);
            AcePanel.Width = new GridLength(3, GridUnitType.Star);
        }

        private void DeafultView_OnClick(object sender, RoutedEventArgs e)
        {
            AcePanel.Width = new GridLength(3, GridUnitType.Star);
            CodePanel.Width = new GridLength(3, GridUnitType.Star);
            LangPanel.Width = new GridLength(3, GridUnitType.Star);
        }

        private void CodeView_OnClick(object sender, RoutedEventArgs e)
        {
            AcePanel.Width = new GridLength(0);
            LangPanel.Width = new GridLength(0);
            CodePanel.Width = new GridLength(3, GridUnitType.Star);
        }

        private void LangView_OnClick(object sender, RoutedEventArgs e)
        {
            AcePanel.Width = new GridLength(0);
            CodePanel.Width = new GridLength(0);
            LangPanel.Width = new GridLength(3, GridUnitType.Star);
        }


    }
}
