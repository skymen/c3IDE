﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using c3IDE.Compiler;
using c3IDE.Managers;
using c3IDE.Server;
using c3IDE.Utilities.Helpers;
using MahApps.Metro.Controls;

namespace c3IDE.Windows
{
    /// <summary>
    /// Interaction logic for PopoutCompileWindow.xaml
    /// </summary>
    public partial class PopoutCompileWindow : MetroWindow
    {
        private readonly int callbackIndex;

        /// <summary>
        /// popout window constructor
        /// </summary>
        public PopoutCompileWindow()
        {
            InitializeComponent();
            WebServerManager.WebServiceUrlChanged = s => Dispatcher.Invoke(() => { UrlTextBox.Text = s; });
            WebServerManager.WebServerStateChanged = b => Dispatcher.Invoke(() =>
            {
                WebServerButton.Content = b ? "Stop Web Server" : "Start Web Server";
            });

            callbackIndex = LogManager.CompilerLog.AddUpdateCallback((s) =>
            {
                Dispatcher.Invoke(() =>
                {
                    LogText.AppendText(s);
                    if (LogText.LineCount > 0)
                    {
                        LogText.ScrollToLine(LogText.LineCount - 1);
                    }
                });
            });
        }

        /// <summary>
        /// handles the closing of the popout window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LogManager.CompilerLog.RemoveCallback(callbackIndex);
        }

        /// <summary>
        /// handles selecting all text when the url text box is in focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectUrl(object sender, RoutedEventArgs e)
        {
            var tb = (sender as TextBox);
            tb?.SelectAll();
            var url = UrlTextBox.Text;
            if (string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    Clipboard.SetText(url);
                    NotificationManager.PublishNotification($"{url} copied to clipboard.");
                }
                catch(Exception ex)
                {
                    LogManager.AddErrorLog(ex);
                }
            }
        }

        /// <summary>
        /// focus on the textbox when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb && !tb.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tb.Focus();
            }
        }

        /// <summary>
        /// handles the web server state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartWebServerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!WebServerManager.WebServerStarted)
                {
                    int.TryParse(OptionsManager.CurrentOptions.Port, out var port);
                    if (port == 0)
                    {
                        port = 8080;
                        LogManager.AddLogMessage("invalid port, fallback to port 8080");
                    }
                    WebServerManager.StartWebServer(port);
                    //AddonCompiler.Insatnce.WebServer = new WebServerClient();
                    //AddonCompiler.Insatnce.WebServer.Start();
                }
                else
                {
                    AddonCompiler.Insatnce.WebServer.Stop();
                }
            }
            catch (Exception ex)
            {
                LogManager.AddErrorLog(ex);
                WebServerManager.WebServerStarted = false;
            }

            WebServerButton.Content = WebServerManager.WebServerStarted ? "Stop Web Server" : "Start Web Server";
            ApplicationWindows.TestWidnow.Update();
        }

        /// <summary>
        /// opens construct from the web or desktop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenConstructButton_Click(object sender, RoutedEventArgs e)
        {
            if (OptionsManager.CurrentOptions.OpenC3InWeb)
            {
                ConstructLauncher.Insatnce.LaunchConstruct(false);
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(OptionsManager.CurrentOptions.C3DesktopPath))
                    {
                        throw new InvalidOperationException("Construct 3 Desktop Path is Invalid");
                    }

                    ProcessHelper.Insatnce.StartProcess(OptionsManager.CurrentOptions.C3DesktopPath);
                }
                catch (Exception ex)
                {
                    LogManager.AddErrorLog(ex);
                    NotificationManager.PublishErrorNotification("Invalid C3 desktop path, please check plath in options");
                }
            }
        }
    }
}
