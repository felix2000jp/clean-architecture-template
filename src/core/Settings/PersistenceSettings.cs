using System.ComponentModel.DataAnnotations;

namespace core.Settings;

public sealed class PersistenceSettings : Settings<PersistenceSettings>
{
    [Required] public string DatabaseConnection { get; set; } = string.Empty;
}