using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] FixedJoystick joystick;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float trackDistance = 1.5f;

    [Header("Selection")]
    public bool isSelected = false;

    // Visuals
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Movement
    public bool isMoving = false;
    private Player activePlayer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        activePlayer = GetComponentInParent<Player>();
    }

    void Update()
    {
        if (!isSelected)
        {
            TrackActivePlayer();
            animator.SetBool("isMoving", isMoving);
            return;
        }

        isMoving = joystick.isDragging;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            spriteRenderer.flipX = joystick.Direction.x > 0 ? true : false;

            transform.position += joystick.GetDirAsVec3() * speed * Time.deltaTime; 
        }
    }

    protected virtual void TrackActivePlayer() 
    {
        Vector3 newDir = Vector3.zero;
        Vector3 joystickDir = new Vector3(joystick.Direction.x, joystick.Direction.y, 0.0f);
        Vector3 trackPos = activePlayer.GetActiveCharacter().transform.position + (joystick.GetDirAsVec3());

        newDir = trackPos - this.transform.position;
        newDir.Normalize();

        spriteRenderer.flipX = newDir.x > 0 ? true : false;

        if (Vector2.Distance(trackPos, this.transform.position) >= trackDistance)
        {
            transform.position += newDir * speed * Time.deltaTime;
            isMoving = true;
        } 
        else if (!joystick.isDragging)
        {
            isMoving = false;
            spriteRenderer.flipX = activePlayer.GetActiveIsFlipped();
        }
    }

    public bool GetFlipped()
    {
        return spriteRenderer.flipX;
    }
}
