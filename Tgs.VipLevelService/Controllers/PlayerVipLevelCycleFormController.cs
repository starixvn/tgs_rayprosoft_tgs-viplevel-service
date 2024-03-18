using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;
using Tgs.VipLevelService.ViewModels;

namespace Tgs.VipLevelService.Controllers;

[Route("cycle-form")]
[ApiController]
public class PlayerVipLevelCycleFormController : ControllerBase
{
	[HttpPost(Name = "PlayerVipLevelCycle_Form_Create")]
	public async ValueTask<Guid> CreateAsync(
		[FromServices] IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult> playerService,
		[FromServices] IGetService<VipLevelSingleByLevelQuery, VipLevelInfo?> vipLevelSingleService,
		[FromServices] IGetService<PlayerVipLevelCycleFormCreateCommand, Guid> formService,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		[FromBody] PlayerVipLevelCycleFormCreateViewModel source)
	{
		var playerVipLevel = await playerService.GetAsync(
			query: new PlayerVipLevelSingleQuery(source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
		var vipLevelInfo = await vipLevelSingleService.GetAsync(
			query: new VipLevelSingleByLevelQuery(playerVipLevel.BrandId, playerVipLevel.VipLevel),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(playerVipLevel.VipLevel.ToString());

		return await formService.GetAsync(
			query: new PlayerVipLevelCycleFormCreateCommand(
				BrandId: playerVipLevel.BrandId,
				PlayerCode: playerVipLevel.PlayerCode,
				VipLevel: playerVipLevel.VipLevel,
				CycleType: (Core.Protos.PlayerVipLevelCycleFormCycleType)source.CycleType,
				CycleOn: source.CycleOn,
				FromOn: source.FromOn,
				ToOn: source.ToOn,
				Amount: vipLevelInfo.QuarterlyBenefit,
				OperatorUserId: operatorUserId,
				OperatorUsername: operatorUsername,
				VipLevelInfo: vipLevelInfo),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	[HttpGet("{id:guid}", Name = "PlayerVipLevelCycle_Form_Get")]
	public async ValueTask<PlayerVipLevelCycleFormViewModel?> GetAsync(
		[FromServices] IPlayerVipLevelCycleFormRepository repository,
		 Guid id)
	{
		var form = await repository.GetAsync(
			id: id,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return form is null ? null : new PlayerVipLevelCycleFormViewModel(
			Id: form.Id,
			BrandId: form.BrandId,
			PlayerCode: form.PlayerCode,
			VipLevel: form.VipLevel,
			CycleType: (Common.PlayerVipLevelCycleFormCycleType)form.CycleType,
			CycleOn: form.CycleOn,
			FromOn: form.FromOn,
			ToOn: form.ToOn,
			AutoState: form.AutoState,
			State: (Common.PlayerVipLevelCycleFormState)form.State,
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

	[HttpPatch("{id:guid}/pending", Name = "PlayerVipLevelCycle_Form_Pending")]
	public ValueTask PendingAsync(
		[FromServices] IPlayerVipLevelCycleFormRepository repository,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id)
		=> repository.PendingAsync(id, operatorUserId, operatorUsername, HttpContext.RequestAborted);

	[HttpPatch("{id:guid}/approved", Name = "PlayerVipLevelCycle_Form_Approved")]
	public ValueTask ApprovedAsync(
		[FromServices] ICommandService<PlayerVipLevelCycleFormApprovedCommand> service,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id,
		[FromBody] PlayerVipLevelCycleFormApprovedViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerVipLevelCycleFormApprovedCommand(
				id,
				source.Amount,
				source.Remark,
				operatorUserId,
				operatorUsername),
			HttpContext.RequestAborted);

	[HttpPatch("{id:guid}/reject", Name = "PlayerVipLevelCycle_Form_Reject")]
	public ValueTask RejectAsync(
		[FromServices] IPlayerVipLevelCycleFormRepository repository,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id,
		PlayerVipLevelCycleFormRejectViewModel source)
		=> repository.RejectAsync(id, source.Remark, operatorUserId, operatorUsername, HttpContext.RequestAborted);

	[HttpGet("exist/{playerCode}/{cycleType}/{cycleOn}", Name = "PlayerVipLevelCycle_Form_Exist")]
	public ValueTask<bool> ExistAsync(
		[FromServices] IPlayerVipLevelCycleFormRepository repository,
		string playerCode,
		Common.PlayerVipLevelCycleFormCycleType cycleType,
		DateOnly cycleOn)
		=> repository.ExistAsync(playerCode, (Core.Protos.PlayerVipLevelCycleFormCycleType)cycleType, cycleOn, HttpContext.RequestAborted);

	[HttpGet(Name = "PlayerVipLevelCycle_Form_Query")]
	public async IAsyncEnumerable<PlayerVipLevelCycleFormQueryViewModel> QueryAsync(
		[FromServices] IPlayerVipLevelCycleFormRepository repository,
		[FromQuery] PlayerVipLevelCycleFormFromQuery source)
	{
		var states = source.States?.Select(state => (Core.Protos.PlayerVipLevelCycleFormState)state).Distinct()
			?? Enumerable.Empty<Core.Protos.PlayerVipLevelCycleFormState>();

		var infos = await repository.QueryAsync(
			brandId: source.BrandId,
			updatedOnStart: source.UpdatedOnStart,
			updatedOnEnd: source.UpdatedOnEnd,
			playerCode: source.PlayerCode ?? string.Empty,
			username: source.Username ?? string.Empty,
			states: states,
			offset: source.Page - 1,
			limit: source.RowCount,
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync()
			.ConfigureAwait(false)
			?? Enumerable.Empty<PlayerVipLevelCycleFormQueryInfo>();

		if (!infos.Any())
		{
			yield break;
		}

		foreach (var info in infos)
		{
			yield return new PlayerVipLevelCycleFormQueryViewModel(
				Id: info.Id,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				Username: info.Username,
				VipLevel: info.VipLevel,
				CycleType: (Common.PlayerVipLevelCycleFormCycleType)info.CycleType,
				CycleOn: info.CycleOn,
				FromOn: info.FromOn,
				ToOn: info.ToOn,
				AutoState: info.AutoState,
				State: (Common.PlayerVipLevelCycleFormState)info.State,
				Amount: info.Amount,
				ActualAmount: info.ActualAmount,
				Remark: info.Remark,
				CreatedOn: info.CreatedOn,
				CreatedUserId: info.CreatedUserId,
				CreatedUsername: info.CreatedUsername,
				UpdatedOn: info.UpdatedOn,
				UpdatedUserId: info.UpdatedUserId,
				UpdatedUsername: info.UpdatedUsername);
		}
	}

	[HttpGet("count", Name = "PlayerVipLevelCycle_Form_Query_Count")]
	public ValueTask<int> QueryCountAsync(
		[FromServices] IPlayerVipLevelCycleFormRepository repository,
		[FromQuery] PlayerVipLevelCycleFormFromQueryCount source)
	{
		var states = source.States?.Select(state => (Core.Protos.PlayerVipLevelCycleFormState)state).Distinct()
			?? Enumerable.Empty<Core.Protos.PlayerVipLevelCycleFormState>();

		return repository.QueryCountAsync(
			brandId: source.BrandId,
			updatedOnStart: source.UpdatedOnStart,
			updatedOnEnd: source.UpdatedOnEnd,
			playerCode: source.PlayerCode ?? string.Empty,
			username: source.Username ?? string.Empty,
			states: states,
			cancellationToken: HttpContext.RequestAborted);
	}
}
