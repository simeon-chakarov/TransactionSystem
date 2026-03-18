using TransactionSystem.Common;

namespace TransactionSystem.Models;

public sealed class Account
{
    private readonly object _lock = new();

    public string AccountNumber { get; }
    public string HolderName { get; }
    public decimal Balance { get; private set; }

    public Account(string accountNumber, string holderName, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException(ErrorMessages.AccountNumberRequired, nameof(accountNumber));
        }

        if (string.IsNullOrWhiteSpace(holderName))
        {
            throw new ArgumentException(ErrorMessages.AccountHolderNameRequired, nameof(holderName));
        }

        if (initialBalance < 0)
        {
            throw new ArgumentException(ErrorMessages.InitialBalanceCannotBeNegative, nameof(initialBalance));
        }

        AccountNumber = accountNumber;
        HolderName = holderName;
        Balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException(ErrorMessages.DepositAmountMustBeGreaterThanZero, nameof(amount));
        }

        lock (_lock)
        {
            Balance += amount;
        }
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException(ErrorMessages.WithdrawalAmountMustBeGreaterThanZero, nameof(amount));
        }

        lock (_lock)
        {
            if (Balance < amount)
            {
                throw new InvalidOperationException(ErrorMessages.InsufficientFunds);
            }

            Balance -= amount;
        }
    }

    public decimal GetBalance()
    {
        lock (_lock)
        {
            return Balance;
        }
    }

    public void TransferTo(Account target, decimal amount)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (ReferenceEquals(this, target))
        {
            throw new InvalidOperationException(ErrorMessages.CannotTransferToSameAccount);
        }

        if (amount <= 0)
        {
            throw new ArgumentException(ErrorMessages.TransferAmountMustBeGreaterThanZero, nameof(amount));
        }

        // Lock in deterministic order to prevent deadlocks.
        var firstAccountToLock = string.Compare(AccountNumber, target.AccountNumber, StringComparison.Ordinal) < 0
            ? this
            : target;

        var secondAccountToLock = ReferenceEquals(firstAccountToLock, this)
            ? target
            : this;

        lock (firstAccountToLock._lock)
        {
            lock (secondAccountToLock._lock)
            {
                if (Balance < amount)
                {
                    throw new InvalidOperationException(ErrorMessages.InsufficientFunds);
                }

                Balance -= amount;
                target.Balance += amount;
            }
        }
    }
}