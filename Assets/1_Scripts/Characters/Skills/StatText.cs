using TMPro;
using UnityEngine;

public class StatText : MonoBehaviour
{
    [Header("Note: Contrary to the name, this script is actually really important and also manages the stat adding.")]

    [Header("Attached Character")]
    public Player player;
    public Character character;

    [Header("All Text")]
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI resText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI spText;
    public TextMeshProUGUI spdText;
    public TextMeshProUGUI dashText;

    private void Start()
    {
        SetChar(0);
    }
    private void Update()
    {
        if (character != null)
        {
            atkText.text = character.attack.ToString();
            magText.text = character.magic.ToString();
            resText.text = character.resist.ToString();
            hpText.text = character.health.ToString();
            spText.text = character.sp.ToString();
        }
    }

    public void SetChar(int index)
    {
        character = player.GetThisCharacter(index);

        spdText.text = character.movementSpeed.ToString();
        dashText.text = character.dashSpeed.ToString();
    }

    public void AddToStat(int stat)
    {
        GameEntity.STATS thisStat = (GameEntity.STATS)stat;

        if (character.sp > 0)
        {
            character.AlterStat(thisStat, 1);
            character.sp -= 1;
        }
    }
}
