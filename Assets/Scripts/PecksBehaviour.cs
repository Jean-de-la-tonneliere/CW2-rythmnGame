using UnityEngine;

public class PecksBehaviour : MonoBehaviour
{
    [SerializeField] Transform Player;

    public float spawnCooldown = 10f;
    public float hoveringSpeed = 100f;
    public float attackSpeed = 10f;

    private float attackDelay = 3f; // every 3 beats
    private float spawnTiming = 0f;
    private float startAttack = 0f;
    private bool isAttacking = false;
    private bool hasSpawned = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
        spawnTiming = Time.time + spawnCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasSpawned && Time.time >= spawnTiming)
            InitialisePeck();

        if (hasSpawned)
        {
           // Debug.Log("Time : " + Time.time + " ")
            if (!isAttacking)
                HoverOverPlayer();

            if (Time.time >= startAttack && !isAttacking)
                isAttacking = true;

            if (isAttacking)
                AttackPlayer();   
        }
    }

    private void InitialisePeck()
    {
        GetComponent<Renderer>().enabled = true;
        hasSpawned = true;
        isAttacking = false;
        startAttack = Time.time + attackDelay;
    }

    private void HoverOverPlayer()
    {
        if (Player == null || isAttacking) return;
        transform.RotateAround(Player.position, Vector3.forward, hoveringSpeed * Time.deltaTime);
    }

    private void AttackPlayer()
    {
        Debug.Log("Is attacking");
        if (Player == null) return;
        transform.position = Vector3.MoveTowards(transform.position, Player.position, attackSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, Player.position) < 0.05f) 
        {
            GetComponent<Renderer>().enabled = false;
            hasSpawned = false;
            isAttacking = false;
            spawnTiming = Time.time + spawnCooldown;
            transform.position = new Vector3(0, 0, 0);
        }
    }
}
