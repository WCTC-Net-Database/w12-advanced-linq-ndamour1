using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Equipments;
using Microsoft.EntityFrameworkCore;

namespace ConsoleRpg.Helpers;

public class MenuManager
{
    private readonly GameContext _context;
    private readonly OutputManager _outputManager;

    private IPlayer _player;
    private ICollection<Item> _playerItems;

    public MenuManager(GameContext context, OutputManager outputManager)
    {
        _outputManager = outputManager;
        _context = context;
    }

    public bool ShowMainMenu()
    {
        _outputManager.WriteLine("Welcome to the RPG Game!", ConsoleColor.Yellow);
        _outputManager.WriteLine("1. Start Game", ConsoleColor.Cyan);
        _outputManager.WriteLine("2. Inventory Management", ConsoleColor.Cyan);
        _outputManager.WriteLine("3. Exit", ConsoleColor.Cyan);
        _outputManager.Display();

        return HandleMainMenuInput();
    }

    private bool HandleMainMenuInput()
    {
        while (true)
        {
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    _outputManager.WriteLine("Starting game...", ConsoleColor.Green);
                    _outputManager.Display();
                    return true;
                case "2":
                    SearchItems();
                    ShowMainMenu();
                    break;
                case "3":
                    _outputManager.WriteLine("Exiting game...", ConsoleColor.Red);
                    _outputManager.Display();
                    Environment.Exit(0);
                    return false;
                default:
                    _outputManager.WriteLine("Invalid selection. Please choose 1 or 2.", ConsoleColor.Red);
                    _outputManager.Display();
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
}
