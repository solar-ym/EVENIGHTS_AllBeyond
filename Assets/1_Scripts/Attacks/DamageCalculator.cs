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
        int finalDamage = 0;
        int baseDmg = 0;
        int resistance = target.resist + target.resMod;

        if (dmgType == DAMAGE_TYPE.cursed)
        {
            baseDmg = attacker.magic + attacker.magMod;
        } else
        {
            baseDmg = attacker.attack + attacker.atkMod;
        }

        finalDamage = baseDmg - resistance;

        if (dmgType == DAMAGE_TYPE.shade)
        {
            finalDamage = Mathf.CeilToInt(finalDamage * 1.5f);
        }


        if (finalDamage < 0) finalDamage = 0;

        //Debug.Log(attacker.gameObject.name + ": did " + finalDamage + " damage to " + target.gameObject.name);

        finalDamage *= -1;

        return finalDamage;
    }
}
