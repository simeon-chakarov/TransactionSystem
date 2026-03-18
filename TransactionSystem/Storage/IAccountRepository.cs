using TransactionSystem.Models;

namespace TransactionSystem.Storage;

public interface IAccountRepository
{
    bool Create(Account account);

    Account? GetByAccountNumber(string accountNumber);
}