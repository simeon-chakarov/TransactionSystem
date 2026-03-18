namespace TransactionSystem.Services;

public interface ITransactionService
{
    void CreateAccount(string accountNumber, string holderName, decimal initialBalance);

    void Deposit(string accountNumber, decimal amount);

    void Withdraw(string accountNumber, decimal amount);

    decimal GetBalance(string accountNumber);

    void Transfer(string fromAccountNumber, string toAccountNumber, decimal amount);
}