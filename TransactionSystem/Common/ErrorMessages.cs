namespace TransactionSystem.Common;

public static class ErrorMessages
{
    public const string AccountNumberRequired = "Account number is required.";
    public const string AccountHolderNameRequired = "Account holder name is required.";
    public const string InitialBalanceCannotBeNegative = "Initial balance cannot be negative.";
    public const string DepositAmountMustBeGreaterThanZero = "Deposit amount must be greater than zero.";
    public const string WithdrawalAmountMustBeGreaterThanZero = "Withdrawal amount must be greater than zero.";
    public const string TransferAmountMustBeGreaterThanZero = "Transfer amount must be greater than zero.";
    public const string InsufficientFunds = "Insufficient funds.";
    public const string AccountNotFound = "Account not found.";
    public const string DuplicateAccount = "An account with this number already exists.";
    public const string CannotTransferToSameAccount = "Cannot transfer to the same account.";
}