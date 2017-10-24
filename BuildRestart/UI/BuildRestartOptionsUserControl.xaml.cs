﻿using BuildRestart.UI.Data;
using System.Windows;
using System.Windows.Controls;

namespace BuildRestart.UI
{
    /// <summary>
    /// Interaction logic for BuildHelperOptionsUserControl.xaml
    /// </summary>
    public partial class BuildRestartOptionsUserControl : UserControl
    {
        private IBuildConfigurationDataSourceProvider m_Options;

        public BuildRestartOptionsUserControl(IBuildConfigurationDataSourceProvider options)
        {
            InitializeComponent();
            m_Options = options;
            DataContext = m_Options;
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            m_Options.DataSource.Configuration.Add(new BuildConfiguration());
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedOption = m_DgOptions.SelectedItem as BuildConfiguration;
            if (selectedOption != null)
            {
                m_Options.DataSource.Configuration.Remove(selectedOption);
            }
        }
    }
}
