using EnsureThat;
using Microsoft.VisualStudio.Settings;
using System;
using System.Collections.ObjectModel;
using BuildRestart.UI.Data;
using Newtonsoft.Json;

namespace BuildRestart.Settings
{
    public class BuildRestartSettings
    {
        private const string PropertyName = "Settings";
        private static readonly string s_CollectionPath;
        private readonly Lazy<WritableSettingsStore> m_Store;

        private static BuildConfigurationDataSource s_DefaultDataSource = new BuildConfigurationDataSource()
        {
            Configuration = new ObservableCollection<BuildConfiguration>
            {
                new BuildConfiguration
                {
                    SolutionName = @"vs2012.srcajxvc.Mini - trunk.sln",

                    AppPoolName = "djura",
                    RestartAppPool = true
                }
            }
        };

        static BuildRestartSettings()
        {
            s_CollectionPath = typeof(BuildRestartSettings).FullName;
        }

        public BuildRestartSettings(Lazy<WritableSettingsStore> settingsStore)
        {
            Ensure.That(() => settingsStore).IsNotNull();
            m_Store = settingsStore;
        }

        public void Save(BuildConfigurationDataSource options)
        {
            if (!m_Store.Value.CollectionExists(s_CollectionPath))
                m_Store.Value.CreateCollection(s_CollectionPath);

            m_Store.Value.SetString(s_CollectionPath, PropertyName, Serialize(options));
        }

        public BuildConfigurationDataSource Load()
        {
            if (!m_Store.Value.CollectionExists(s_CollectionPath))
                return s_DefaultDataSource;

            return Deserialize(m_Store.Value.GetString(s_CollectionPath, PropertyName));
        }

        private string Serialize(BuildConfigurationDataSource options)
        {
            return JsonConvert.SerializeObject(options);
        }

        private BuildConfigurationDataSource Deserialize(string options)
        {
            return JsonConvert.DeserializeObject<BuildConfigurationDataSource>(options);
        }
    }
}
