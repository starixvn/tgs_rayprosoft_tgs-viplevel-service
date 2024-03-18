using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;
using Tgs.VipLevelService.ViewModels;

namespace Tgs.VipLevelService.Controllers;

[Route("birthday-form")]
[ApiController]
public class PlayerVipLevelBirthdayFormController : ControllerBase
{
	[HttpPost(Name = "PlayerVipLevelBirthday_Form_Create")]
	public async ValueTask<Guid> CreateAsync(
		[FromServices] IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult> playerService,
		[FromServices] IGetService<VipLevelSingleByLevelQuery, VipLevelInfo?> vipLevelSingleService,
		[FromServices] IGetService<PlayerVipLevelBirthdayFormCreateCommand, Guid> formService,
		[FromHeader(Name = "OperatorUserId")] Guid operatorUserId,
		[FromHeader(Name = "OperatorUsername")] string operatorUsername,
		[FromBody] PlayerVipLevelBirthdayFormCreateViewModel source)
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
			query: new PlayerVipLevelBirthdayFormCreateCommand(
				BrandId: playerVipLevel.BrandId,
				PlayerCode: playerVipLevel.PlayerCode,
				VipLevel: playerVipLevel.VipLevel,
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
			AutoState: form.AutoState,
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

	[HttpGet(Name = "PlayerVipLevelBirthday_Form_Query")]
	public async IAsyncEnumerable<PlayerVipLevelBirthdayFormQueryViewModel> QueryAsync(
		[FromServices] IPlayerVipLevelBirthdayFormRepository repository,
		[FromQuery] PlayerVipLevelBirthdayFormFromQuery source)
	{
		var states = source.States?.Select(state => (Core.Protos.PlayerVipLevelBirthdayFormState)state).Distinct()
			?? Enumerable.Empty<Core.Protos.PlayerVipLevelBirthdayFormState>();

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
			?? Enumerable.Empty<PlayerVipLevelBirthdayFormQueryInfo>();

		if (!infos.Any())
		{
			yield break;
		}

		foreach (var info in infos)
		{
			yield return new PlayerVipLevelBirthdayFormQueryViewModel(
				Id: info.Id,
				BrandId: info.BrandId,
				PlayerCode: info.PlayerCode,
				Username: info.Username,
				VipLevel: info.VipLevel,
				Birthday: info.Birthday,
				AutoState: info.AutoState,
				State: (Common.PlayerVipLevelBirthdayFormState)info.State,
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

	[HttpGet("count", Name = "PlayerVipLevelBirthday_Form_Count")]
	public ValueTask<int> QueryCountAsync(
		[FromServices] IPlayerVipLevelBirthdayFormRepository repository,
		[FromQuery] PlayerVipLevelBirthdayFormFromQueryCount source)
	{
		var states = source.States?.Select(state => (Core.Protos.PlayerVipLevelBirthdayFormState)state).Distinct()
			?? Enumerable.Empty<Core.Protos.PlayerVipLevelBirthdayFormState>();

		return repository.QueryCountAsync(
			brandId: source.BrandId,
			updatedOnStart: source.UpdatedOnStart,
			updatedOnEnd: source.UpdatedOnEnd,
			playerCode: source.PlayerCode ?? string.Empty,
			username: source.Username ?? string.Empty,
			states: states.ToArray(),
			cancellationToken: HttpContext.RequestAborted);
	}
}
