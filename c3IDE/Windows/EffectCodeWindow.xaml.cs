﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using c3IDE.DataAccess;
using c3IDE.Managers;
using c3IDE.Utilities.SyntaxHighlighting;
using c3IDE.Windows.Interfaces;
using ICSharpCode.AvalonEdit.Search;

namespace c3IDE.Windows
{
    /// <summary>
    /// Interaction logic for EffectCodeWindow.xaml
    /// </summary>
    public partial class EffectCodeWindow : UserControl, IWindow
    {
        public string DisplayName { get; set; } = "Code";
        private SearchPanel effectPanel;

        public EffectCodeWindow()
        {
            InitializeComponent();

            EffectPluginTextEditor.TextArea.TextEntering += TextEditor_TextEntering;
            EffectPluginTextEditor.TextArea.TextEntered += EffectPluginTextEditor_TextEntered;

            EffectPluginTextEditor.Options.EnableEmailHyperlinks = false;
            EffectPluginTextEditor.Options.EnableHyperlinks = false;

            effectPanel = SearchPanel.Install(EffectPluginTextEditor);
        }



        /// <summary>
        /// handles when the effect code window gets focus
        /// </summary>
        public void OnEnter()
        {
            ThemeManager.SetupTextEditor(EffectPluginTextEditor, Syntax.Javascript);
            ThemeManager.SetupSearchPanel(effectPanel);

            if (AddonManager.CurrentAddon != null)
            {
                EffectPluginTextEditor.Text = AddonManager.CurrentAddon.Effect.Code;
            }
            else
            {
                EffectPluginTextEditor.Text = string.Empty;
            }
        }

        /// <summary>
        /// handles when the effect code window loses focus
        /// </summary>
        public void OnExit()
        {
            if (AddonManager.CurrentAddon != null)
            {
                AddonManager.CurrentAddon.Effect.Code = EffectPluginTextEditor.Text;
                AddonManager.SaveCurrentAddon();
            }
        }

        /// <summary>
        /// clears all inputs on effect code window
        /// </summary>
        public void Clear()
        {
            EffectPluginTextEditor.Text = string.Empty;
        }

        public void ChangeTab(string tab, int lineNum)
        {

        }

        /// <summary>
        /// handles keyboard shortcuts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextEditor_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                WindowManager.MainWindow.Save(true, true);
            }
        }

        //todo - add code completion
        private void EffectPluginTextEditor_TextEntered(object sender, TextCompositionEventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        //todo - add code completion
        private void TextEditor_TextEntering(object sender, TextCompositionEventArgs e)
        {

        }

        private void GenerateUniforms_OnClick(object sender, RoutedEventArgs e)
        {
            var uniformText = string.Join("\n", AddonManager.CurrentAddon.Effect.Parameters.Select(x => x.Value.VariableDeclaration));
            //EffectPluginTextEditor.Text =
            //    EffectPluginTextEditor.Text.Replace("void main(void)", $"{uniformText}\n\nvoid main(void)");

            EffectPluginTextEditor.Text = $"{uniformText}\n\n{EffectPluginTextEditor.Text}";
        }

        private void Compile_OnClick(object sender, RoutedEventArgs e)
        {
            WindowManager.MainWindow.Save(true, true);
        }
    }
}
