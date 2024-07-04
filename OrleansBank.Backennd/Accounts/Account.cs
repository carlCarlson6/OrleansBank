using Orleans.Runtime;
using OrleansBank.Backennd.Infrastructure.Orleans;

namespace OrleansBank.Backennd.Accounts;

[Alias(nameof(IAccount))]
public interface IAccount : IGrainWithStringKey
{
    [Alias(nameof(Open))] 
    Task Open(OpenAccountCommand command);

    [Alias(nameof(GetState))]
    Task<AccountState?> GetState();
    
    [Alias(nameof(MakeDeposit))] 
    Task<AccountState> MakeDeposit(MakeDepositCommand command);
    
    [Alias(nameof(Withdraw))] 
    Task Withdraw(int amountInCents);
    
    [Alias(nameof(ExecuteWireTransfer))] 
    Task ExecuteWireTransfer(ExecuteWireTransferCommand command);
}

[GenerateSerializer, Alias(nameof(OpenAccountCommand))]
public record OpenAccountCommand(Guid ClientId, int InitialAmountInCents);

[GenerateSerializer, Alias(nameof(MakeDepositCommand))]
public record MakeDepositCommand(int AmountInCents, string? Reason);

[GenerateSerializer, Alias(nameof(ExecuteWireTransferCommand))]
public record ExecuteWireTransferCommand;

public class Account(
    [PersistentState(
        stateName: "account-state",
        storageName: "storage"
    )] IPersistentState<AccountState> accountPersistentState,
    [PersistentState(
        stateName: "account-transactions",
        storageName: nameof(RavenDbGrainStateStorage)
    )] IPersistentState<AccountTransactions> transactionsPersistentState) : Grain, IAccount
{
    public async Task Open(OpenAccountCommand command)
    {
        if (accountPersistentState.RecordExists) 
        {
            throw new Exception("account already opened");
        }

        accountPersistentState.State = new AccountState
        {
            AccountId = this.GetPrimaryKeyString(),
            CliendId = command.ClientId,
            BalanceInCents = command.InitialAmountInCents
        };
        transactionsPersistentState.State = new AccountTransactions 
        {
            Transactions = []
        };

        await accountPersistentState.WriteStateAsync();
        await transactionsPersistentState.WriteStateAsync();
    }

    public Task<AccountState?> GetState() => Task.FromResult<AccountState?>(accountPersistentState.State);

    public async Task<AccountState> MakeDeposit(MakeDepositCommand command)
    {
        if (!accountPersistentState.RecordExists || !transactionsPersistentState.RecordExists) 
        {
            throw new Exception("account must be opened first");
        }

        transactionsPersistentState.State.Transactions.Add(new(AccountTransactionType.InitialDeposit, command.AmountInCents));
        await transactionsPersistentState.WriteStateAsync();

        accountPersistentState.State.BalanceInCents = accountPersistentState.State.BalanceInCents + command.AmountInCents;
        await accountPersistentState.WriteStateAsync();
        
        return accountPersistentState.State;
    }

    public Task Withdraw(int amountInCents)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteWireTransfer(ExecuteWireTransferCommand command)
    {
        throw new NotImplementedException();
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
    }

    public static string BuildId(Guid guid) => $"accounts/{guid}";
}

[GenerateSerializer, Alias(nameof(AccountState))]
public class AccountState
{
    [Id(0)]
    public string AccountId { get; set; }
    
    [Id(1)]
    public Guid CliendId { get; set; }
    
    [Id(2)]
    public int BalanceInCents { get; set; }
}

[GenerateSerializer, Alias(nameof(AccountTransactions))]
public class AccountTransactions 
{
    [Id(0)]
    public List<AccountTransaction> Transactions { get; set; }
}

[GenerateSerializer]
public record AccountTransaction(AccountTransactionType Type, int AmountInCents);

public enum AccountTransactionType 
{
    InitialDeposit = 0,
}

