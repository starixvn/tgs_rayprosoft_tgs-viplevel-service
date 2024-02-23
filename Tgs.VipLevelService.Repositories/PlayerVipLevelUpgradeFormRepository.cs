using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelUpgradeFormRepository(
	TgsContext context) : IPlayerVipLevelUpgradeFormRepository
{
	public async ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelUpgradeForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelUpgradeFormState.Approved;
		form.ActualAmount = amount;
		form.Remark = remark;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, bool autoState, decimal amount, PlayerVipLevelUpgradeFormSnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		var form = new PlayerVipLevelUpgradeForm
		{
			Id = id,
			BrandId = brandId,
			PlayerCode = playerCode,
			VipLevel = vipLevel,
			AutoState = autoState,
			State = (int)PlayerVipLevelUpgradeFormState.Create,
			Amount = amount,
			ActualAmount = 0,
			SnapshotVipLevel = JsonSerializer.Serialize(snapshotVipLevel),
			CreatedOn = DateTime.UtcNow,
			CreatedUserId = createdUserId,
			CreatedUsername = createdUsername,
			UpdatedUserId = createdUserId,
			UpdatedUsername = createdUsername,
			Remark = string.Empty
		};

		_ = await context.PlayerVipLevelUpgradeForms.AddAsync(
			form,
			cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(string playerCode, int vipLevel, CancellationToken cancellationToken = default)
		=> await context.PlayerVipLevelUpgradeForms
		.Where(predicate: f => f.PlayerCode == playerCode && f.VipLevel == vipLevel)
		.AnyAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<PlayerVipLevelUpgradeFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await context.PlayerVipLevelUpgradeForms
		.Where(predicate: form => form.Id == id)
		.Select(selector: form => new PlayerVipLevelUpgradeFormInfo(
			form.Id,
			form.BrandId,
			form.PlayerCode,
			form.VipLevel,
			form.AutoState,
			(PlayerVipLevelUpgradeFormState)form.State,
			form.Amount,
			form.ActualAmount,
			form.Remark,
			form.CreatedOn,
			form.CreatedUserId,
			form.CreatedUsername,
			form.UpdatedOn,
			form.UpdatedUserId,
			form.UpdatedUsername))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask PendingAsync(Guid id, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelUpgradeForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelUpgradeFormState.Pending;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public IAsyncEnumerable<PlayerVipLevelUpgradeFormQueryInfo> QueryAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelUpgradeFormState> states, int offset = 0, int limit = 20, CancellationToken cancellationToken = default)
	{
		var filters = context.PlayerVipLevelUpgradeForms
			.Join(
			inner: context.Players,
			outerKeySelector: form => form.PlayerCode,
			innerKeySelector: player => player.PlayerCode,
			resultSelector: (form, player) => new { Form = form, Player = player })
			.Where(predicate: combined => combined.Form.UpdatedOn >= updatedOnStart.UtcDateTime && combined.Form.UpdatedOn < updatedOnEnd.UtcDateTime)
			.Where(predicate: combined => combined.Form.BrandId == brandId);

		if (!string.IsNullOrWhiteSpace(username))
		{
			filters = filters.Where(predicate: combined => combined.Player.Username.StartsWith(username) || combined.Player.Username == username);
		}

		if (!string.IsNullOrWhiteSpace(playerCode))
		{
			filters = filters.Where(predicate: combined => combined.Form.PlayerCode == playerCode);
		}

		if (states.Any())
		{
			filters = filters.Where(predicate: combined => states.Contains((PlayerVipLevelUpgradeFormState)combined.Form.State));
		}

		return filters
		 .OrderByDescending(keySelector: combined => combined.Form.UpdatedOn)
		 .Skip(limit * offset)
		 .Take(limit)
		 .Select(selector: combined => new PlayerVipLevelUpgradeFormQueryInfo(
			 combined.Form.Id,
			 combined.Form.BrandId,
			 combined.Form.PlayerCode,
			 combined.Player.Username,
			 combined.Form.VipLevel,
			 combined.Form.AutoState,
			 (PlayerVipLevelUpgradeFormState)combined.Form.State,
			 combined.Form.Amount,
			 combined.Form.ActualAmount,
			 combined.Form.Remark,
			 combined.Form.CreatedOn,
			 combined.Form.CreatedUserId,
			 combined.Form.CreatedUsername,
			 combined.Form.UpdatedOn,
			 combined.Form.UpdatedUserId,
			 combined.Form.UpdatedUsername))
		 .ToAsyncEnumerable();
	}

	public async ValueTask<int> QueryCountAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelUpgradeFormState> states, CancellationToken cancellationToken = default)
	{
		var filters = context.PlayerVipLevelUpgradeForms
			.Join(
			inner: context.Players,
			outerKeySelector: form => form.PlayerCode,
			innerKeySelector: player => player.PlayerCode,
			resultSelector: (form, player) => new { Form = form, Player = player })
			.Where(predicate: combined => combined.Form.UpdatedOn >= updatedOnStart.UtcDateTime && combined.Form.UpdatedOn < updatedOnEnd.UtcDateTime)
			.Where(predicate: combined => combined.Form.BrandId == brandId);

		if (!string.IsNullOrWhiteSpace(username))
		{
			filters = filters.Where(predicate: combined => combined.Player.Username.StartsWith(username) || combined.Player.Username == username);
		}

		if (!string.IsNullOrWhiteSpace(playerCode))
		{
			filters = filters.Where(predicate: combined => combined.Form.PlayerCode == playerCode);
		}

		if (states.Any())
		{
			filters = filters.Where(predicate: combined => states.Contains((PlayerVipLevelUpgradeFormState)combined.Form.State));
		}

		return await filters.CountAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelUpgradeForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelUpgradeFormState.Reject;
		form.ActualAmount = form.Amount;
		form.Remark = remark;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
