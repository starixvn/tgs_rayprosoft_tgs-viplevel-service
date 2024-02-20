using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelExpFormCreateTurnoverCommandService(
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerVipLevelExpFormRepository repository) : IGetService<PlayerVipLevelExpFormCreateTurnoverCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(PlayerVipLevelExpFormCreateTurnoverCommand query, CancellationToken cancellationToken = default)
	{
		var id = await serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);

		/*
		 * 自動/手動
		 */
		var autoState = true;

		await repository.CreateAsync(
			id: id,
			brandId: query.BrandId,
			playerCode: query.PlayerCode,
			vipLevel: query.VipLevel,
			type: PlayerVipLevelExpFormType.Turnover,
			autoState: autoState,
			sign: query.Sign,
			amount: query.Amount,
			snapshotExp: new PlayerVipLevelExpFormSnapshotExpInfo(
				PlayerCode: query.PlayerVipLevel.PlayerCode,
				AccumDepositId: query.PlayerVipLevel.AccumDepositId,
				AccumDeposit: query.PlayerVipLevel.AccumDeposit,
				AccumTurnoverId: query.PlayerVipLevel.AccumTurnoverId,
				AccumTurnover: query.PlayerVipLevel.AccumTurnover),
			remark: query.Remark,
			createdUserId: query.OperatorUserId,
			createdUsername: query.OperatorUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return id;
	}
}
