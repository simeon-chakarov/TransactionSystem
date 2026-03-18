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

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateAccount_Should_Throw_When_AccountNumber_Is_Invalid(string accountNumber)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _service.CreateAccount(accountNumber, "Ivan", 100m);
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateAccount_Should_Throw_When_HolderName_Is_Invalid(string holderName)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _service.CreateAccount("ACC1", holderName, 100m);
        });
    }

    [Fact]
    public void CreateAccount_Should_Throw_When_InitialBalance_Is_Negative()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _service.CreateAccount("ACC1", "Ivan", -1m);
        });
    }

    [Fact]
    public void CreateAccount_Should_Allow_Zero_Initial_Balance()
    {
        _service.CreateAccount("ACC1", "Ivan", 0m);

        Assert.Equal(0m, _service.GetBalance("ACC1"));
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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deposit_Should_Throw_When_Amount_Is_Not_Positive(double amount)
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        Assert.Throws<ArgumentException>(() =>
        {
            _service.Deposit("ACC1", (decimal)amount);
        });
    }

    [Fact]
    public void Deposit_Should_Throw_When_Account_Does_Not_Exist()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            _service.Deposit("MISSING", 10m);
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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Withdraw_Should_Throw_When_Amount_Is_Not_Positive(double amount)
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        Assert.Throws<ArgumentException>(() =>
        {
            _service.Withdraw("ACC1", (decimal)amount);
        });
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
    public void Withdraw_Should_Allow_Withdrawing_Exact_Balance()
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        _service.Withdraw("ACC1", 100m);

        Assert.Equal(0m, _service.GetBalance("ACC1"));
    }

    [Fact]
    public void Withdraw_Should_Throw_When_Account_Does_Not_Exist()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            _service.Withdraw("MISSING", 10m);
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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Transfer_Should_Throw_When_Amount_Is_Not_Positive(double amount)
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);
        _service.CreateAccount("ACC2", "Maria", 100m);

        Assert.Throws<ArgumentException>(() =>
        {
            _service.Transfer("ACC1", "ACC2", (decimal)amount);
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
    public void Transfer_Should_Throw_When_Source_Account_Does_Not_Exist()
    {
        _service.CreateAccount("ACC2", "Maria", 100m);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _service.Transfer("MISSING", "ACC2", 10m);
        });
    }

    [Fact]
    public void Transfer_Should_Throw_When_Destination_Account_Does_Not_Exist()
    {
        _service.CreateAccount("ACC1", "Ivan", 100m);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _service.Transfer("ACC1", "MISSING", 10m);
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

    [Fact]
    public void Withdraw_Should_Be_Thread_Safe()
    {
        _service.CreateAccount("ACC1", "Ivan", 1000m);

        Parallel.For(0, 1000, _ =>
        {
            _service.Withdraw("ACC1", 1m);
        });

        var balance = _service.GetBalance("ACC1");

        Assert.Equal(0m, balance);
    }

    [Fact]
    public void Transfer_Should_Be_Thread_Safe()
    {
        _service.CreateAccount("ACC1", "Ivan", 500m);
        _service.CreateAccount("ACC2", "Maria", 500m);

        Parallel.For(0, 1000, i =>
        {
            if (i % 2 == 0)
            {
                _service.Transfer("ACC1", "ACC2", 1m);
            }
            else
            {
                _service.Transfer("ACC2", "ACC1", 1m);
            }
        });

        var total = _service.GetBalance("ACC1") + _service.GetBalance("ACC2");

        Assert.Equal(1000m, total);
    }
}