using TMPro;
using UnityEngine;

public class DebugPause : MonoBehaviour
{
    public TMP_InputField input;
    public void ChangeSpeedMult()
    {
        float mult = PersistentValues.speedMultiplier;
        if (float.TryParse(input.text, out mult))
        {
            PersistentValues.speedMultiplier = mult;
        }
    }
}
