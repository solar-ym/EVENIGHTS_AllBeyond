using UnityEngine;
using UnityEngine.UI;

public class VisualFollowBar : MonoBehaviour
{
    public Image toFollow;
    public Image thisImage;

    // Update is called once per frame
    void Update()
    {
        if (thisImage.fillAmount != toFollow.fillAmount)
        {
            float amt = (toFollow.fillAmount - thisImage.fillAmount) * Time.deltaTime;

            thisImage.fillAmount += amt;
        }
    }
}
