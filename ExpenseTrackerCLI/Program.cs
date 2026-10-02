using System.Linq;
using System.IO;
using System.Text.Json;

List<Expense> expenses = LoadExpenses();
bool isRunning = true;

while (isRunning)
{
    Console.WriteLine("Expense Tracker CLI");
    Console.WriteLine("1. Add Expense");
    Console.WriteLine("2. View All Expenses");
    Console.WriteLine("3. Save Expenses");
    Console.WriteLine("4. Filter by Category");
    Console.WriteLine("5. Exit");
    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddExpense(expenses);
            break;
        case "2":
            ViewExpenses(expenses);
            break;
        case "3":
            SaveExpenses(expenses);
            break;
        case "4":
            FilterByCategory(expenses);
            break;
        case "5":
            isRunning = false;
            break;
        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}
static void AddExpense(List<Expense> list)
{
    Console.Write("Enter description: ");
    string description = Console.ReadLine() ?? "";

    Console.Write("Enter amount: ");
    decimal amount;

    while (!decimal.TryParse(Console.ReadLine(), out amount))
    {
        Console.WriteLine("Invalid amount, try again: ");
    }


    Console.WriteLine("0 = Food, 1 = Transport, 2 = Utilities, 3 = Others");
    int.TryParse(Console.ReadLine(), out int categoryIndex);


    ExpenseCategory category = (ExpenseCategory)categoryIndex;
    DateOnly date = DateOnly.FromDateTime(DateTime.Now);


    Expense newExpense = new Expense(description, amount, category, date);
    list.Add(newExpense);

    Console.WriteLine("Success: Expense added!");
}

static void ViewExpenses(List<Expense> list)
{
    if (list.Count == 0)
    {
        Console.WriteLine("No expenses yet.");
        return;
    }

    foreach (Expense item in list)
    {
        Console.WriteLine($"{item.Date} | {item.Category} | {item.Description} | {item.Amount}");

    }
    decimal total = list.Sum(e => e.Amount);
    Console.WriteLine($"Total Expenses: {total}");
}


static void SaveExpenses(List<Expense> list)
{
    string jsonString = JsonSerializer.Serialize(list);
    File.WriteAllText("expenses.json", jsonString);
    Console.WriteLine("Data saved.");
}


static List<Expense> LoadExpenses()
{
    if (!File.Exists("expenses.json"))
    {
        return new List<Expense>();
    }
    string jsonString = File.ReadAllText("expenses.json");
    return JsonSerializer.Deserialize<List<Expense>>(jsonString) ?? new List<Expense>();
}


static void FilterByCategory(List<Expense> list)
{
    Console.WriteLine("0 = Food, 1 = Transport, 2 = Utilities, 3 = Others");
    int.TryParse(Console.ReadLine(), out int categoryIndex);

    ExpenseCategory selectedCategory = (ExpenseCategory)categoryIndex;
    List<Expense> filteredList = list.Where(e => e.Category == selectedCategory).ToList();

    ViewExpenses(filteredList);
}

public record Expense(string Description, decimal Amount, ExpenseCategory Category, DateOnly Date);

public enum ExpenseCategory{
    Food,
    Transport,
    Utilities,
    Others
}