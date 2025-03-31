using UnityEngine;

public class TrebleClefsBehaviour : MonoBehaviour
{
    [SerializeField] Transform Player;

    public float jumpForce = 2f;
    public float attackDelay = 3f; // Jump every 3 beats

    private Rigidbody2D rb;
    private float attackTiming;
    private bool isAttacking = false;
    private bool hasCooldown = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackTiming = Time.time + attackDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= attackTiming && !isAttacking)
        {
            isAttacking = true; 
            AttackPlayer();     
        }

        // Handle cooldown after the attack
        if (isAttacking && Time.time >= attackTiming)
        {
            isAttacking = false; 
        }
    }

    private void AttackPlayer()
    {
        if (Player == null) return;
        Vector2 direction = (Player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * jumpForce * 0.5f, jumpForce);
                
        attackTiming = Time.time + attackDelay;
    }
}
