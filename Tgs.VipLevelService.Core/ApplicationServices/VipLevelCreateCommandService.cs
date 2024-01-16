using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelCreateCommandService(ITgsRepository tgsRepository) : IGetService<VipLevelCreateCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(VipLevelCreateCommand command, CancellationToken cancellationToken = default)
	{
		var newId = Guid.NewGuid();

		await tgsRepository.CreateVipLevelAsync(
			newId,
			command.BrandId,
			command.CreateModel,
			command.Modifier,
			cancellationToken).ConfigureAwait(false);

		return newId;
	}
}
