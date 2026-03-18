using TransactionSystem.Common;

namespace TransactionSystem.Models;

public sealed class Account
{
    private readonly object _lock = new();
    private decimal _balance;

    public string AccountNumber { get; }
    public string HolderName { get; }

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
        _balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException(ErrorMessages.DepositAmountMustBeGreaterThanZero, nameof(amount));
        }

        lock (_lock)
        {
            _balance += amount;
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
            if (_balance < amount)
            {
                throw new InvalidOperationException(ErrorMessages.InsufficientFunds);
            }

            _balance -= amount;
        }
    }

    public decimal GetBalance()
    {
        lock (_lock)
        {
            return _balance;
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
                if (_balance < amount)
                {
                    throw new InvalidOperationException(ErrorMessages.InsufficientFunds);
                }

                _balance -= amount;
                target._balance += amount;
            }
        }
    }
}