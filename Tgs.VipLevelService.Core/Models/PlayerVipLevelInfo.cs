using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.Models;

public record PlayerVipLevelInfo(
   string PlayerCode,
   Guid VipLevelId,
   int VipLevel = 0,
   PlayerVipLevelAutoState AutoState = PlayerVipLevelAutoState.Enable);
