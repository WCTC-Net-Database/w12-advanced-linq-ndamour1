using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    public class Goblin : Monster
    {
        public int Sneakiness { get; set; }

        public override void Attack(ITargetable target)
        {
            {
                // "Dice roll"
                Random rand = new Random();
                int roll = rand.Next(1, 7);

                // Equipment stats
                int attack = 0;
                int defense = 0;

                // Damage calculations
                decimal damage = 0;

                if (target.Equipment.Armor != null)
                {
                    damage = (roll + attack) / ((target.Equipment.Armor.Defense + 100) / 100);
                }
                else
                {
                    damage = roll + attack;
                }

                // Total damage
                int totalDamage = (int)Math.Round(damage, MidpointRounding.AwayFromZero);
                int overkill = totalDamage + (target.Health - totalDamage);

                // Goblin-specific attack logic
                Console.WriteLine($"{Name} sneaks up and attacks {target.Name}!");
                target.Health -= totalDamage;
                if (target.Health > 0)
                {
                    Console.WriteLine($"{target.Name} took {totalDamage} damage.\n{target.Name}'s HP is at {target.Health}.");
                }
                else
                {
                    if (target.Health - totalDamage < 0)
                    {
                        Console.WriteLine($"{target.Name} took {overkill} damage.\n{target.Name} has been defeated.");
                    }
                    else if (target.Health - totalDamage == 0)
                    {
                        Console.WriteLine($"{target.Name} took {totalDamage} damage.\n{target.Name} has been defeated.");
                    }
                }
            }
        }
    }
}
