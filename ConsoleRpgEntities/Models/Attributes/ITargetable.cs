using ConsoleRpgEntities.Models.Equipments;

namespace ConsoleRpgEntities.Models.Attributes;

public interface ITargetable
{
    string Name { get; set; }
    int Health { get; set; }
    Equipment? Equipment { get; set; }
}
