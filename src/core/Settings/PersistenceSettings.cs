using System.ComponentModel.DataAnnotations;

namespace core.Settings;

public sealed class PersistenceSettings : Settings<PersistenceSettings>
{
    [Required] public required string DatabaseConnection { get; set; }
}