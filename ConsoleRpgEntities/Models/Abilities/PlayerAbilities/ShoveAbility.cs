using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    public class ShoveAbility : Ability
    {
        public int Damage { get; set; }
        public int Distance { get; set; }

        public override void Activate(IPlayer user, ITargetable target)
        {
            int totalDamage = Damage + (target.Health - Damage);

            // Fireball ability logic
            Console.WriteLine($"{user.Name} shoves {target.Name} back {Distance} feet, dealing {totalDamage} damage!");
            target.Health -= Damage;

            if (target.Health > 0)
            {
                Console.WriteLine($"{target.Name}'s HP is at {target.Health}.");
            }
            else
            {
                Console.WriteLine($"{target.Name} has been defeated.");
            }
        }
    }
}
