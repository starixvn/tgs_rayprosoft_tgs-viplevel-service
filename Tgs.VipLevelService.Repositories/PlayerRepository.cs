using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerRepository(
	TgsContext context) : IPlayerRepository
{
	public async ValueTask<PlayerInfo> GetAsync(string playerCode, CancellationToken cancellationToken = default)
		=> await context.Players.AsQueryable()
		.Where(predicate => predicate.PlayerCode == playerCode)
		.Join(
		   context.Brands,
		   inner => inner.BrandId,
		   outer => outer.Id,
		   (player, brand) => new
		   {
			   brand.LicenseeId,
			   BrandId = brand.Id,
			   BrandCode = brand.Code,
			   player.PlayerCode,
			   player.Username
		   })
		.Join(
		   context.Licensees,
		   inner => inner.LicenseeId,
		   outer => outer.Id,
		   (combined, licensee) => new
		   {
			   licensee.ResellerId,
			   LicenseeId = licensee.Id,
			   LicenseeCode = licensee.Code,
			   combined.BrandId,
			   combined.BrandCode,
			   combined.PlayerCode,
			   combined.Username
		   })
		.Join(
			context.Resellers,
			inner => inner.ResellerId,
			outer => outer.Id,
			(combined, reseller) => new
			{
				ResellerId = reseller.Id,
				ResellerCode = reseller.Code,
				combined.LicenseeId,
				combined.LicenseeCode,
				combined.BrandId,
				combined.BrandCode,
				combined.PlayerCode,
				combined.Username
			})
		.Select(selector: selector => new PlayerInfo(
			selector.ResellerId,
			selector.ResellerCode,
			selector.LicenseeId,
			selector.LicenseeCode,
			selector.BrandId,
			selector.BrandCode,
			selector.PlayerCode,
			selector.Username))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		 ?? throw new NullReferenceException($"This player does not exist.{playerCode}");

	public async ValueTask<PlayerVipIdInfo> GetVipIdAsync(string playerCode, CancellationToken cancellationToken = default)
		=> await context.Players.AsQueryable()
		.Where(predicate => predicate.PlayerCode == playerCode)
		.Join(
		   context.Brands,
		   inner => inner.BrandId,
		   outer => outer.Id,
		   (player, brand) => new
		   {
			   brand.LicenseeId,
			   BrandId = brand.Id,
			   BrandCode = brand.Code,
			   player.PlayerCode,
			   player.Username,
			   VipId = player.VipId!.Value
		   })
		.Join(
		   context.Licensees,
		   inner => inner.LicenseeId,
		   outer => outer.Id,
		   (combined, licensee) => new
		   {
			   licensee.ResellerId,
			   LicenseeId = licensee.Id,
			   LicenseeCode = licensee.Code,
			   combined.BrandId,
			   combined.BrandCode,
			   combined.PlayerCode,
			   combined.Username,
			   combined.VipId
		   })
		.Join(
			context.Resellers,
			inner => inner.ResellerId,
			outer => outer.Id,
			(combined, reseller) => new
			{
				ResellerId = reseller.Id,
				ResellerCode = reseller.Code,
				combined.LicenseeId,
				combined.LicenseeCode,
				combined.BrandId,
				combined.BrandCode,
				combined.PlayerCode,
				combined.Username,
				combined.VipId
			})
		.Select(selector: selector => new PlayerVipIdInfo(
			selector.ResellerId,
			selector.ResellerCode,
			selector.LicenseeId,
			selector.LicenseeCode,
			selector.BrandId,
			selector.BrandCode,
			selector.PlayerCode,
			selector.Username,
			selector.VipId))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		 ?? throw new NullReferenceException($"This player does not exist.{playerCode}");

	public async ValueTask UpdateAsync(string playerCode, Guid vipLevelId, CancellationToken cancellationToken = default)
	{
		var player = await context.Players
			.SingleAsync(
			predicate: predicate => predicate.PlayerCode == playerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		player.VipId = vipLevelId;

		_ = context.Players.Update(
			entity: player);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
