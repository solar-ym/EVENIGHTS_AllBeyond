using System.Collections;
using UnityEngine;

public class VisualEntityAttacked : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnHit()
    {
        StartCoroutine(InternalOnHit());
    }

    private IEnumerator InternalOnHit()
    {
        Vector3 newScale = Vector3.one;
        newScale.x = 0.9f;
        this.transform.localScale = newScale;
        this.transform.Rotate(0, 0, 5.0f);

        yield return new WaitForSeconds(0.1f);

        this.transform.localScale = Vector3.one;
        this.transform.rotation = Quaternion.identity;
    }
}
