using UnityEngine;

public class NotesBehaviour : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private LayerMask playerLayer;
    public float detectionRange = 1f;
    private float speed = 3f;

    private bool isChasing = false;

    // Update is called once per frame
    void Update()
    {
        if (!isChasing)
            FindPlayer();
        if (isChasing)
            AttackPlayer();
    }

    private void FindPlayer()
    {
        //Look left and right
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, playerLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, detectionRange, playerLayer);
        Debug.Log("Note : " + transform.position + "Player : " + Player.position);
        if (hitRight.collider != null || hitLeft.collider != null)
        {
            isChasing = true;
            Debug.Log("Player detected! Notes started chasing");
        }
    }

    private void AttackPlayer()
    {
        if (Player == null) return;

        float direction = (Player.position.x > transform.position.x) ? 1 : -1;
        transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
    }
}
