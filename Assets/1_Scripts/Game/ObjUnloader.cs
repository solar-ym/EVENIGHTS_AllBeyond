using UnityEngine;

public class ObjUnloader : MonoBehaviour
{
    public float timeToLive = 100.0f;

    private float timer = 0.0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeToLive)
        {
            Destroy(gameObject);
        }
    }
}
