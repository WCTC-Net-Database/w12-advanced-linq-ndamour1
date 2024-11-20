using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Equipments;

namespace ConsoleRpgEntities.Models.Characters;

public interface IPlayer
{
    int Id { get; set; }
    string Name { get; set; }
    int Health { get; set; }
    Inventory Inventory { get; set; }

    ICollection<Ability> Abilities { get; set; }

    void Attack(ITargetable target);
    void UseAbility(IAbility ability, ITargetable target);
    void AddToInventory(List<Item> items);
    void EquippingInventory(List<Item> items);
    void RemoveFromInventory(List<Item> items);
}
