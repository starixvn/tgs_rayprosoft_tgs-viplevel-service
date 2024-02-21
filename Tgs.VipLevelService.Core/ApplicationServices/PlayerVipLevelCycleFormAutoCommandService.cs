using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelCycleFormAutoCommandService(
	ILogger<PlayerVipLevelCycleFormAutoCommandService> logger,
	IPlayerVipLevelCycleFormRepository repository,
	ICommandService<PlayerVipLevelCycleFormApprovedCommand> service) : ICommandService<PlayerVipLevelCycleFormAutoCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelCycleFormAutoCommand command, CancellationToken cancellationToken = default)
	{
		var form = await repository.GetAsync(
			id: command.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException($"This form cannot be found.{command.Id}");

		if (form.State is not Core.Protos.PlayerVipLevelCycleFormState.Create)
		{
			logger.LogWarning("LogOn:{logOn} Message:This form({id}) status is not created.({state})",
				DateTime.Now.ToString("u"),
				form.Id,
				form.State);
			return;
		}

		if (form.AutoState)
		{
			await service.ExecuteAsync(
				command: new PlayerVipLevelCycleFormApprovedCommand(
					Id: form.Id,
					Amount: form.Amount,
					Remark: form.Remark,
					OperatorUserId: form.CreatedUserId,
					OperatorUsername: form.CreatedUsername),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		else
		{
			await repository.PendingAsync(
				form.Id,
				form.CreatedUserId,
				form.CreatedUsername,
				cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
