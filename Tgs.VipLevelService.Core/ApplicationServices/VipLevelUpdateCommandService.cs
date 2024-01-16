using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelUpdateCommandService(IVipLevelRepository repository) : ICommandService<VipLevelUpdateCommand>
{
	public async ValueTask ExecuteAsync(VipLevelUpdateCommand command, CancellationToken cancellationToken = default)
	{
		var vipLevelInfo = await repository.GetAsync(
			id: command.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException();

		await repository.UpdateAsync(
			id: vipLevelInfo.Id,
			brandId: vipLevelInfo.BrandId,
			update: command.Update,
			modifier: command.Modifier,
			cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
}
