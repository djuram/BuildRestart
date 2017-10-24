using BuildRestart.UI.Data;

namespace BuildRestart.UI
{
    public interface IBuildConfigurationDataSourceProvider
    {
        BuildConfigurationDataSource DataSource { get; set; }
    }
}