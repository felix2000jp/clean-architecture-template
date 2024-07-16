namespace core.Settings;

public class DatabaseSettings : Settings<DatabaseSettings>
{
    public string ConnectionString { get; }
}