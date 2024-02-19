using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Controllers;

[Route("birthday-form")]
[ApiController]
public class PlayerVipLevelBirthdayFormController : ControllerBase
{
	[HttpPost(Name = "PlayerVipLevelBirthday_Form_Create")]
	public async ValueTask<Guid> CreateAsync(
		[FromServices] IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult> playerService,
		[FromServices] IGetService<VipLevelSnapshotCurrentSnapshotIdQuery, Guid> snapshotService,
		[FromServices] IQueryService<VipLevelSnapshotQueryVipLevelQuery, VipLevelInfo> snapshotQueryService,
		[FromServices] IGetService<PlayerVipLevelBirthdayFormCreateCommand, Guid> formService,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		[FromBody] PlayerVipLevelBirthdayFormCreateViewModel source)
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
			query: new PlayerVipLevelBirthdayFormCreateCommand(
				BrandId: playerVipLevelPlayerCodeResult.BrandId,
				PlayerCode: playerVipLevelPlayerCodeResult.PlayerCode,
				VipLevel: playerVipLevelPlayerCodeResult.VipLevel,
				Birthday: source.Birthday,
				Amount: vipLevelInfo.BirthBenefit,
				OperatorUserId: operatorUserId,
				OperatorUsername: operatorUsername,
				VipLevelInfo: vipLevelInfo),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	[HttpGet("{id:guid}", Name = "PlayerVipLevelBirthday_Form_Get")]
	public async ValueTask<PlayerVipLevelBirthdayFormViewModel?> GetAsync(
		[FromServices] IPlayerVipLevelBirthdayFormRepository repository,
		 Guid id)
	{
		var form = await repository.GetAsync(
			id: id,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return form is null ? null : new PlayerVipLevelBirthdayFormViewModel(
			Id: form.Id,
			BrandId: form.BrandId,
			PlayerCode: form.PlayerCode,
			VipLevel: form.VipLevel,
			Birthday: form.Birthday,
			State: (Common.PlayerVipLevelBirthdayFormState)form.State,
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

	[HttpPatch("{id:guid}/pending", Name = "PlayerVipLevelBirthday_Form_Pending")]
	public ValueTask PendingAsync(
		[FromServices] IPlayerVipLevelBirthdayFormRepository repository,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id)
		=> repository.PendingAsync(id, operatorUserId, operatorUsername, HttpContext.RequestAborted);

	[HttpPatch("{id:guid}/approved", Name = "PlayerVipLevelBirthday_Form_Approved")]
	public ValueTask ApprovedAsync(
		[FromServices] ICommandService<PlayerVipLevelBirthdayFormApprovedCommand> service,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id,
		[FromBody] PlayerVipLevelBirthdayFormApprovedViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerVipLevelBirthdayFormApprovedCommand(
				id,
				source.Amount,
				source.Remark,
				operatorUserId,
				operatorUsername),
			HttpContext.RequestAborted);

	[HttpPatch("{id:guid}/reject", Name = "PlayerVipLevelBirthday_Form_Reject")]
	public ValueTask RejectAsync(
		[FromServices] IPlayerVipLevelBirthdayFormRepository repository,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		Guid id,
		PlayerVipLevelBirthdayFormRejectViewModel source)
		=> repository.RejectAsync(id, source.Remark, operatorUserId, operatorUsername, HttpContext.RequestAborted);

	[HttpGet("exist/{playerCode}/{birthday}", Name = "PlayerVipLevelBirthday_Form_Exist")]
	public ValueTask<bool> ExistAsync(
		[FromServices] IPlayerVipLevelBirthdayFormRepository repository,
		string playerCode,
		DateOnly birthday)
		=> repository.ExistAsync(playerCode, birthday, HttpContext.RequestAborted);
}
