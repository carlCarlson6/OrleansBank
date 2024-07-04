using Microsoft.AspNetCore.Mvc;

namespace OrleansBank.Backennd.Accounts;

public static class AccountEndpoints 
{
    public static void MapOpenAccountEndpoint(this WebApplication app) => app.MapPost(
        "/api/accounts", async (
        [FromServices] IGrainFactory grains,
        [FromBody] OpenAccountRequest request
    ) => {
            var accountId = Guid.NewGuid();
            await grains
                .GetGrain<IAccount>(Account.BuildId(accountId))
                .Open(new OpenAccountCommand(request.UserId, request.InitialBalanceAmountInCents));
        return Results.Ok(new { accountId });
    });

    public static void MapGetAccountStatusEndpoint(this WebApplication app) => app.MapGet(
        "/api/accounts/{accountId}", async (
        [FromServices] IGrainFactory grains, 
        [FromRoute] Guid accountId
    ) => {
        var bankAccountStatus = await grains
                .GetGrain<IAccount>(Account.BuildId(accountId))
                .GetState();
        return Results.Ok(bankAccountStatus);
    });

    public static void MapMakeDepositStatusEndpoint(this WebApplication app) => app.MapPut(
        "/api/accounts/{accountId}/deposit", async (
        [FromServices] IGrainFactory grains, 
        [FromRoute] Guid accountId,
        [FromBody] MakeDepositRequest makeDepositRequest
    ) => {
        var bankAccountStatus = await grains
                .GetGrain<IAccount>(Account.BuildId(accountId))
                .MakeDeposit(new MakeDepositCommand(makeDepositRequest.AmountInCents, string.Empty));
        return Results.Ok(bankAccountStatus);
    });
}

public record OpenAccountRequest(Guid UserId, int InitialBalanceAmountInCents);

public record MakeDepositRequest(int AmountInCents);