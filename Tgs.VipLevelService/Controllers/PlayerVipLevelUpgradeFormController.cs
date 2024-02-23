using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;
using Tgs.VipLevelService.ViewModels;

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
		var playerVipLevel = await playerService.GetAsync(
			query: new PlayerVipLevelSingleQuery(source.PlayerCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
		var snapshotId = await snapshotService.GetAsync(
			query: new VipLevelSnapshotCurrentSnapshotIdQuery(playerVipLevel.BrandId),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
		var vipLevelInfos = await snapshotQueryService.QueryAsync(
			query: new VipLevelSnapshotQueryVipLevelQuery(snapshotId),
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync()
			.ConfigureAwait(false)
			?? Enumerable.Empty<VipLevelInfo>();
		var vipLevelInfo = vipLevelInfos.Single(predicate: vl => vl.Level == playerVipLevel.VipLevel);

		return await formService.GetAsync(
			query: new PlayerVipLevelUpgradeFormCreateCommand(
				BrandId: playerVipLevel.BrandId,
				PlayerCode: playerVipLevel.PlayerCode,
				VipLevel: playerVipLevel.VipLevel,
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
			AutoState: form.AutoState,
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

	[HttpGet(Name = "PlayerVipLevelUpgrade_Form_Query")]
	public async IAsyncEnumerable<PlayerVipLevelUpgradeFormQueryViewModel> QueryAsync(
		[FromServices] IPlayerVipLevelUpgradeFormRepository repository,
		[FromQuery] PlayerVipLevelUpgradeFormFromQuery source)
	{
		var states = source.States?.Select(state => (Core.Protos.PlayerVipLevelUpgradeFormState)state).Distinct()
			?? Enumerable.Empty<Core.Protos.PlayerVipLevelUpgradeFormState>();

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
			?? Enumerable.Empty<PlayerVipLevelUpgradeFormQueryInfo>();

		if (!infos.Any())
		{
			yield break;
		}

		foreach (var info in infos)
		{
			yield return new PlayerVipLevelUpgradeFormQueryViewModel(
				Id: info.Id,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				Username: info.Username,
				VipLevel: info.VipLevel,
				AutoState: info.AutoState,
				State: (Common.PlayerVipLevelUpgradeFormState)info.State,
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

	[HttpGet("count", Name = "PlayerVipLevelUpgrade_Form_Query_Count")]
	public ValueTask<int> QueryCountAsync(
		[FromServices] IPlayerVipLevelUpgradeFormRepository repository,
		[FromQuery] PlayerVipLevelUpgradeFormFromQueryCount source)
	{
		var states = source.States?.Select(state => (Core.Protos.PlayerVipLevelUpgradeFormState)state).Distinct()
			?? Enumerable.Empty<Core.Protos.PlayerVipLevelUpgradeFormState>();

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
