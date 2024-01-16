using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelCreateCommandService(IVipLevelRepository repository) : IGetService<VipLevelCreateCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(VipLevelCreateCommand command, CancellationToken cancellationToken = default)
	{
		var newId = Guid.NewGuid();

		await repository.CreateAsync(
			id: newId,
			brandId: command.BrandId,
			create: command.CreateModel,
			modifier: command.Modifier,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		return newId;
	}
}
