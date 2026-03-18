# Transaction System

Simple console-based transaction system implemented in C# (.NET).

## Features

- Create account with initial balance
- Deposit money
- Withdraw money (with validation)
- Check account balance
- Transfer money between accounts (atomic and thread-safe)

## Architecture

The application follows a simple layered design:

- **Models**  
  Domain entities (e.g. `Account`) encapsulating business rules and invariants.

- **Services**  
  Business logic and orchestration (`TransactionService`).

- **Storage**  
  In-memory repository implementation using `ConcurrentDictionary`.

- **Console UI**  
  Thin layer for user interaction (`Program.cs`).

## Key Design Decisions

- **Thread safety**
  - Per-account locking to prevent race conditions
  - Deterministic lock ordering in transfers to avoid deadlocks

- **Separation of concerns**
  - Domain logic in models
  - Business orchestration in services
  - Storage isolated in repository

- **In-memory storage**
  - Implemented via `ConcurrentDictionary`
  - Suitable for demo/testing scenarios

- **Validation**
  - Input validation at both domain and service levels
  - Consistent error handling using centralized messages

## Running the Application

```bash
dotnet run

## Running tests

```bash
dotnet test
