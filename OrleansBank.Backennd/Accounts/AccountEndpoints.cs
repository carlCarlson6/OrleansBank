using Microsoft.AspNetCore.Mvc;

namespace OrleansBank.Backennd.Accounts;

public static class OpenAccountEndpoint
{
    private const string Path = "/api/accounts";
    
    private static async Task<IResult> Handler (
        [FromServices] IGrainFactory grains,
        [FromBody] OpenAccountRequest request)
    {
        var accountId = Guid.NewGuid();
        await grains
            .GetGrain<IAccount>(Account.BuildId(accountId))
            .Open(new OpenAccountCommand(request.UserId, request.InitialBalanceAmountInCents));
        return Results.Ok(new { accountId });
    }
    
    public static void MapOpenAccountEndpoint(this WebApplication app) => app.MapPost(Path, Handler);
}

public record OpenAccountRequest(Guid UserId, int InitialBalanceAmountInCents);

public static class GetAccountStatusEndpoint 
{
    private const string Path = "/api/accounts/{accountId}";

    private static async Task<IResult> Handler(
        [FromServices] IGrainFactory grains, 
        [FromRoute] Guid accountId) 
    {
        var bankAccountStatus = await grains.GetGrain<IAccount>(Account.BuildId(accountId)).GetState();
        return Results.Ok(bankAccountStatus);
    }

    public static void MapGetAccountStatusEndpoint(this WebApplication app) => app.MapGet(Path, Handler);
}

public static class MakeDepositStatusEndpoint 
{
    private const string Path = "/api/accounts/{accountId}/deposit";

        private static async Task<IResult> Handler(
        [FromServices] IGrainFactory grains, 
        [FromRoute] Guid accountId,
        [FromBody] MakeDepositRequest makeDepositRequest) 
    {
        var bankAccountStatus = await grains
            .GetGrain<IAccount>(Account.BuildId(accountId))
            .MakeDeposit(new MakeDepositCommand(makeDepositRequest.AmountInCents, string.Empty));
        return Results.Ok(bankAccountStatus);
    }

    public static void MapMakeDepositStatusEndpoint(this WebApplication app) => app.MapPut(Path, Handler);
}

public record MakeDepositRequest(int AmountInCents);