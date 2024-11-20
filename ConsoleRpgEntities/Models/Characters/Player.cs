using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using ConsoleRpgEntities.Models.Equipments;
using System;

namespace ConsoleRpgEntities.Models.Characters
{
    public class Player : ITargetable, IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }

        // Foreign key
        public int? EquipmentId { get; set; }

        // Navigation properties
        public virtual Inventory Inventory { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual ICollection<Ability> Abilities { get; set; }

        public void Attack(ITargetable target)
        {
            // "Dice roll"
            Random rand = new Random();
            int roll = rand.Next(1, 7);

            // Equipment stats
            int attack = 0;
            int defense = 0;

            if (Equipment.Armor != null)
            {
                defense = Equipment.Armor.Defense;
            }
            if (Equipment.Weapon != null)
            {
                attack = Equipment.Weapon.Attack;

                // Damage calculations
                decimal damage = (roll + attack) / ((0 + 100) / 100);

                // Total damage
                int totalDamage = (int)Math.Round(damage, MidpointRounding.AwayFromZero);
                int overkill = totalDamage + (target.Health - totalDamage);

                // Player-specific attack logic
                Console.WriteLine($"{Name} attacks {target.Name} with a {Equipment.Weapon.Name}!");
                target.Health -= totalDamage;
                if (target.Health > 0)
                {
                    Console.WriteLine($"{target.Name} took {totalDamage} damage.");
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
            else
            {
                Console.WriteLine($"{Name} has no weapon!", ConsoleColor.Red);
            }
        }

        public void UseAbility(IAbility ability, ITargetable target)
        {
            if (Abilities.Contains(ability))
            {
                ability.Activate(this, target);
            }
            else
            {
                Console.WriteLine($"{Name} does not have the ability {ability.Name}!");
            }
        }

        public void AddToInventory(List<Item> items)
        {
            while (true)
            {
                var itemsToBeAdded = items;
                int counterOne = 0;
                int counterTwo = 0;

                // Player item list                
                Console.WriteLine("Your current items:", ConsoleColor.White);
                for (int i = 0; i < Inventory.Items.Count; i++)
                {
                    counterOne++;
                    Console.WriteLine($"{counterOne}. {Inventory.Items.ElementAt(i).Name}, {Inventory.Items.ElementAt(i).Type}, Attack: {Inventory.Items.ElementAt(i).Attack}, Defense: {Inventory.Items.ElementAt(i).Defense}");

                    for (int j = 0; j < itemsToBeAdded.Count; j++)
                    {
                        if (Inventory.Items.ElementAt(i).Name.Equals(itemsToBeAdded.ElementAt(j).Name))
                        {
                            itemsToBeAdded.RemoveAt(j);
                        }
                    }
                }

                // Items to be added
                Console.WriteLine("\nUnlisted items:", ConsoleColor.White);
                for (int k = 0; k < itemsToBeAdded.Count; k++)
                {
                    counterTwo++;
                    Console.WriteLine($"{counterTwo}. {itemsToBeAdded.ElementAt(k).Name}, {itemsToBeAdded.ElementAt(k).Type}, Attack: {itemsToBeAdded.ElementAt(k).Attack}, Defense: {itemsToBeAdded.ElementAt(k).Defense}");
                }

                // User input
                try
                {
                    Console.WriteLine($"\nTotal items: {items.Count}\nChoose which item to add to your inventory.", ConsoleColor.Cyan);
                    int chosenNumber = Convert.ToInt32(Console.ReadLine());

                    if (chosenNumber > 0 && chosenNumber <= itemsToBeAdded.Count)
                    {
                        var chosenItem = itemsToBeAdded.ElementAt(chosenNumber - 1);
                        Inventory.Items.Add(chosenItem);
                        Console.WriteLine($"{chosenItem.Name} has been added to your inventory.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection.\n", ConsoleColor.Red);
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Invalid input.\n", ConsoleColor.Red);
                }
            }
        }

        public void EquippingInventory(List<Item> items)
        {
            while (true)
            {
                int counter = 0;
                Console.WriteLine("Your current items:", ConsoleColor.White);
                for (int i = 0; i < Inventory.Items.Count; i++)
                {
                    counter++;
                    Console.WriteLine($"{counter}. {Inventory.Items.ElementAt(i).Name}, {Inventory.Items.ElementAt(i).Type}, Attack: {Inventory.Items.ElementAt(i).Attack}, Defense: {Inventory.Items.ElementAt(i).Defense}");
                }

                try
                {
                    Console.WriteLine("Select which item to equip.", ConsoleColor.Cyan);
                    int chosenNumber = Convert.ToInt32(Console.ReadLine());

                    if (chosenNumber > 0 && chosenNumber <= items.Count)
                    {
                        var chosenItem = Inventory.Items.ElementAt(chosenNumber - 1);

                        if (chosenItem.Type.Equals("Weapon"))
                        {
                            Equipment.Weapon = chosenItem;
                            Equipment.WeaponId = chosenItem.Id;
                            Console.WriteLine($"You have equipped {chosenItem.Name}.");
                            break;
                        }
                        else if (chosenItem.Type.Equals("Armor"))
                        {
                            Equipment.Armor = chosenItem;
                            Equipment.ArmorId = chosenItem.Id;
                            Console.WriteLine($"You have equipped {chosenItem.Name}.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"You cannot equip this item.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection.\n", ConsoleColor.Red);
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Invalid input.\n", ConsoleColor.Red);
                }
            }
        }

        public void RemoveFromInventory(List<Item> items)
        {
            while (true)
            {
                var otherItems = items;
                int counter = 0;

                Console.WriteLine("Your current items:", ConsoleColor.White);
                for (int i = 0; i < Inventory.Items.Count; i++)
                {
                    counter++;
                    if (Inventory.Items.ElementAt(i).Name.Equals(Equipment.Weapon.Name) || Inventory.Items.ElementAt(i).Name.Equals(Equipment.Armor.Name))
                    {
                        Console.WriteLine($"{counter}. {Inventory.Items.ElementAt(i).Name}, {Inventory.Items.ElementAt(i).Type}, Attack: {Inventory.Items.ElementAt(i).Attack}, Defense: {Inventory.Items.ElementAt(i).Defense} (Equipped)");
                    }
                    else
                    {
                        Console.WriteLine($"{counter}. {Inventory.Items.ElementAt(i).Name}, {Inventory.Items.ElementAt(i).Type}, Attack: {Inventory.Items.ElementAt(i).Attack}, Defense: {Inventory.Items.ElementAt(i).Defense}");
                    }
                }

                try
                {
                    Console.WriteLine("Select which item to remove.", ConsoleColor.Cyan);
                    Console.WriteLine("Warning. Doing this will also de-equip the selected item.", ConsoleColor.Red);
                    int chosenNumber = Convert.ToInt32(Console.ReadLine());

                    if (chosenNumber > 0 && chosenNumber <= items.Count)
                    {
                        var chosenItem = Inventory.Items.ElementAt(chosenNumber - 1);
                        otherItems.Add(chosenItem);
                        if (chosenItem.Type.Equals("Weapon"))
                        {
                            this.Equipment.Weapon = null;
                            Equipment.WeaponId = null;
                        }
                        else if (chosenItem.Type.Equals("Armor"))
                        {
                            this.Equipment.Armor = null;
                            Equipment.ArmorId = null;
                        }
                        Inventory.Items.Remove(chosenItem);
                        Console.WriteLine($"{chosenItem.Name} has been removed from your inventory.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection.\n", ConsoleColor.Red);
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Invalid input.\n", ConsoleColor.Red);
                }
            }
        }
    }
}
