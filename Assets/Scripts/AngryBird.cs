using UnityEngine;

public class AngryBirds : MonoBehaviour
{
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private float destroyDelayAfterHit = 2f;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private AudioSource audioSource;

    private bool hasBeenLaunched;
    private bool shouldFaceVelocityDirection;
    private bool hasCollided;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        circleCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (hasBeenLaunched && shouldFaceVelocityDirection)
        {
            transform.right = rb.linearVelocity;
        }
    }

    public void LaunchBird(Vector2 direction, float force)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        circleCollider.enabled = true;

        rb.AddForce(direction * force, ForceMode2D.Impulse);
        hasBeenLaunched = true;
        shouldFaceVelocityDirection = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasCollided) return; // tránh va chạm liên tiếp gọi lại nhiều lần
        hasCollided = true;

        shouldFaceVelocityDirection = false;
        SoundManager.instance.PlayClip(hitClip, audioSource);

        Destroy(gameObject, destroyDelayAfterHit);
    }
}