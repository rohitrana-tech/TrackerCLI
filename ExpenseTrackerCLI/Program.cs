using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
const string filePath = "expenses.json";
Console.OutputEncoding = System.Text.Encoding.UTF8;


List<Expense> expenses = LoadExpenses();

bool isRunning = true;

while (isRunning)
{
    Console.WriteLine("\n================================");

    Console.WriteLine("      Expense Tracker CLI");

    Console.WriteLine("================================");

    Console.WriteLine("1. Add Expense");
    Console.WriteLine("2. View All Expenses");
    Console.WriteLine("3. Filter by Category");
    Console.WriteLine("4. Exit");
    Console.Write("Select an option (1-4): ");

    string choice = Console.ReadLine() ?? "";

    switch (choice)
    {
        case "1":
            AddExpense(expenses); 
            break;
        case "2":
            ViewExpenses(expenses);
            break;
        case "3":
            FilterByCategory(expenses);
            break;
        case "4":
            SaveExpenses(expenses);
            isRunning = false;
            Console.WriteLine("\nExiting application.Goodbye!");
            break;
        default:
            Console.WriteLine("\nInvalid selection! Please enter 1, 2, 3 or 4.");
            break;
    }
}

void AddExpense(List<Expense> list)
{
    Console.WriteLine("\n--- Add New Expense ---");

    
    string description = "";
    while (string.IsNullOrWhiteSpace(description))
    {
        Console.Write("Enter description (e.g., Lunch): ");
        description = Console.ReadLine() ?? "";
    }


    Console.Write("Enter amount: ");
    decimal amount;
    while (!decimal.TryParse(Console.ReadLine(), out amount))
    {
        Console.WriteLine("Invalid amount. Try again: ");
    }


    Console.WriteLine("Categories: Food = 0, Transport = 1, Utilities = 2, Other = 3");
    Console.Write("Select category number (0-3): ");
    int.TryParse(Console.ReadLine(), out int categoryIndex);

    ExpenseCategory category = Enum.IsDefined(typeof(ExpenseCategory), categoryIndex)
        ? (ExpenseCategory)categoryIndex
        : ExpenseCategory.Other;

    DateOnly date = DateOnly.FromDateTime(DateTime.Now);

    Expense newExpense = new Expense(description, amount, category, date);
    list.Add(newExpense);

    Console.WriteLine($"\n ✅ Success: Added '{description}' for {amount:C} under {category}.");
}


void ViewExpenses(List<Expense> list)
{
    if(list.Count == 0)
    {
        Console.WriteLine("No expenses recorded yet");
        return;
    }

    Console.WriteLine("--- Expense Report ---");
    foreach(Expense item in list)
    {
        Console.WriteLine($"{item.Date} | {item.Description}...");
    }

    decimal total = list.Sum(e => e.Amount);
    Console.WriteLine($"Total Spent: {total:C}");

    Console.WriteLine("\nPress any key to return to the main menu...");
    Console.ReadKey();
}


void SaveExpenses(List<Expense> list)
{
    string json = JsonSerializer.Serialize(list);
    File.WriteAllText(filePath, json);
    Console.WriteLine("Data saved successfully!");
}

List<Expense> LoadExpenses()
{
    if (!File.Exists(filePath))
    {
        return new List<Expense>();
    }

    string json = File.ReadAllText(filePath);
    return JsonSerializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
}


void FilterByCategory(List<Expense> list)
{
    Console.WriteLine("\n--- Filter by Category ---");
    Console.WriteLine("Categories: Food = 0, Transport = 1, Utilities = 2, Other = 3");
    Console.Write("Select category number (0-3): ");

    // 1. Capture and parse the input
    int.TryParse(Console.ReadLine(), out int categoryIndex);

    // 2. Define selectedCategory securely using your enum
    ExpenseCategory selectedCategory = Enum.IsDefined(typeof(ExpenseCategory), categoryIndex)
        ? (ExpenseCategory)categoryIndex
        : ExpenseCategory.Other;

    // 3. Execute the LINQ query and pass the result to your display method
    List<Expense> filteredList = list.Where(e => e.Category == selectedCategory).ToList();
    ViewExpenses(filteredList);
}


enum ExpenseCategory
        {
        Food,
            Transport,
            Utilities,
            Other
    }

record Expense(string Description, decimal Amount, ExpenseCategory Category, DateOnly Date);