using BuildRestart.UI.Data;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace BuildRestart.UI
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("4C11F29C-7247-49CE-A962-55AFD031092D")]
    public class BuildRestartOptions : UIElementDialogPage, IBuildConfigurationDataSourceProvider
    {
        BuildConfigurationDataSource m_OptionsDatasource;

        public BuildRestartOptions()
        {
            m_OptionsDatasource = BuildRestartPackage.Settings.Load();
        }

        public BuildConfigurationDataSource DataSource
        {
            get { return m_OptionsDatasource; }
            set { m_OptionsDatasource = value; }
        }

        protected override UIElement Child
        {
            get
            {
                return new BuildRestartOptionsUserControl(this);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            BuildRestartPackage.Settings.Save(m_OptionsDatasource);
            base.OnClosed(e);
        }
    }
}
