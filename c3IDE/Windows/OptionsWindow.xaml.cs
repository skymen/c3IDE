﻿using c3IDE.DataAccess;
using c3IDE.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using c3IDE.Managers;
using c3IDE.Utilities.Helpers;
using c3IDE.Windows.Interfaces;


namespace c3IDE.Windows
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : UserControl, IWindow
    {
        public string DisplayName { get; set; } = "Options";

        /// <summary>
        /// option window constructor
        /// </summary>
        public OptionsWindow()
        {
            InitializeComponent();
            ControlHelper.Insatnce.PopulateComboBox(FontSizeCombo, 
                8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f ,18f, 19f, 20f, 21f, 22f, 23f, 24f, 25f);

            ControlHelper.Insatnce.PopulateComboBox(FontFamilyCombo, "Consolas", "Calibri", "Cambria", "Candara",
                "Constantia", "Corbel", "Lucida Console, Monaco, monospace", "Courier New, Courier, monospace", "Arial, Helvetica, sans-serif",
                "Courier New", "Georgia, serif", "Trebuchet MS, Helvetica, sans-serif", "Verdana, Geneva, sans-serif", "Tahoma, Geneva, sans-serif",
                "Palatino Linotype, Book Antiqua, Palatino, serif", "Lucida Sans Unicode, Lucida Grande, sans-serif");
        }

        /// <summary>
        /// handles when the options windows get focus
        /// </summary>
        public void OnEnter()
        {
            CompilePathText.Text = OptionsManager.CurrentOptions.CompilePath;
            ExportPathText.Text = OptionsManager.CurrentOptions.ExportPath;
            DataPathText.Text = OptionsManager.CurrentOptions.DataPath;
            DefaultAuthorTextBox.Text = OptionsManager.CurrentOptions.DefaultAuthor;
            C3AddonPathText.Text = OptionsManager.CurrentOptions.C3AddonPath;
            FontSizeCombo.Text = OptionsManager.CurrentOptions.FontSize.ToString(CultureInfo.CurrentCulture);
            FontFamilyCombo.Text = OptionsManager.CurrentOptions.FontFamily;
            ThemeCombo.Text = OptionsManager.CurrentOptions.ThemeKey;
            IncludeTimeStamp.IsChecked = OptionsManager.CurrentOptions.IncludeTimeStampOnExport;
            OpenC3InWeb.IsChecked = OptionsManager.CurrentOptions.OpenC3InWeb;
            C3DesktopPathText.Text = OptionsManager.CurrentOptions.C3DesktopPath;
            PinMainMenu.IsChecked = OptionsManager.CurrentOptions.PinMenu;
            CompileOnSave.IsChecked = OptionsManager.CurrentOptions.CompileOnSave;
            ExportSingleProjectFile.IsChecked = OptionsManager.CurrentOptions.ExportSingleFileProject;
            OverwriteGuidOnImport.IsChecked = OptionsManager.CurrentOptions.OverwriteGuidOnImport;
            RemoveConsoleLogsOnCompile.IsChecked = OptionsManager.CurrentOptions.RemoveConsoleLogsOnCompile;
            UseC2ParsingService.IsChecked = OptionsManager.CurrentOptions.UseC2ParserService;
            C3BetaUrl.Text = OptionsManager.CurrentOptions.BetaUrl;
            C3StableUrl.Text = OptionsManager.CurrentOptions.StableUrl;
            OpenConstructInBeta.IsChecked = OptionsManager.CurrentOptions.OpenConstructInBeta;
            AutoIncrementVersionOnPublish.IsChecked = OptionsManager.CurrentOptions.AutoIncrementVersionOnPublish;
            DefaultPort.Text = OptionsManager.CurrentOptions.Port;
            DisableCodeFormatting.IsChecked = OptionsManager.CurrentOptions.DisableCodeFormatting;
        }

        /// <summary>
        /// handles whne the option window loses focus
        /// </summary>
        public void OnExit()
        {
            if (OptionsManager.CurrentOptions != null)
            {
                OptionsManager.CurrentOptions = new Options
                {
                    CompilePath = !string.IsNullOrWhiteSpace(CompilePathText.Text) ? CompilePathText.Text : OptionsManager.DefaultOptions.CompilePath,
                    ExportPath = !string.IsNullOrWhiteSpace(ExportPathText.Text) ? ExportPathText.Text : OptionsManager.DefaultOptions.ExportPath ,
                    DataPath = !string.IsNullOrWhiteSpace(DataPathText.Text) ? DataPathText.Text : OptionsManager.DefaultOptions.DataPath,
                    DefaultAuthor = !string.IsNullOrWhiteSpace(DefaultAuthorTextBox.Text) ? DefaultAuthorTextBox.Text : OptionsManager.DefaultOptions.DefaultAuthor,
                    C3AddonPath = !string.IsNullOrWhiteSpace(C3AddonPathText.Text) ? C3AddonPathText.Text : OptionsManager.DefaultOptions.C3AddonPath,
                    FontSize = Convert.ToDouble(FontSizeCombo.Text),
                    FontFamily = !string.IsNullOrWhiteSpace(FontFamilyCombo.Text) ? FontFamilyCombo.Text : OptionsManager.DefaultOptions.FontFamily,
                    ThemeKey = !string.IsNullOrWhiteSpace(ThemeCombo.Text) ? ThemeCombo.Text : OptionsManager.DefaultOptions.ThemeKey,
                    IncludeTimeStampOnExport = IncludeTimeStamp.IsChecked != null && IncludeTimeStamp.IsChecked.Value,
                    OpenC3InWeb = OpenC3InWeb.IsChecked != null && OpenC3InWeb.IsChecked.Value,
                    C3DesktopPath = C3DesktopPathText.Text.Contains(".exe") ? C3DesktopPathText.Text : System.IO.Path.Combine(C3DesktopPathText.Text, "Construct3.exe"),
                    PinMenu = PinMainMenu.IsChecked != null && PinMainMenu.IsChecked.Value,
                    CompileOnSave = CompileOnSave.IsChecked != null && CompileOnSave.IsChecked.Value,
                    ExportSingleFileProject = ExportSingleProjectFile.IsChecked != null && ExportSingleProjectFile.IsChecked.Value,
                    OverwriteGuidOnImport = OverwriteGuidOnImport.IsChecked != null && OverwriteGuidOnImport.IsChecked.Value,
                    RemoveConsoleLogsOnCompile = RemoveConsoleLogsOnCompile.IsChecked != null && RemoveConsoleLogsOnCompile.IsChecked.Value,
                    UseC2ParserService = UseC2ParsingService.IsChecked != null && UseC2ParsingService.IsChecked.Value,
                    BetaUrl = C3BetaUrl.Text,
                    StableUrl = C3StableUrl.Text,
                    OpenConstructInBeta = OpenConstructInBeta.IsChecked != null && OpenConstructInBeta.IsChecked.Value,
                    AutoIncrementVersionOnPublish = AutoIncrementVersionOnPublish.IsChecked != null && AutoIncrementVersionOnPublish.IsChecked.Value,
                    Port = !string.IsNullOrWhiteSpace(DefaultPort.Text) ? DefaultPort.Text : OptionsManager.DefaultOptions.Port,
                    DisableCodeFormatting = DisableCodeFormatting.IsChecked != null && DisableCodeFormatting.IsChecked.Value
            };

                //create exports folder if it does not exists
                if (!System.IO.Directory.Exists(OptionsManager.CurrentOptions.DataPath)) Directory.CreateDirectory(OptionsManager.CurrentOptions.DataPath);
                if (!System.IO.Directory.Exists(OptionsManager.CurrentOptions.ExportPath)) Directory.CreateDirectory(OptionsManager.CurrentOptions.ExportPath);
                if (!System.IO.Directory.Exists(OptionsManager.CurrentOptions.CompilePath)) Directory.CreateDirectory(OptionsManager.CurrentOptions.CompilePath);
                if (!System.IO.Directory.Exists(OptionsManager.CurrentOptions.C3AddonPath)) Directory.CreateDirectory(OptionsManager.CurrentOptions.C3AddonPath);

                //persist options
                OptionsManager.SaveOptions();
            }
        }

        /// <summary>
        /// clears all options inputs
        /// </summary>
        public void Clear()
        {
        }

        public void ChangeTab(string tab, int lineNum)
        {

        }

        /// <summary>
        /// wipes all data in local database (clears addon list, options...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ClearDataButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await WindowManager.ShowDialog("Delete saved addon data", "All saved addon's will be deleted");

            if (result)
            {
                DataAccessFacade.Insatnce.AddonData.ResetCollection();
                DataAccessFacade.Insatnce.OptionData.Delete(OptionsManager.CurrentOptions);
                AddonManager.CurrentAddon = null;
                AddonManager.AllAddons= new List<C3Addon>();
                OptionsManager.CurrentOptions = OptionsManager.DefaultOptions;
            }
        }

        /// <summary>
        /// opens the compiled folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCompiledButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessHelper.Insatnce.StartProcess(CompilePathText.Text);
            }
            catch (Exception ex)
            {
                LogManager.AddErrorLog(ex);
                NotificationManager.PublishErrorNotification($"error opening compiled folder, {ex.Message}");
            }
        }

        /// <summary>
        /// opens export folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenExportButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessHelper.Insatnce.StartProcess(ExportPathText.Text);
            }
            catch (Exception ex)
            {
                LogManager.AddErrorLog(ex);
                NotificationManager.PublishErrorNotification($"error opening export folder, {ex.Message}");
            }
        }

        /// <summary>
        /// opens data folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDataButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessHelper.Insatnce.StartProcess(DataPathText.Text);
            }
            catch (Exception ex)
            {
                LogManager.AddErrorLog(ex);
                NotificationManager.PublishErrorNotification($"error opening data folder, {ex.Message}");
            }
        }

        /// <summary>
        /// opens c3addon folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenAddonButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessHelper.Insatnce.StartProcess(C3AddonPathText.Text);
            }
            catch (Exception ex)
            {
                LogManager.AddErrorLog(ex);
                NotificationManager.PublishErrorNotification($"error opening c3addon folder, {ex.Message}");
            }
        }

        /// <summary>
        /// resets current options to default values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetOptionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            OptionsManager.CurrentOptions = OptionsManager.DefaultOptions;
            OptionsManager.SaveOptions();

            //create exports folder if it does not exists
            if (!System.IO.Directory.Exists(OptionsManager.CurrentOptions.DataPath)) Directory.CreateDirectory(OptionsManager.CurrentOptions.DataPath);
            if (!System.IO.Directory.Exists(OptionsManager.CurrentOptions.ExportPath)) Directory.CreateDirectory(OptionsManager.CurrentOptions.ExportPath);
            if (!System.IO.Directory.Exists(OptionsManager.CurrentOptions.CompilePath)) Directory.CreateDirectory(OptionsManager.CurrentOptions.CompilePath);
            if (!System.IO.Directory.Exists(OptionsManager.CurrentOptions.C3AddonPath)) Directory.CreateDirectory(OptionsManager.CurrentOptions.C3AddonPath);
        }
        
        /// <summary>
        /// handles when the font size combo box is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontSizeCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var size = Convert.ToDouble(FontSizeCombo.Text);
            OptionsManager.CurrentOptions.FontSize = size;
            OptionsManager.SaveOptions();
        }

        /// <summary>
        /// handles when the font family combo box changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontFamilyCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var font = FontFamilyCombo.Text;
            OptionsManager.CurrentOptions.FontFamily = font;
            FontFamilyCombo.FontFamily = new FontFamily(font);
            OptionsManager.SaveOptions();
        }

        /// <summary>
        /// handles when the theme combo box changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThemeCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = ((ComboBoxItem)ThemeCombo.SelectedItem).Content;
            OptionsManager.CurrentOptions.ThemeKey = selection.ToString();
            OptionsManager.SaveOptions();
            ThemeManager.SetupTheme();
        }

        /// <summary>
        /// handles when the pin menu is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PinMainMenu_OnChecked(object sender, RoutedEventArgs e)
        {
            OptionsManager.CurrentOptions.PinMenu = PinMainMenu.IsChecked != null && PinMainMenu.IsChecked.Value;
            OptionsManager.OptionChangedCallback(OptionsManager.CurrentOptions);
            OptionsManager.SaveOptions();
        }

        /// <summary>
        /// handles when the compile saved button is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompileOnSave_OnChecked(object sender, RoutedEventArgs e)
        {
            OptionsManager.CurrentOptions.CompileOnSave = CompileOnSave.IsChecked != null && CompileOnSave.IsChecked.Value;
            OptionsManager.OptionChangedCallback(OptionsManager.CurrentOptions);
            OptionsManager.SaveOptions();
        }

        private void ExportSingleProjectFile_OnChecked(object sender, RoutedEventArgs e)
        {
            OptionsManager.CurrentOptions.ExportSingleFileProject = ExportSingleProjectFile.IsChecked != null  && ExportSingleProjectFile.IsChecked.Value;
            OptionsManager.SaveOptions();
        }

        private void OverwriteGuidOnImport_OnChecked(object sender, RoutedEventArgs e)
        {
            OptionsManager.CurrentOptions.OverwriteGuidOnImport = OverwriteGuidOnImport.IsChecked != null && OverwriteGuidOnImport.IsChecked.Value;
            OptionsManager.SaveOptions();
        }


        private void RemoveConsoleLogsOnCompile_OnChecked(object sender, RoutedEventArgs e)
        {
            OptionsManager.CurrentOptions.RemoveConsoleLogsOnCompile = RemoveConsoleLogsOnCompile.IsChecked != null && RemoveConsoleLogsOnCompile.IsChecked.Value;
            OptionsManager.SaveOptions();
        }

        private void UseC2ParsingService_OnChecked(object sender, RoutedEventArgs e)
        {
            OptionsManager.CurrentOptions.UseC2ParserService = UseC2ParsingService.IsChecked != null && UseC2ParsingService.IsChecked.Value;
            OptionsManager.SaveOptions();
        }

        private void OpenConstructInBeta_OnChecked(object sender, RoutedEventArgs e)
        {
            OptionsManager.CurrentOptions.OpenConstructInBeta = OpenConstructInBeta.IsChecked != null && OpenConstructInBeta.IsChecked.Value;
            OptionsManager.SaveOptions();
        }

        private void DisableCodeFormatting_OnChecked(object sender, RoutedEventArgs e)
        {
            OptionsManager.CurrentOptions.DisableCodeFormatting = DisableCodeFormatting.IsChecked != null && DisableCodeFormatting.IsChecked.Value;
            OptionsManager.SaveOptions();
        }

        private void AutoIncrementVersionOnPublish_OnChecked(object sender, RoutedEventArgs e)
        {
            OptionsManager.CurrentOptions.AutoIncrementVersionOnPublish = AutoIncrementVersionOnPublish.IsChecked != null && AutoIncrementVersionOnPublish.IsChecked.Value;
            OptionsManager.SaveOptions();
        }

        private void OpenImportLogButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessHelper.Insatnce.StartProcess("notepad.exe", Path.Combine(DataPathText.Text, "import.log"));
            }
            catch (Exception ex)
            {
                LogManager.AddErrorLog(ex);
                NotificationManager.PublishErrorNotification($"error opening import log, {ex.Message}");
            }
        }

        private void UpdateConstructVersionButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ConstructLauncher.Insatnce.UpdateVersions();
                C3StableUrl.Text = OptionsManager.CurrentOptions.StableUrl;
                C3BetaUrl.Text = OptionsManager.CurrentOptions.BetaUrl;
                NotificationManager.PublishNotification("construct version links updated successfully");
            }
            catch (Exception ex)
            {
                LogManager.AddErrorLog(ex);
                NotificationManager.PublishErrorNotification($"failed to update construct 3 version => {ex.Message}");
            }  
        }


    }   
}
