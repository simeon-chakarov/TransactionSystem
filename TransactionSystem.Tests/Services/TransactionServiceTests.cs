using TransactionSystem.Services;
using TransactionSystem.Storage;

namespace TransactionSystem.Tests.Services;

public class TransactionServiceTests
{
    private readonly ITransactionService _service;

    public TransactionServiceTests()
    {
        var repository = new InMemoryAccountRepository();
        _service = new TransactionService(repository);
    }

    [Fact]
    public void CreateAccount_Should_Set_InitialBalance()
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        var balance = _service.GetBalance("ACC1");

        Assert.Equal(100m, balance);
    }

    [Fact]
    public void CreateAccount_Should_Throw_When_AccountNumber_Already_Exists()
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _service.CreateAccount("ACC1", "Maria", 50m);
        });
    }

    [Fact]
    public void Deposit_Should_Increase_Balance()
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        _service.Deposit("ACC1", 50m);

        var balance = _service.GetBalance("ACC1");

        Assert.Equal(150m, balance);
    }

    [Fact]
    public void Deposit_Should_Throw_When_Amount_Is_Not_Positive()
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        Assert.Throws<ArgumentException>(() =>
        {
            _service.Deposit("ACC1", 0m);
        });
    }

    [Fact]
    public void Withdraw_Should_Decrease_Balance()
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        _service.Withdraw("ACC1", 40m);

        var balance = _service.GetBalance("ACC1");

        Assert.Equal(60m, balance);
    }

    [Fact]
    public void Withdraw_Should_Throw_When_InsufficientFunds()
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _service.Withdraw("ACC1", 200m);
        });
    }

    [Fact]
    public void GetBalance_Should_Throw_When_Account_Does_Not_Exist()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            _service.GetBalance("MISSING");
        });
    }

    [Fact]
    public void Transfer_Should_Move_Money_Between_Accounts()
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);
        _service.CreateAccount("ACC2", "Maria", 50m);

        _service.Transfer("ACC1", "ACC2", 30m);

        var sourceBalance = _service.GetBalance("ACC1");
        var destinationBalance = _service.GetBalance("ACC2");

        Assert.Equal(70m, sourceBalance);
        Assert.Equal(80m, destinationBalance);
    }

    [Fact]
    public void Transfer_Should_Throw_When_InsufficientFunds()
    {
        _service.CreateAccount("ACC1", "Ivan", 50m);
        _service.CreateAccount("ACC2", "Maria", 50m);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _service.Transfer("ACC1", "ACC2", 100m);
        });
    }

    [Fact]
    public void Transfer_Should_Throw_When_Source_And_Destination_Are_The_Same()
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _service.Transfer("ACC1", "ACC1", 10m);
        });
    }

    [Fact]
    public void Deposit_Should_Be_Thread_Safe()
    {
        _service.CreateAccount("ACC1", "Ivan", 0m);

        Parallel.For(0, 1000, _ =>
        {
            _service.Deposit("ACC1", 1m);
        });

        var balance = _service.GetBalance("ACC1");

        Assert.Equal(1000m, balance);
    }
}