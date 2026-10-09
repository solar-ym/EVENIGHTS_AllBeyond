using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject normalDamage;
    [SerializeField] private GameObject shadeDamage;
    [SerializeField] private GameObject cursedDamage;
    [SerializeField] private GameObject holyDamage;
    [SerializeField] private GameObject healText;

    public void SpawnDamageNumber(DamageCalculator.DAMAGE_TYPE damageType, int damageValue)
    {

        GameObject dmg;
        TextMeshProUGUI text;

        switch (damageType)
        {
            case (DamageCalculator.DAMAGE_TYPE.normal):
                
                dmg = Instantiate(normalDamage,this.transform);
                text = dmg.GetComponent<TextMeshProUGUI>();
                text.text = damageValue.ToString();

                dmg.transform.localRotation = Quaternion.identity;

                break;
            case (DamageCalculator.DAMAGE_TYPE.cursed):

                dmg = Instantiate(cursedDamage, this.transform);
                text = dmg.GetComponent<TextMeshProUGUI>();
                text.text = damageValue.ToString();

                dmg.transform.localRotation = Quaternion.identity;

                break;
            case (DamageCalculator.DAMAGE_TYPE.shade):

                dmg = Instantiate(shadeDamage, this.transform);
                text = dmg.GetComponent<TextMeshProUGUI>();
                text.text = damageValue.ToString();

                dmg.transform.localRotation = Quaternion.identity;

                break;
            case (DamageCalculator.DAMAGE_TYPE.holy):

                dmg = Instantiate(holyDamage, this.transform);
                text = dmg.GetComponent<TextMeshProUGUI>();
                text.text = damageValue.ToString();

                dmg.transform.localRotation = Quaternion.identity;

                break;
        }
    }

    public void SpawnHealText(int healValue)
    {
        GameObject heal;
        TextMeshProUGUI text;

        heal = Instantiate(healText,this.transform);
        text = heal.GetComponent<TextMeshProUGUI>();
        text.text = "+" + healValue.ToString();

    }
}
