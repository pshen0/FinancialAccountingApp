using FinancialAccountingLibrary;
using Newtonsoft.Json;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


class Program
{
    static List<BankAccount> accounts = new();
    static List<Category> categories = new();

    static void Main()
    {
        static void ExportAccountsToJson()
        {
            var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            string filePath = "accounts.json";
            File.WriteAllText(filePath, json);

            Console.WriteLine("Данные успешно экспортированы в файл accounts.json.");
        }

        static void ExportAccountsToYaml()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string yaml = serializer.Serialize(accounts);
            File.WriteAllText("accounts.yaml", yaml);
            Console.WriteLine("Данные успешно экспортированы в файл accounts.yaml.");
        }

        static void ExportAccountsToCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id,Name,Balance");

            foreach (var account in accounts)
            {
                sb.AppendLine($"{account.GetId()},{account.GetName()},{account.GetBalance()}");
            }

            File.WriteAllText("accounts.csv", sb.ToString());
            Console.WriteLine("Данные успешно экспортированы в файл accounts.csv.");
        }



        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Создать счет");
            Console.WriteLine("2. Редактировать счет");
            Console.WriteLine("3. Удалить счет");
            Console.WriteLine("4. Добавить категорию");
            Console.WriteLine("5. Редактировать категорию");
            Console.WriteLine("6. Удалить категорию");
            Console.WriteLine("7. Добавить операцию");
            Console.WriteLine("8. Редактировать операцию");
            Console.WriteLine("9. Удалить операцию");
            Console.WriteLine("10. Показать счета");
            Console.WriteLine("11. Показать категории");
            Console.WriteLine("12. Показать операции для определенного счета");
            Console.WriteLine("13. Вывести все данные о счетах в JSON");
            Console.WriteLine("14. Вывести все данные о счетах в YAML");
            Console.WriteLine("15. Вывести все данные о счетах в CSV");
            Console.WriteLine("16. Подсчитать разницу доходов и расходов за период");
            Console.WriteLine("17. Группировать доходы и расходы по категориям за период");
            Console.WriteLine("0. Выход");

            if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

            if (choice == 0) break;

            switch (choice)
            {
                case 1:
                    Console.Write("Введите имя счета: ");
                    string? accountName = Console.ReadLine();
                    try
                    {
                        var account = BankAccountFactory.CreateBankAccount(accountName);
                        accounts.Add(account);
                        Console.WriteLine("Счет успешно создан.");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case 2:
                    Console.Write("Введите номер счета для редактирования: ");
                    if (int.TryParse(Console.ReadLine(), out int editIndex) && editIndex >= 0 && editIndex < accounts.Count)
                    {
                        Console.Write("Введите новое имя счета: ");
                        string? newAccountName = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newAccountName))
                        {
                            accounts[editIndex].SetName(newAccountName);
                            Console.WriteLine("Имя счета успешно обновлено.");
                        }
                        else
                        {
                            Console.WriteLine("Имя счета не может быть пустым.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный номер счета.");
                    }
                    break;

                case 3:
                    Console.Write("Введите номер счета для удаления: ");
                    if (int.TryParse(Console.ReadLine(), out int delIndex) && delIndex >= 0 && delIndex < accounts.Count)
                    {
                        accounts.RemoveAt(delIndex);
                        Console.WriteLine("Счет успешно удален.");
                    }
                    else
                    {
                        Console.WriteLine("Неверный номер счета.");
                    }
                    break;

                case 4:
                    Console.Write("Введите имя категории: ");
                    string? categoryName = Console.ReadLine();
                    try
                    {
                        var category = CategoryFactory.CreateCategory(categoryName);
                        categories.Add(category);
                        Console.WriteLine("Категория успешно добавлена.");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case 5:
                    Console.Write("Введите номер категории для редактирования: ");
                    if (int.TryParse(Console.ReadLine(), out int catEditIndex) && catEditIndex >= 0 && catEditIndex < categories.Count)
                    {
                        Console.Write("Введите новое имя категории: ");
                        string? newCategoryName = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newCategoryName))
                        {
                            categories[catEditIndex].SetName(newCategoryName);
                            Console.WriteLine("Имя категории успешно обновлено.");
                        }
                        else
                        {
                            Console.WriteLine("Имя категории не может быть пустым.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный номер категории.");
                    }
                    break;

                case 6:
                    Console.Write("Введите номер категории для удаления: ");
                    if (int.TryParse(Console.ReadLine(), out int catIndex) && catIndex >= 0 && catIndex < categories.Count)
                    {
                        categories.RemoveAt(catIndex);
                        Console.WriteLine("Категория успешно удалена.");
                    }
                    else
                    {
                        Console.WriteLine("Неверный номер категории.");
                    }
                    break;

                case 7:
                    Console.Write("Введите сумму: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal amount))
                    {
                        Console.Write("Тип операции (0 - доход, 1 - расход): ");
                        string? typeInput = Console.ReadLine();
                        Console.Write("Введите номер категории: ");
                        if (int.TryParse(Console.ReadLine(), out int categoryIndex) && categoryIndex >= 0 && categoryIndex < categories.Count)
                        {
                            Console.Write("Введите номер счета: ");
                            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < accounts.Count)
                            {
                                try
                                {
                                    var operation = OperationFactory.CreateOperation(amount, typeInput, categories[categoryIndex], DateTime.Now);
                                    accounts[index].AddOperation(operation);
                                    Console.WriteLine("Операция успешно добавлена.");
                                }
                                catch (ArgumentException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Неверный номер счета.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверный номер категории.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверная сумма.");
                    }
                    break;
                case 8:
                    Console.Write("Введите номер операции для редактирования: ");
                    if (int.TryParse(Console.ReadLine(), out int operationIndex) && operationIndex >= 0 && operationIndex < accounts.Count)
                    {
                        Console.Write("Введите новую сумму: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal newAmount) && newAmount > 0)
                        {
                            Console.Write("Введите новый тип операции (0 - доход, 1 - расход): ");
                            string? newTypeInput = Console.ReadLine();
                            if ((newTypeInput == "0" || newTypeInput == "1"))
                            {
                                Console.Write("Введите новый номер категории: ");
                                if (int.TryParse(Console.ReadLine(), out int newCategoryIndex) && newCategoryIndex >= 0 && newCategoryIndex < categories.Count)
                                {
                                    accounts[operationIndex].EditOperation(
                                        accounts[operationIndex].GetOperations()[operationIndex].GetId(),
                                        newAmount, newTypeInput, categories[newCategoryIndex]);
                                    Console.WriteLine("Операция успешно отредактирована.");
                                }
                                else
                                {
                                    Console.WriteLine("Неверный номер категории.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Неверный тип операции.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверная сумма.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный номер операции.");
                    }
                    break;

                case 9:
                    Console.Write("Введите номер счета: ");
                    if (int.TryParse(Console.ReadLine(), out int accountIndex) && accountIndex >= 0 && accountIndex < accounts.Count)
                    {
                        var account = accounts[accountIndex];
                        Console.Write("Введите номер операции для удаления: ");
                        if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < account.GetOperations().Count)
                        {
                            var operation = account.GetOperations()[index];
                            var operationId = operation.GetId();
                            account.RemoveOperation(operationId);
                        }
                        else
                        {
                            Console.WriteLine("Неверный номер операции.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный номер счета.");
                    }
                    break;
                case 10:
                    if (accounts.Any())
                    {
                        Console.WriteLine("Счета:");
                        for (int i = 0; i < accounts.Count; i++)
                        {
                            Console.WriteLine($"{i}. {accounts[i].GetName()} - Баланс: {accounts[i].GetBalance()}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Нет созданных счетов.");
                    }
                    break;
                case 11:
                    if (categories.Any())
                    {
                        Console.WriteLine("Категории:");
                        for (int i = 0; i < categories.Count; i++)
                        {
                            Console.WriteLine($"{i}. {categories[i].GetName()}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Нет добавленных категорий.");
                    }
                    break;
                case 12:
                    Console.Write("Введите номер счета для отображения операций: ");
                    if (int.TryParse(Console.ReadLine(), out int accountIndexForOps) && accountIndexForOps >= 0 && accountIndexForOps < accounts.Count)
                    {
                        Console.WriteLine($"Операции для счета {accounts[accountIndexForOps].GetName()}:");
                        foreach (var operation in accounts[accountIndexForOps].GetOperations())
                        {
                            Console.WriteLine($"ID: {operation.GetId()}, Сумма: {operation.GetAmount()}, Тип: {operation.GetType()}, Дата: {operation.GetDate()}, Категория: {operation.GetCategory().GetName()}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный номер счета.");
                    }
                    break;
                case 13:
                    ExportAccountsToJson();
                    break;
                case 14:
                    ExportAccountsToYaml();
                    break;
                case 15:
                    ExportAccountsToCsv();
                    break;
                case 16:
                    Console.Write("Введите номер счета: ");
                    if (int.TryParse(Console.ReadLine(), out int accIdx) && accIdx >= 0 && accIdx < accounts.Count)
                    {
                        Console.Write("Введите начальную дату (ГГГГ-ММ-ДД): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                        {
                            Console.Write("Введите конечную дату (ГГГГ-ММ-ДД): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                            {
                                decimal difference = accounts[accIdx].GetIncomeExpenseDifference(startDate, endDate);
                                Console.WriteLine($"Разница доходов и расходов за период: {difference}");
                            }
                            else Console.WriteLine("Некорректная конечная дата.");
                        }
                        else Console.WriteLine("Некорректная начальная дата.");
                    }
                    else Console.WriteLine("Неверный номер счета.");
                    break;

                case 17:
                    Console.Write("Введите номер счета: ");
                    if (int.TryParse(Console.ReadLine(), out int accIdxForGrouping) && accIdxForGrouping >= 0 && accIdxForGrouping < accounts.Count)
                    {
                        Console.Write("Введите начальную дату (ГГГГ-ММ-ДД): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                        {
                            Console.Write("Введите конечную дату (ГГГГ-ММ-ДД): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                            {
                                accounts[accIdxForGrouping].ShowGroupedTransactionsByCategory(startDate, endDate);
                            }
                            else Console.WriteLine("Некорректная конечная дата.");
                        }
                        else Console.WriteLine("Некорректная начальная дата.");
                    }
                    else Console.WriteLine("Неверный номер счета.");
                    break;
            }

        }
    }
}
