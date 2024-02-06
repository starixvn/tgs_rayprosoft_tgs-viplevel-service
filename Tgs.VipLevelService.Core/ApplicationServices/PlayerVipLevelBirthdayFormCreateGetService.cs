using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelBirthdayFormCreateGetService(
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerVipLevelBirthdayFormRepository repository
	) : IGetService<PlayerVipLevelBirthdayFormCreateCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(PlayerVipLevelBirthdayFormCreateCommand query, CancellationToken cancellationToken = default)
	{
		var id = await serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);
		await repository.CreateAsync(
			id: id,
			brandId: query.BrandId,
			playerCode: query.PlayerCode,
			vipLevel: query.VipLevel,
			birthday: query.Birthday,
			amount: query.Amount,
			snapshotVipLevel: new PlayerVipLevelBirthdaySnapshotVipLevelInfo(
				VipLevelId: query.VipLevelInfo.Id,
				VipLevel: query.VipLevelInfo.Level,
				BrandId: query.VipLevelInfo.BrandId,
				BirthBenefit: query.VipLevelInfo.BirthBenefit),
			createdUserId: query.OperatorUserId,
			createdUsername: query.OperatorUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return id;
	}
}
