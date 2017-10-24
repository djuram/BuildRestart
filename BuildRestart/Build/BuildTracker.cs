using System;
using System.Collections.Generic;
using System.IO;
using BuildRestart.Settings;
using EnsureThat;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Web.Administration;
using System.Threading;

namespace BuildRestart.Build
{
    internal partial class BuildTracker : IVsUpdateSolutionEvents2
    {
        private readonly IVsSolutionBuildManager2 _buildManager;
        private readonly BuildRestartSettings _settings;
        private readonly DTE2 _vsInstance;
        private readonly Dictionary<string, AppPoolInfo> _stopedAppPool = new Dictionary<string, AppPoolInfo>();

        public BuildTracker(
            DTE2 vsInstance,
            IVsSolutionBuildManager2 buildManager,
            BuildRestartSettings settings
            )
        {
            Ensure.That(() => vsInstance).IsNotNull();
            Ensure.That(() => settings).IsNotNull();
            Ensure.That(() => buildManager).IsNotNull();

            _vsInstance = vsInstance;
            _settings = settings;
            _buildManager = buildManager;
            _buildManager.AdviseUpdateSolutionEvents(this, out uint pdwCookieSolutionBM);
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            var optionsDataSource = _settings.Load();
            optionsDataSource.WithConfiguration(GetSolution(), config => //get configuration for the given solution
            {
                foreach (var appPoolName in config.ParseAppPoolNames())
                {
                    AppPoolInfo appPoolInfo;
                    if (_stopedAppPool.ContainsKey(appPoolName))
                    {
                        appPoolInfo = _stopedAppPool[appPoolName];
                    }
                    else
                    {
                        var serverManager = new ServerManager();
                        var appPool = serverManager.ApplicationPools[appPoolName];
                        appPoolInfo = new AppPoolInfo(config.RestartAppPool, appPool);
                        _stopedAppPool.Add(appPoolName, appPoolInfo);
                    }

                    if (appPoolInfo.ApplicationPool == null)
                        continue;

                    var iteration = 0;
                    while (appPoolInfo.ApplicationPool.State != ObjectState.Stopped)
                    {
                        if (appPoolInfo.ApplicationPool.State == ObjectState.Started)
                            appPoolInfo.ApplicationPool.Stop();
                        iteration++;
                        if (iteration > 10 || appPoolInfo.ApplicationPool.State == ObjectState.Stopped)
                            break;
                        Thread.Sleep(1000);
                    }
                }
            });
            return VSConstants.S_OK;
        }
        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            if (fSucceeded == 1)
            {
                foreach (var pool in _stopedAppPool)
                {
                    var appPoolInfo = pool.Value;
                    if (appPoolInfo.Restart)
                    {
                        if (appPoolInfo.ApplicationPool.State == ObjectState.Stopped)
                            appPoolInfo.ApplicationPool.Start();
                    }
                }
            }
            return VSConstants.S_OK;
        }

        private string GetSolution()
        {
            string fileName = Path.GetFileName(_vsInstance.Solution.FileName);
            return fileName;
        }
    }
}
