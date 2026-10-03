using UnityEngine;

public class ActiveTracker : MonoBehaviour
{
    [SerializeField] Player player;

    private void Update()
    {
        this.transform.position = player.GetActiveCharacter().transform.position;
    }
}
