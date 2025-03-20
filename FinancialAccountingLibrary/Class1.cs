namespace FinancialAccountingLibrary;
using Newtonsoft.Json;

public static class BankAccountFactory
{
    public static BankAccount CreateBankAccount(string? name, decimal initialBalance = 0)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Имя счета не может быть пустым.");
        }

        if (initialBalance < 0)
        {
            throw new ArgumentException("Начальный баланс не может быть отрицательным.");
        }

        return new BankAccount(name, initialBalance);
    }
}

public static class CategoryFactory
{
    public static Category CreateCategory(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Имя категории не может быть пустым.");
        }

        return new Category(name);
    }
}

public static class OperationFactory
{
    public static Operation CreateOperation(decimal amount, string? type, Category category, DateTime date)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Сумма операции должна быть положительной.");
        }

        if (category == null)
        {
            throw new ArgumentException("Категория не может быть null.");
        }

        if (type != "0" && type != "1")
        {
            throw new ArgumentException("Тип операции должен быть '0' (доход) или '1' (расход).");
        }

        return new Operation(amount, type, category, date);
    }
}

public class BankAccount
{
    private Guid _id = Guid.NewGuid();

    private string _name;
    private decimal _balance;

    private readonly List<Operation> _operations = new();

    public BankAccount(string name, decimal initialBalance = 0)
    {
        _name = name;
        _balance = initialBalance;
    }

    public Guid Id => _id;
    public string Name => _name;
    public decimal Balance => _balance;
    public List<Operation> Operations => _operations;

    public Guid GetId() => _id;
    public string GetName() => _name;
    public decimal GetBalance() => _balance;
    public void SetName(string name) => _name = name;

    public void AddOperation(Operation operation)
    {
        _operations.Add(operation);
        _balance += operation.IsIncome() ? operation.GetAmount() : -operation.GetAmount();
    }

    public void RemoveOperation(Guid operationId)
    {
        var operation = _operations.FirstOrDefault(o => o.GetId() == operationId);
        if (operation != null)
        {
            _operations.Remove(operation);
            _balance -= operation.IsIncome() ? operation.GetAmount() : -operation.GetAmount();
            Console.WriteLine("Операция успешно удалена.");
        }
        else
        {
            Console.WriteLine("Операция с таким ID не найдена.");
        }
    }
    public void EditOperation(Guid operationId, decimal newAmount, string newType, Category newCategory)
    {
        var operation = _operations.FirstOrDefault(o => o.GetId() == operationId);
        if (operation != null)
        {
            _balance -= operation.IsIncome() ? operation.GetAmount() : -operation.GetAmount();
            operation.SetAmount(newAmount);
            operation.SetType(newType);
            operation.SetCategory(newCategory);
            _balance += operation.IsIncome() ? operation.GetAmount() : -operation.GetAmount();
        }
    }
    public List<Operation> GetOperations()
    {
        return _operations;
    }

    public decimal GetIncomeExpenseDifference(DateTime startDate, DateTime endDate)
    {
        decimal income = _operations
            .Where(o => o.GetDate() >= startDate && o.GetDate() <= endDate && o.IsIncome())
            .Sum(o => o.GetAmount());

        decimal expense = _operations
            .Where(o => o.GetDate() >= startDate && o.GetDate() <= endDate && !o.IsIncome())
            .Sum(o => o.GetAmount());

        return income - expense;
    }

    public void ShowGroupedTransactionsByCategory(DateTime startDate, DateTime endDate)
    {
        var grouped = _operations
            .Where(o => o.GetDate() >= startDate && o.GetDate() <= endDate)
            .GroupBy(o => o.GetCategory().GetName());

        if (!grouped.Any())
        {
            Console.WriteLine("Нет операций за выбранный период.");
            return;
        }

        Console.WriteLine("Группировка доходов и расходов по категориям:");
        foreach (var group in grouped)
        {
            decimal income = group.Where(o => o.IsIncome()).Sum(o => o.GetAmount());
            decimal expense = group.Where(o => !o.IsIncome()).Sum(o => o.GetAmount());

            Console.WriteLine($"Категория: {group.Key} | Доход: {income} | Расход: {expense}");
        }
    }

}

public class Category
{
    private Guid _id = Guid.NewGuid();
    private string _name;

    public Category(string name) => _name = name;

    public Guid GetId() => _id;
    public string GetName() => _name;
    public void SetName(string name) => _name = name;

    public Guid Id => _id;
    public string Name => _name;
}
public enum OperationType { Income, Expense }

public class Operation
{
    private Guid _id = Guid.NewGuid();

    private decimal _amount;

    private string _type;

    private DateTime _date;
    private Category _category;

    public Operation(decimal amount, string type, Category category, DateTime date)
    {
        _amount = amount;
        _type = type == "0" ? "income" : "outcome";
        _category = category;
        _date = date;
    }

    public Guid GetId() => _id;
    public decimal GetAmount() => _amount;
    public string GetType() => _type;
    public Category GetCategory() => _category;
    public DateTime GetDate() => _date;

    public Guid Id => _id;
    public decimal Amount => _amount;
    public string Type => _type;
    public Category Category => _category;
    public DateTime Date => _date;

    public void SetAmount(decimal amount) => _amount = amount;
    public void SetType(string type) => _type = type == "0" ? "income" : "outcome";
    public void SetCategory(Category category) => _category = category;
    public bool IsIncome() => _type == "income";
}