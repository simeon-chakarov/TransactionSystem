using System.Collections.Concurrent;
using TransactionSystem.Common;
using TransactionSystem.Models;

namespace TransactionSystem.Storage;

public sealed class InMemoryAccountRepository : IAccountRepository
{
    private readonly ConcurrentDictionary<string, Account> _accounts = new();

    public bool Create(Account account)
    {
        if (account is null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        return _accounts.TryAdd(account.AccountNumber, account);
    }

    public Account? GetByAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException(ErrorMessages.AccountNumberRequired, nameof(accountNumber));
        }

        _accounts.TryGetValue(accountNumber, out var account);
        return account;
    }
}