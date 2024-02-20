using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelExpFormApprovedCommandService(
	ILogger<PlayerVipLevelExpFormApprovedCommandService> logger,
	IPlayerVipLevelExpFormRepository playerVipLevelExpFormRepository) : ICommandService<PlayerVipLevelExpFormApprovedCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelExpFormApprovedCommand command, CancellationToken cancellationToken = default)
	{
		var form = await playerVipLevelExpFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException($"This form cannot be found.{command.Id}");

		if (form.State is Core.Protos.PlayerVipLevelExpFormState.Approved or Core.Protos.PlayerVipLevelExpFormState.Reject)
		{
			logger.LogWarning("LogOn:{logOn} Message:This form({id}) has been reviewed.({state})",
				DateTime.Now.ToString("u"),
				form.Id,
				form.State);
			return;
		}

		await playerVipLevelExpFormRepository.ApprovedAsync(
			id: form.Id,
			amount: command.Amount,
			remark: command.Remark,
			updatedUserId: command.OperatorUserId,
			updatedUsername: command.OperatorUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		/*
		 * 發送通知
		 */
	}
}
