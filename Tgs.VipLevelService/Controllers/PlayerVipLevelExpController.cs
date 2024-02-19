using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Controllers;

[Route("player-exp")]
[ApiController]
public class PlayerVipLevelExpController : ControllerBase
{
	/// <summary>
	/// Players the vip level exp add turnover asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///			{
	///				"playerCode": "4ucmqplxb029",
	///				"gameProviderId": "6214260a-cf3c-4fb5-ba78-9214fe5608bf",
	///				"gameId": 3500042,
	///				"walletId": "36a6cdef-094f-eb11-8d88-70b5e82b6b8d",
	///				"externalWagerId": "PG-1744653599482446848",
	///				"wagerId": 1744653777370501121,
	///				"amount": 0.5
	///			}
	/// </remarks>
	[HttpPost("{playerCode}/turnover", Name = "PlayerVipLevelExp_AddTurnover")]
	public ValueTask PlayerVipLevelExpAddTurnoverAsync(
		[FromServices] ICommandService<PlayerVipLevelExpAddTurnoverCommand> service,
		string playerCode,
		PlayerVipLevelExpAddTurnoverViewModel source)
			=> service.ExecuteAsync(
				command: new PlayerVipLevelExpAddTurnoverCommand(
					PlayerCode: playerCode,
					GameProviderId: source.GameProviderId,
					GameId: source.GameId,
					WalletId: source.WalletId,
					ExternalWagerId: source.ExternalWagerId,
					WagerId: source.WagerId,
					Amount: source.Amount),
				cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the vip level exp add deposit asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="playerCode">The player code.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///			{
	///				"playerCode": "4ucmqplxb029",
	///				"formId": "a0fb3f41-8e83-45a5-ad5f-8272a72ec334",
	///				"amount": 3500.000000
	///			}
	/// </remarks>
	[HttpPost("{playerCode}/deposit", Name = "PlayerVipLevelExp_AddDeposit")]
	public ValueTask PlayerVipLevelExpAddDepositAsync(
		[FromServices] ICommandService<PlayerVipLevelExpAddDepositCommand> service,
		string playerCode,
		PlayerVipLevelExpAddDepositViewModel source)
			=> service.ExecuteAsync(
				command: new PlayerVipLevelExpAddDepositCommand(
					PlayerCode: playerCode,
					FormId: source.FormId,
					Amount: source.Amount),
				cancellationToken: HttpContext.RequestAborted);
}
