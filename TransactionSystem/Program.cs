using TransactionSystem.Services;
using TransactionSystem.Storage;

var accountRepository = new InMemoryAccountRepository();
var transactionService = new TransactionService(accountRepository);

while (true)
{
    PrintMenu();

    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                CreateAccount(transactionService);
                break;

            case "2":
                Deposit(transactionService);
                break;

            case "3":
                Withdraw(transactionService);
                break;

            case "4":
                CheckBalance(transactionService);
                break;

            case "5":
                TransferMoney(transactionService);
                break;

            case "0":
                Console.WriteLine("Exiting application...");
                return;

            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    Pause();
}

static void PrintMenu()
{
    Console.WriteLine();
    Console.WriteLine("=== Transaction System ===");
    Console.WriteLine("1. Create Account");
    Console.WriteLine("2. Deposit Money");
    Console.WriteLine("3. Withdraw Money");
    Console.WriteLine("4. Check Balance");
    Console.WriteLine("5. Transfer Money");
    Console.WriteLine("0. Exit");
    Console.Write("Choose an option: ");
}

static void CreateAccount(ITransactionService transactionService)
{
    var accountNumber = ReadInput("Enter account number: ");
    var holderName = ReadInput("Enter account holder name: ");
    var initialBalance = ReadAmount("Enter initial balance: ");

    if (initialBalance is null)
    {
        Console.WriteLine("Invalid amount.");
        return;
    }

    transactionService.CreateAccount(accountNumber, holderName, initialBalance.Value);
    Console.WriteLine("Account created successfully.");
}

static void Deposit(ITransactionService transactionService)
{
    var accountNumber = ReadInput("Enter account number: ");
    var amount = ReadAmount("Enter deposit amount: ");

    if (amount is null)
    {
        Console.WriteLine("Invalid amount.");
        return;
    }

    transactionService.Deposit(accountNumber, amount.Value);
    Console.WriteLine("Deposit completed successfully.");
}

static void Withdraw(ITransactionService transactionService)
{
    var accountNumber = ReadInput("Enter account number: ");
    var amount = ReadAmount("Enter withdrawal amount: ");

    if (amount is null)
    {
        Console.WriteLine("Invalid amount.");
        return;
    }

    transactionService.Withdraw(accountNumber, amount.Value);
    Console.WriteLine("Withdrawal completed successfully.");
}

static void CheckBalance(ITransactionService transactionService)
{
    var accountNumber = ReadInput("Enter account number: ");
    var balance = transactionService.GetBalance(accountNumber);

    Console.WriteLine($"Current balance: {balance:F2}");
}

static void TransferMoney(ITransactionService transactionService)
{
    var fromAccountNumber = ReadInput("Enter source account number: ");
    var toAccountNumber = ReadInput("Enter destination account number: ");
    var amount = ReadAmount("Enter transfer amount: ");

    if (amount is null)
    {
        Console.WriteLine("Invalid amount.");
        return;
    }

    transactionService.Transfer(fromAccountNumber, toAccountNumber, amount.Value);
    Console.WriteLine("Transfer completed successfully.");
}

static string ReadInput(string prompt)
{
    Console.Write(prompt);
    return (Console.ReadLine() ?? string.Empty).Trim();
}

static decimal? ReadAmount(string prompt)
{
    Console.Write(prompt);
    var input = Console.ReadLine();

    if (!decimal.TryParse(input, out var amount))
    {
        return null;
    }

    return amount;
}

static void Pause()
{
    Console.WriteLine();
    Console.Write("Press any key to continue...");
    Console.ReadKey();
    Console.Clear();
}