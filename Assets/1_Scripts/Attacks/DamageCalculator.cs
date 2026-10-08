using UnityEngine;

public static class DamageCalculator
{
    public enum DAMAGE_TYPE
    {
        normal,
        cursed,
        shade,
        holy
    }
    public static int CalculateDamage(GameEntity attacker, GameEntity target, DAMAGE_TYPE dmgType)
    {

        return 10;
    }
}
