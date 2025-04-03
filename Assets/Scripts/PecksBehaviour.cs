using UnityEngine;

public class PecksBehaviour : MonoBehaviour
{
    Transform Player;

    public float offsetX = 0f, offsetY = 0f, knockbackSpeedX, knockbackSpeedY, knockbackTorque, boxOffsetX = 0f, boxOffsetY = 0f;
    public float spawnCooldown = 10f;
    public float attackSpeed = 10f;

    private float attackDelay = 3f; // every 3 beats
    private float spawnTiming = 0f;
    private float startAttack = 0f;

    private bool isAttacking = false;
    private bool isBlocked = false;
    private bool hasSpawned = false;
    private bool isGrounded = false;


    [SerializeField]
    private Vector2 groundDetectionBoxSize;
    private Vector2 groundDetectionBoxPos;

    [SerializeField]
    private GameObject aliveGO, deadGO;
    private Rigidbody2D rbDead;
    private Rigidbody2D rb;
    private BeatSynchronizer beat;

    public LayerMask whatIsGround;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //spawnTiming = Time.time + spawnCooldown;
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        //rbDead = deadGO.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        beat = GetComponent<BeatSynchronizer>();
        beat.OnBeatAction += OnBeat;


        //aliveGO.SetActive(false);
        //deadGO.SetActive(false);

        //groundDetectionBoxPos = new Vector2(deadGO.transform.position.x + boxOffsetX, deadGO.transform.position.y + boxOffsetY);
    }

    private float smallChaseTimer = 0f;
    int beats = 0;

    void OnBeat()
    {
        if (Vector3.Distance(transform.position, Player.position) < 14f)
        {
            if (Player.transform.position.x < transform.position.x)
            {
                rb.AddForce(new Vector2(-3f, 7.5f), ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(new Vector2(3f, 7.5f), ForceMode2D.Impulse);
            }
            beats++;
            if (beats % 4 == 0)
            {
                smallChaseTimer = 0.15f;
            }
        }
        else
        {
            rb.AddForce(new Vector2(0f, 7.5f), ForceMode2D.Impulse);
        }
        
    }

    // Update is called once per frame
    void Update()
    {



        //if (!hasSpawned && Time.time >= spawnTiming)
        //    InitialisePeck();

        //if (hasSpawned)
        //{
        //    // Debug.Log("Time : " + Time.time + " ")
        //    if (!isAttacking)
        //        HoverOverPlayer();

        //    if (Time.time >= startAttack && !isAttacking)
        //        isAttacking = true;

        //    if (isAttacking && !isBlocked)
        //        AttackPlayer();

        //    if (isGrounded)
        //    {
        //        aliveGO.SetActive(false);
        //        deadGO.SetActive(false);

        //        hasSpawned = false;
        //        isAttacking = false;
        //        spawnTiming = Time.time + spawnCooldown;

        //        transform.position = new Vector3(0, 0, 0);
        //        aliveGO.transform.position = new Vector3(0, 0, 0);
        //        deadGO.transform.position = new Vector3(0, 0, 0);

        //        isGrounded = false;
        //        isBlocked = false;
        //    }
        //}
        if (smallChaseTimer > 0f)
        {
            smallChaseTimer -= Time.deltaTime;
            AttackPlayer();
        }
    }
    private void FixedUpdate()
    {
        //groundDetectionBoxPos = new Vector2(deadGO.transform.position.x + boxOffsetX, deadGO.transform.position.y + boxOffsetY);

        //if (isBlocked && !isGrounded)
        //    isGrounded = Physics2D.OverlapBox(groundDetectionBoxPos, groundDetectionBoxSize, 0f, whatIsGround);
        //else
        //    isGrounded = false;
    }

    public bool IsChasing()
    {
        return smallChaseTimer > 0f;
    }

    private void InitialisePeck()
    {
        aliveGO.SetActive(true);
        hasSpawned = true;
        isAttacking = false;
        startAttack = Time.time + attackDelay;
    }

    private void HoverOverPlayer()
    {
        if (Player == null || isAttacking) return;
        transform.position = new Vector3(Player.position.x + offsetX, Player.position.y + offsetY, Player.position.z);
    }

    private void Blocked()
    {
        aliveGO.SetActive(false);
        deadGO.SetActive(true);

        deadGO.transform.position = aliveGO.transform.position;

        rbDead.linearVelocity = new Vector2(knockbackSpeedX, knockbackSpeedY);
        rbDead.AddTorque(knockbackTorque, ForceMode2D.Impulse);
        isBlocked = true;
    }

    private void AttackPlayer()
    {
        //Debug.Log("Is attacking");
        if (Player == null) return;
        transform.position = Vector3.MoveTowards(transform.position, Player.position, attackSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, Player.position) <= 0f)
        {
            aliveGO.SetActive(false);
            hasSpawned = false;
            isAttacking = false;
            spawnTiming = Time.time + spawnCooldown;
            //Destroy(gameObject);
        }
    }
/*    private void OnDrawGizmos()
    {
        groundDetectionBoxPos = new Vector2(deadGO.transform.position.x + boxOffsetX, deadGO.transform.position.y + boxOffsetY);
        Gizmos.DrawWireCube(groundDetectionBoxPos, groundDetectionBoxSize);
    }*/
}
