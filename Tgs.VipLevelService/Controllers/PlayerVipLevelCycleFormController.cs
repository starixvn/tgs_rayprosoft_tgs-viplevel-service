using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Controllers;

[Route("cycle-form")]
[ApiController]
public class PlayerVipLevelCycleFormController : ControllerBase
{
	[HttpPost(Name = "PlayerVipLevelCycle_Form_Create")]
	public async ValueTask<Guid> CreateAsync(
		[FromServices] IGetService<PlayerVipLevelPlayerCodeQuery, PlayerVipLevelPlayerCodeResult> playerService,
		[FromServices] IGetService<VipLevelSnapshotCurrentSnapshotIdQuery, Guid> snapshotService,
		[FromServices] IQueryService<VipLevelSnapshotQueryVipLevelQuery, VipLevelInfo> snapshotQueryService,
		[FromServices] IGetService<PlayerVipLevelCycleFormCreateCommand, Guid> formService,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		[FromBody] PlayerVipLevelCycleFormCreateViewModel source)
	{
		var playerVipLevelPlayerCodeResult = await playerService.GetAsync(
			query: new PlayerVipLevelPlayerCodeQuery(source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
		var snapshotId = await snapshotService.GetAsync(
			query: new VipLevelSnapshotCurrentSnapshotIdQuery(playerVipLevelPlayerCodeResult.BrandId),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
		var vipLevelInfos = await snapshotQueryService.QueryAsync(
			query: new VipLevelSnapshotQueryVipLevelQuery(snapshotId),
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync()
			.ConfigureAwait(false)
			?? Enumerable.Empty<VipLevelInfo>();
		var vipLevelInfo = vipLevelInfos.Single(predicate: vl => vl.Level == playerVipLevelPlayerCodeResult.VipLevel);

		return await formService.GetAsync(
			query: new PlayerVipLevelCycleFormCreateCommand(
				BrandId: playerVipLevelPlayerCodeResult.BrandId,
				PlayerCode: playerVipLevelPlayerCodeResult.PlayerCode,
				VipLevel: playerVipLevelPlayerCodeResult.VipLevel,
				CycleType: (Core.Protos.PlayerVipLevelCycleFormCycleType)source.CycleType,
				CycleOn: source.CycleOn,
				FromOn: source.FromOn,
				ToOn: source.ToOn,
				Amount: vipLevelInfo.BirthBenefit,
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
}
