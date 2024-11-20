using ConsoleRpg.Helpers;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Equipments;

namespace ConsoleRpg.Services;

public class GameEngine
{
    private readonly GameContext _context;
    private readonly MenuManager _menuManager;
    private readonly OutputManager _outputManager;

    private IPlayer _player;
    private IMonster _goblin;
    private List<Item> _items;
    private ICollection<Item> _playerItems;

    public GameEngine(GameContext context, MenuManager menuManager, OutputManager outputManager)
    {
        _menuManager = menuManager;
        _outputManager = outputManager;
        _context = context;
    }

    public void Run()
    {
        if (_menuManager.ShowMainMenu())
        {
            SetupGame();
        }
    }

    private void GameLoop()
    {
        _outputManager.Clear();
        int restorePlayerHealth = _context.Players.OfType<Player>().FirstOrDefault().Health;
        int restoreGoblinHealth = _context.Monsters.OfType<Goblin>().FirstOrDefault().Health;

        while (true)
        {
            if (_player.Health > 0 && _goblin.Health > 0)
            {
                _outputManager.WriteLine("Choose an action:", ConsoleColor.Cyan);
                _outputManager.WriteLine("1. Attack");
                _outputManager.WriteLine("2. Alter inventory");
                _outputManager.WriteLine("3. Seach inventory");
                _outputManager.WriteLine("4. Quit");

                _outputManager.Display();

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AttackCharacter();
                        break;
                    case "2":
                        AlterInventory();
                        break;
                    case "3":
                        SearchItems();
                        break;
                    case "4":
                        _outputManager.WriteLine("Exiting game...", ConsoleColor.Red);
                        _outputManager.Display();
                        Environment.Exit(0);
                        break;
                    default:
                        _outputManager.WriteLine("Invalid selection. Please choose 1.", ConsoleColor.Red);
                        break;
                }
            }
            else
            {
                if (_player.Health > 0 && _goblin.Health <= 0)
                {
                    _outputManager.WriteLine($"{_player.Name} wins!", ConsoleColor.Yellow);
                }
                else if (_player.Health <= 0 && _goblin.Health > 0)
                {
                    _outputManager.WriteLine($"{_player.Name} has fallen in battle.", ConsoleColor.Gray);
                }

                _outputManager.WriteLine($"Would you like to play again?");
                _outputManager.WriteLine("1. Yes");
                _outputManager.WriteLine("2. No");
                _outputManager.Display();

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        _outputManager.Clear();
                        _player.Health = restorePlayerHealth;
                        _goblin.Health = restoreGoblinHealth;
                        break;
                    case "2":
                        _outputManager.WriteLine("Exiting game...", ConsoleColor.Red);
                        _outputManager.Display();
                        Environment.Exit(0);
                        break;
                    default:
                        _outputManager.WriteLine("Invalid selection. Please choose a valid option.", ConsoleColor.Red);
                        break;
                }
            }
        }
    }

    private void AttackCharacter()
    {
        _outputManager.Clear();
        if (_goblin is ITargetable targetableGoblin)
        {
            _player.Attack(targetableGoblin);
            _player.UseAbility(_player.Abilities.First(), targetableGoblin);
        }
        if (_player is ITargetable targetablePlayer)
        {
            _goblin.Attack(targetablePlayer);
        }
    }

    private void AlterInventory()
    {
        _outputManager.Clear();
        while (true)
        {
            _outputManager.WriteLine("Choose what to do with your inventory:", ConsoleColor.Cyan);
            _outputManager.WriteLine("1. Add items");
            _outputManager.WriteLine("2. Equip items");
            _outputManager.WriteLine("3. Remove items");
            _outputManager.Display();
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    _player.AddToInventory(_items);
                    break;
                case "2":
                    _player.EquippingInventory(_items);
                    break;
                case "3":
                    _player.RemoveFromInventory(_items);
                    break;
                default:
                    _outputManager.WriteLine("Invalid selection.", ConsoleColor.Red);
                    break;
            }

            if (input == "1" || input == "2" || input == "3")
            {
                break;
            }
        }
    }

    private void SearchItems()
    {
        _player = _context.Players.FirstOrDefault();
        _playerItems = _player.Inventory.Items;

        _outputManager.Clear();
        int nameCounter = 0;

        while (true)
        {
            _outputManager.WriteLine("Choose a search method:", ConsoleColor.Cyan);
            _outputManager.WriteLine("1. Name");
            _outputManager.WriteLine("2. Group by type");
            _outputManager.WriteLine("3. Sort");
            _outputManager.Display();
            var input = Console.ReadLine();
            var items = _playerItems;

            switch (input)
            {
                case "1":
                    while (true)
                    {
                        _outputManager.WriteLine("Write the item's name:");
                        _outputManager.Display();
                        string name = Console.ReadLine();

                        // LINQ search for right name
                        var result = _playerItems.Where(i => i.Name.ToLower().Contains(name.ToLower())).ToList();
                        var nameMatches = new List<Item>();
                        for (int i = 0; i < result.Count(); ++i)
                        {
                            var currentItem = result.ElementAt(i);
                            if (currentItem.Name.ToLower().Contains(name.ToLower()))
                            {
                                nameMatches.Add(currentItem);
                                nameCounter++;
                            }
                        }
                        if (nameCounter > 0)
                        {
                            items = result;
                            foreach (var item in items)
                            {
                                var nameItem = item as Item;
                                _outputManager.WriteLine($"{nameItem.Name}, {nameItem.Type}, Attack: {nameItem.Attack}, Defense: {nameItem.Defense}");
                            }
                            break;
                        }
                        else
                        {
                            _outputManager.WriteLine($"There is no item that contains \"{name}\" in it\'s name.", ConsoleColor.Red);
                            _outputManager.Display();
                        }
                    }
                    break;
                case "2":
                    var query = items.GroupBy(i => i.Type);
                    int counter = 0;
                    foreach (var result in query)
                    {
                        _outputManager.WriteLine(result.Key);
                        var groupedItems = items.Where(g => g.Type.Equals(result.Key));
                        foreach (var groupedItem in groupedItems)
                        {
                            counter++;
                            _outputManager.WriteLine($"     {groupedItem.Name}, {groupedItem.Type}, Attack: {groupedItem.Attack}, Defense: {groupedItem.Defense}");
                        }
                        _outputManager.WriteLine($"Total: {counter}\n");
                    }
                    break;
                case "3":
                    SortItems(items);
                    break;
                default:
                    _outputManager.WriteLine("Invalid selection.", ConsoleColor.Red);
                    break;
            }
            break;
        }
    }
    private void SortItems(ICollection<Item> items)
    {
        _outputManager.WriteLine("Choose how to sort the items:", ConsoleColor.Cyan);
        _outputManager.Display();
        while (true)
        {
            _outputManager.WriteLine("1. Name");
            _outputManager.WriteLine("2. Attack");
            _outputManager.WriteLine("3. Defense");
            _outputManager.Display();
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    var nameList = items.OrderBy(i => i.Name).ToList();
                    for (int i = 0; i < nameList.Count(); ++i)
                    {
                        var currentName = nameList.ElementAt(i);
                        _outputManager.WriteLine($"{currentName.Name}, {currentName.Type}, Attack: {currentName.Attack}, Defense: {currentName.Defense}");
                    }
                    break;
                case "2":
                    var attackList = items.OrderByDescending(a => a.Attack);
                    for (int i = 0; i < attackList.Count(); ++i)
                    {
                        var currentAttack = attackList.ElementAt(i);
                        _outputManager.WriteLine($"{currentAttack.Name}, {currentAttack.Type}, Attack: {currentAttack.Attack}, Defense: {currentAttack.Defense}");
                    }
                    break;
                case "3":
                    var defenseList = items.OrderByDescending(d => d.Defense);
                    for (int i = 0; i < defenseList.Count(); ++i)
                    {
                        var currentDefense = defenseList.ElementAt(i);
                        _outputManager.WriteLine($"{currentDefense.Name}, {currentDefense.Type}, Attack: {currentDefense.Attack}, Defense: {currentDefense.Defense}");
                    }
                    break;
                default:
                    _outputManager.WriteLine("Invalid selection.", ConsoleColor.Red);
                    break;
            }

            if (input == "1" || input == "2" || input == "3")
            {
                break;
            }
        }
    }

    private void SetupGame()
    {
        _player = _context.Players.FirstOrDefault();
        _items = _context.Items.ToList();
        _playerItems = _player.Inventory.Items;
        _outputManager.WriteLine($"{_player.Name} has entered the game.", ConsoleColor.Green);

        // Load monsters into random rooms 
        LoadMonsters();

        // Pause before starting the game loop
        Thread.Sleep(500);
        GameLoop();
    }

    private void LoadMonsters()
    {
        _goblin = _context.Monsters.OfType<Goblin>().FirstOrDefault();
    }
}
