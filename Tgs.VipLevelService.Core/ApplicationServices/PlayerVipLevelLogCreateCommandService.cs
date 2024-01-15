using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelLogCreateCommandService(
	ISerialNumberGenerator serialNumberGenerator,
	IVipLevelRepository vipLevelRepository,
	IPlayerVipLevelLogRepository playerVipLevelLogRepository) : ICommandService<PlayerVipLevelLogCreateCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelLogCreateCommand command, CancellationToken cancellationToken = default)
	{
		var beforePlayerVipLevelInfo = default(PlayerVipLevelInfo);
		if (command.BeforeLevel.HasValue)
		{
			var beforeVipLevelInfoInfo = await vipLevelRepository.GetAsync(
				brandId: command.BrandId,
				level: command.BeforeLevel.Value,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (beforeVipLevelInfoInfo != null)
			{
				beforePlayerVipLevelInfo = new PlayerVipLevelInfo(
					PlayerCode: command.PlayerCode,
					VipLevelId: beforeVipLevelInfoInfo.Id,
					VipLevel: beforeVipLevelInfoInfo.Level,
					AutoState: command.BeforeAutoState ?? PlayerVipLevelAutoState.ENABLE);
			}
		}

		var afterVipLevelInfoInfo = await vipLevelRepository.GetAsync(
			brandId: command.BrandId,
			level: command.AfterLevel,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException();

		var afterPlayerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: command.PlayerCode,
			VipLevelId: afterVipLevelInfoInfo.Id,
			VipLevel: afterVipLevelInfoInfo.Level,
			AutoState: command.BeforeAutoState ?? PlayerVipLevelAutoState.ENABLE);

		var id = await serialNumberGenerator.GenerateAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await playerVipLevelLogRepository.CreateAsync(
			id: id,
			playerCode: command.PlayerCode,
			type: command.Type,
			before: beforePlayerVipLevelInfo,
			after: afterPlayerVipLevelInfo!,
			createdUserId: command.CreatedUserId,
			createdUsername: command.CreatedUsername,
			remark: command.Remark,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
