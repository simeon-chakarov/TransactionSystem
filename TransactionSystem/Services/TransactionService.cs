using TransactionSystem.Common;
using TransactionSystem.Models;
using TransactionSystem.Storage;

namespace TransactionSystem.Services;

public sealed class TransactionService(IAccountRepository accountRepository) : ITransactionService
{
    private readonly IAccountRepository _accountRepository = 
        accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));

    public void CreateAccount(string accountNumber, string holderName, decimal initialBalance)
    {
        var account = new Account(accountNumber, holderName, initialBalance);
        var isCreated = _accountRepository.Create(account);

        if (!isCreated)
        {
            throw new InvalidOperationException(ErrorMessages.DuplicateAccount);
        }
    }

    public void Deposit(string accountNumber, decimal amount)
    {
        ValidateAccountNumber(accountNumber, nameof(accountNumber));

        var account = GetRequiredAccount(accountNumber);
        account.Deposit(amount);
    }

    public void Withdraw(string accountNumber, decimal amount)
    {
        ValidateAccountNumber(accountNumber, nameof(accountNumber));

        var account = GetRequiredAccount(accountNumber);
        account.Withdraw(amount);
    }

    public decimal GetBalance(string accountNumber)
    {
        ValidateAccountNumber(accountNumber, nameof(accountNumber));

        var account = GetRequiredAccount(accountNumber);
        return account.GetBalance();
    }

    public void Transfer(string fromAccountNumber, string toAccountNumber, decimal amount)
    {
        ValidateAccountNumber(fromAccountNumber, nameof(fromAccountNumber));
        ValidateAccountNumber(toAccountNumber, nameof(toAccountNumber));

        if (string.Equals(fromAccountNumber, toAccountNumber, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(ErrorMessages.CannotTransferToSameAccount);
        }

        var fromAccount = GetRequiredAccount(fromAccountNumber);
        var toAccount = GetRequiredAccount(toAccountNumber);

        fromAccount.TransferTo(toAccount, amount);
    }

    private Account GetRequiredAccount(string accountNumber)
    {
        var account = _accountRepository.GetByAccountNumber(accountNumber);

        if (account is null)
        {
            throw new InvalidOperationException(ErrorMessages.AccountNotFound);
        }

        return account;
    }

    private static void ValidateAccountNumber(string accountNumber, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException(ErrorMessages.AccountNumberRequired, parameterName);
        }
    }
}