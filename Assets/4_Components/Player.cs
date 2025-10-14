using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] FixedJoystick playerJoystick;
    [SerializeField] float speed;

    private Animator animator;
    public bool isMoving;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();

        if (playerJoystick.Direction != Vector2.zero)
        {
            isMoving = true;
        } else
        {
            isMoving = false;
        }

        animator.SetBool("isMoving", isMoving);
    }

    void MovePlayer()
    {
        Vector3 direction = new Vector3(playerJoystick.Direction.x, playerJoystick.Direction.y, 1);
        Vector3 dir = direction * speed * Time.deltaTime;
        this.gameObject.transform.Translate(dir);
    }
}
