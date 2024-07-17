namespace core.Settings;

public class Settings<T> where T : Settings<T>
{
    public static string Section => typeof(T).Name.EndsWith("Settings")
        ? typeof(T).Name.Replace("Settings", "")
        : throw new ArgumentException($"Invalid class name for {typeof(T).Name}");
}