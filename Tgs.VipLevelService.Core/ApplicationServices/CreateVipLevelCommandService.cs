using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class CreateVipLevelCommandService(ITgsRepository tgsRepository) : IGetService<CreateVipLevelCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(CreateVipLevelCommand command, CancellationToken cancellationToken = default)
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
