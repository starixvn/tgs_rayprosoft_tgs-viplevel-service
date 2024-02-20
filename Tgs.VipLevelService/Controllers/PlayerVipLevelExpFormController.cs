using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Controllers;

[Route("exp-form")]
[ApiController]
public class PlayerVipLevelExpFormController : ControllerBase
{
	[HttpPost("deposit/credit", Name = "PlayerVipLevelExp_Form_Create_Deposit_Credit")]
	public async ValueTask<Guid> CreateDepositCreditAsync(
		[FromServices] IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult> playerService,
		[FromServices] IGetService<PlayerVipLevelExpFormCreateDepositCommand, Guid> formService,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		[FromBody] PlayerVipLevelExpFormCreateViewModel source)
	{
		var playerVipLevel = await playerService.GetAsync(
			query: new PlayerVipLevelSingleQuery(source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return await formService.GetAsync(
			query: new PlayerVipLevelExpFormCreateDepositCommand(
				BrandId: playerVipLevel.BrandId,
				PlayerCode: playerVipLevel.PlayerCode,
				VipLevel: playerVipLevel.VipLevel,
				Sign: 1,
				Amount: source.Amount,
				Remark: source.Remark,
				OperatorUserId: operatorUserId,
				OperatorUsername: operatorUsername,
				PlayerVipLevel: playerVipLevel),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	[HttpPost("deposit/debit", Name = "PlayerVipLevelExp_Form_Create_Deposit_Debit")]
	public async ValueTask<Guid> CreateDepositDebitAsync(
		[FromServices] IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult> playerService,
		[FromServices] IGetService<PlayerVipLevelExpFormCreateDepositCommand, Guid> formService,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		[FromBody] PlayerVipLevelExpFormCreateViewModel source)
	{
		var playerVipLevel = await playerService.GetAsync(
			query: new PlayerVipLevelSingleQuery(source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return await formService.GetAsync(
			query: new PlayerVipLevelExpFormCreateDepositCommand(
				BrandId: playerVipLevel.BrandId,
				PlayerCode: playerVipLevel.PlayerCode,
				VipLevel: playerVipLevel.VipLevel,
				Sign: -1,
				Amount: source.Amount,
				Remark: source.Remark,
				OperatorUserId: operatorUserId,
				OperatorUsername: operatorUsername,
				PlayerVipLevel: playerVipLevel),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	[HttpPost("turnover/credit", Name = "PlayerVipLevelExp_Form_Create_Turnover_Credit")]
	public async ValueTask<Guid> CreateDepositCreditAsync(
		[FromServices] IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult> playerService,
		[FromServices] IGetService<PlayerVipLevelExpFormCreateTurnoverCommand, Guid> formService,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		[FromBody] PlayerVipLevelExpFormCreateViewModel source)
	{
		var playerVipLevel = await playerService.GetAsync(
			query: new PlayerVipLevelSingleQuery(source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return await formService.GetAsync(
			query: new PlayerVipLevelExpFormCreateTurnoverCommand(
				BrandId: playerVipLevel.BrandId,
				PlayerCode: playerVipLevel.PlayerCode,
				VipLevel: playerVipLevel.VipLevel,
				Sign: 1,
				Amount: source.Amount,
				Remark: source.Remark,
				OperatorUserId: operatorUserId,
				OperatorUsername: operatorUsername,
				PlayerVipLevel: playerVipLevel),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	[HttpPost("turnover/debit", Name = "PlayerVipLevelExp_Form_Create_Turnover_Debit")]
	public async ValueTask<Guid> CreateDepositDebitAsync(
		[FromServices] IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult> playerService,
		[FromServices] IGetService<PlayerVipLevelExpFormCreateTurnoverCommand, Guid> formService,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		[FromBody] PlayerVipLevelExpFormCreateViewModel source)
	{
		var playerVipLevel = await playerService.GetAsync(
			query: new PlayerVipLevelSingleQuery(source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return await formService.GetAsync(
			query: new PlayerVipLevelExpFormCreateTurnoverCommand(
				BrandId: playerVipLevel.BrandId,
				PlayerCode: playerVipLevel.PlayerCode,
				VipLevel: playerVipLevel.VipLevel,
				Sign: -1,
				Amount: source.Amount,
				Remark: source.Remark,
				OperatorUserId: operatorUserId,
				OperatorUsername: operatorUsername,
				PlayerVipLevel: playerVipLevel),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	[HttpGet("{id:guid}", Name = "PlayerVipLevelExp_Form_Get")]
	public async ValueTask<PlayerVipLevelExpFormViewModel?> GetAsync(
		[FromServices] IPlayerVipLevelExpFormRepository repository,
		Guid id)
	{
		var form = await repository.GetAsync(
			id: id,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return form is null ? null : new PlayerVipLevelExpFormViewModel(
			Id: form.Id,
			BrandId: form.BrandId,
			PlayerCode: form.PlayerCode,
			VipLevel: form.VipLevel,
			Type: (Common.PlayerVipLevelExpFormType)form.Type,
			AutoState: form.AutoState,
			State: (Common.PlayerVipLevelExpFormState)form.State,
			Sign: form.Sign,
			Amount: form.Amount,
			ActualAmount: form.ActualAmount,
			Remark: form.Remark,
			CreatedOn: form.CreatedOn,
			CreatedUserId: form.CreatedUserId,
			CreatedUsername: form.CreatedUsername,
			UpdatedOn: form.UpdatedOn,
			UpdatedUserId: form.UpdatedUserId,
			UpdatedUsername: form.UpdatedUsername);
	}

	[HttpPatch("{id:guid}/pending", Name = "PlayerVipLevelExp_Form_Pending")]
	public ValueTask PendingAsync(
		[FromServices] IPlayerVipLevelExpFormRepository repository,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id)
		=> repository.PendingAsync(id, operatorUserId, operatorUsername, HttpContext.RequestAborted);

	[HttpPatch("{id:guid}/approved", Name = "PlayerVipLevelExp_Form_Approved")]
	public ValueTask ApprovedAsync(
		[FromServices] ICommandService<PlayerVipLevelExpFormApprovedCommand> service,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id,
		[FromBody] PlayerVipLevelExpFormApprovedViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerVipLevelExpFormApprovedCommand(
				id,
				source.Amount,
				source.Remark,
				operatorUserId,
				operatorUsername),
			HttpContext.RequestAborted);

	[HttpPatch("{id:guid}/reject", Name = "PlayerVipLevelExp_Form_Reject")]
	public ValueTask RejectAsync(
		[FromServices] IPlayerVipLevelExpFormRepository repository,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id,
		PlayerVipLevelExpFormRejectViewModel source)
		=> repository.RejectAsync(id, source.Remark, operatorUserId, operatorUsername, HttpContext.RequestAborted);
}
