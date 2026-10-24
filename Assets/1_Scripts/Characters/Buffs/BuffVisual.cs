using UnityEngine;
using UnityEngine.UI;

public class BuffVisual : MonoBehaviour
{
    public GameObject arrow;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetPlaying(bool playing)
    {
        animator.SetBool("Playing", playing);
    }
}
