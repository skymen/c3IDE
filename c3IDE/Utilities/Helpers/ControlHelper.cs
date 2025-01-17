﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace c3IDE.Utilities.Helpers
{
    public class ControlHelper : Singleton<ControlHelper>
    {
        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public void PopulateComboBox(ComboBox cmbBox, params object[] items)
        {
            foreach (var item in items)
            {
                cmbBox.Items.Add(item);
            }
            cmbBox.Items.Refresh();
        }

        public bool IsWindowOpen<T>(string name = "") where T : MetroWindow
        {
            return string.IsNullOrWhiteSpace(name)
                ? Application.Current.Windows.OfType<T>().Any()
                : Application.Current.Windows.OfType<T>().Any(x => x.Name.Equals(name));
        } 
    }


}
