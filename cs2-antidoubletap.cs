using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace AntiDoubleTap;

public class AntiDoubleTap : BasePlugin
{
    public override string ModuleName => "Anti DoubleTap";
    public override string ModuleVersion => "v1";
    public override string ModuleAuthor => "schwarper";
    public override string ModuleDescription => "Prevents double tap from working";

    [GameEventHandler]
    public HookResult OnBulletImpact(EventBulletImpact @event, GameEventInfo _)
    {
        var player = @event.Userid;
        if (player == null)
            return HookResult.Continue;

        var weapon = player.PlayerPawn.Value?.WeaponServices?.ActiveWeapon.Value;
        if (weapon == null)
            return HookResult.Continue;

        var weaponData = weapon.GetVData<CCSWeaponBaseVData>();
        if (weaponData == null)
            return HookResult.Continue;

        int tickBase = (int)player.TickBase;
        int fixedPrimaryTick = (int)Math.Round(weaponData.CycleTime.Values[0] * 64) - 1;
        weapon.NextPrimaryAttackTick = Math.Max(weapon.NextPrimaryAttackTick, tickBase + fixedPrimaryTick);
        Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_nNextPrimaryAttackTick");
        return HookResult.Continue;
    }
}