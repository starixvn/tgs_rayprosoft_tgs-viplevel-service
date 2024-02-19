using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Controllers;

[Route("upgrade-form")]
[ApiController]
public class PlayerVipLevelUpgradeFormController : ControllerBase
{
	[HttpPost(Name = "PlayerVipLevelUpgrade_Form_Create")]
	public async ValueTask<Guid> CreateAsync(
		[FromServices] IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult> playerService,
		[FromServices] IGetService<VipLevelSnapshotCurrentSnapshotIdQuery, Guid> snapshotService,
		[FromServices] IQueryService<VipLevelSnapshotQueryVipLevelQuery, VipLevelInfo> snapshotQueryService,
		[FromServices] IGetService<PlayerVipLevelUpgradeFormCreateCommand, Guid> formService,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		[FromBody] PlayerVipLevelUpgradeFormCreateViewModel source)
	{
		var playerVipLevelPlayerCodeResult = await playerService.GetAsync(
			query: new PlayerVipLevelSingleQuery(source.PlayerCode),
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
			query: new PlayerVipLevelUpgradeFormCreateCommand(
				BrandId: playerVipLevelPlayerCodeResult.BrandId,
				PlayerCode: playerVipLevelPlayerCodeResult.PlayerCode,
				VipLevel: playerVipLevelPlayerCodeResult.VipLevel,
				Amount: vipLevelInfo.PromotionBenefit,
				OperatorUserId: operatorUserId,
				OperatorUsername: operatorUsername,
				VipLevelInfo: vipLevelInfo),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	[HttpGet("{id:guid}", Name = "PlayerVipLevelUpgrade_Form_Get")]
	public async ValueTask<PlayerVipLevelUpgradeFormViewModel?> GetAsync(
		[FromServices] IPlayerVipLevelUpgradeFormRepository repository,
		 Guid id)
	{
		var form = await repository.GetAsync(
			id: id,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return form is null ? null : new PlayerVipLevelUpgradeFormViewModel(
			Id: form.Id,
			BrandId: form.BrandId,
			PlayerCode: form.PlayerCode,
			VipLevel: form.VipLevel,
			State: (Common.PlayerVipLevelUpgradeFormState)form.State,
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

	[HttpPatch("{id:guid}/pending", Name = "PlayerVipLevelUpgrade_Form_Pending")]
	public ValueTask PendingAsync(
		[FromServices] IPlayerVipLevelUpgradeFormRepository repository,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id)
		=> repository.PendingAsync(id, operatorUserId, operatorUsername, HttpContext.RequestAborted);

	[HttpPatch("{id:guid}/approved", Name = "PlayerVipLevelUpgrade_Form_Approved")]
	public ValueTask ApprovedAsync(
		[FromServices] ICommandService<PlayerVipLevelUpgradeFormApprovedCommand> service,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id,
		[FromBody] PlayerVipLevelUpgradeFormApprovedViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerVipLevelUpgradeFormApprovedCommand(
				id,
				source.Amount,
				source.Remark,
				operatorUserId,
				operatorUsername),
			HttpContext.RequestAborted);

	[HttpPatch("{id:guid}/reject", Name = "PlayerVipLevelUpgrade_Form_Reject")]
	public ValueTask RejectAsync(
		[FromServices] IPlayerVipLevelUpgradeFormRepository repository,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id,
		PlayerVipLevelUpgradeFormRejectViewModel source)
		=> repository.RejectAsync(id, source.Remark, operatorUserId, operatorUsername, HttpContext.RequestAborted);

	[HttpGet("exist/{playerCode}/{vipLevel}", Name = "PlayerVipLevelUpgrade_Form_Exist")]
	public ValueTask<bool> ExistAsync(
		[FromServices] IPlayerVipLevelUpgradeFormRepository repository,
		string playerCode,
		int vipLevel)
		=> repository.ExistAsync(playerCode, vipLevel, HttpContext.RequestAborted);
}
