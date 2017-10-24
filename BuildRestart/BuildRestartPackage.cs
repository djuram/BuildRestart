using System;
using System.Runtime.InteropServices;
using BuildRestart.Build;
using BuildRestart.Log;
using BuildRestart.Settings;
using BuildRestart.UI;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;

namespace BuildRestart
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(BuildRestartPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(BuildRestartOptions), "BuildRestart", "BuildRestart options", 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class BuildRestartPackage : Package
    {
        /// <summary>
        /// BuildRestartPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "0206B168-69C9-44C9-A69F-66EEE2D882A7";
        private BuildTracker _buildTracker;
        private DTE2 _vsInstance;

        /// <summary>
        /// Settings for all package
        /// </summary>
        public static BuildRestartSettings Settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildRestartPackage"/> class.
        /// </summary>
        public BuildRestartPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            //Command.Initialize(this);
            base.Initialize();
            var settingsStore = new Lazy<WritableSettingsStore>(() => new ShellSettingsManager(this).
                GetWritableSettingsStore(SettingsScope.UserSettings));

            Settings = new BuildRestartSettings(settingsStore);
            _vsInstance = (DTE2)GetService(typeof(DTE));
            var solutionBuildManager = (IVsSolutionBuildManager2)GetService(typeof(SVsSolutionBuildManager));
            //var statusBar = (IVsStatusbar)GetService(typeof(SVsStatusbar));
            //var logger = new ExtensionLogger(() => GetService(typeof(SVsActivityLog)) as IVsActivityLog, statusBar);
            //never cache the activity log reference
            _buildTracker = new BuildTracker(
                _vsInstance,
                solutionBuildManager,
                Settings
                );
        }

        #endregion
    }
}
