using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    private Conductor conductor;
    private BeatSynchronizer beatSync;

    // Spike movement configuration
    public float moveDistance = 1.5f;
    public float moveSpeed = 5f;
    public int beatsTrigger = 2;
    public float retractDelay = 1f;

    public GameObject warningIndicator;
    public AudioClip emergingSound;
    public AudioClip retractingSound;

    private AudioSource audioSource;
    private Vector3 downPosition;
    private Vector3 upPosition;

    private enum SpikeState { Down, Emerging, Up, Retracting }
    private SpikeState currentState = SpikeState.Down;
    private int beatCounter = 0;
    private float retractTimer = 0f;

    void Start()
    {
        conductor = Conductor.instance;

        beatSync = gameObject.AddComponent<BeatSynchronizer>();
        beatSync.beatInterval = 1; // Trigger on every beat
        beatSync.OnBeatAction += OnBeat;

        // Set up audio source if needed
        if (GetComponent<AudioSource>() == null && (emergingSound != null || retractingSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Store initial position as down position
        downPosition = transform.position;

        // Calculate up position
        upPosition = downPosition + Vector3.up * moveDistance;

        // Initialize warning indicator if it exists
        if (warningIndicator != null)
        {
            warningIndicator.SetActive(false);
        }
    }

    void Update()
    {
        // Handle spike movement based on current state
        switch (currentState)
        {
            case SpikeState.Emerging:
                // Move spike upward
                transform.position = Vector3.MoveTowards(transform.position, upPosition, moveSpeed * Time.deltaTime);

                // Check if spike has reached the up position
                if (Vector3.Distance(transform.position, upPosition) < 0.01f)
                {
                    currentState = SpikeState.Up;
                    retractTimer = retractDelay;
                }
                break;

            case SpikeState.Up:
                // Wait for retract delay
                retractTimer -= Time.deltaTime;
                if (retractTimer <= 0)
                {
                    // Start retracting
                    currentState = SpikeState.Retracting;

                    // Play retracting sound if available
                    if (retractingSound != null && audioSource != null)
                    {
                        audioSource.clip = retractingSound;
                        audioSource.Play();
                    }
                }
                break;

            case SpikeState.Retracting:
                transform.position = Vector3.MoveTowards(transform.position, downPosition, moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, downPosition) < 0.01f)
                {
                    currentState = SpikeState.Down;
                    beatCounter = 0;
                }
                break;
        }
    }

    private void OnBeat()
    {
        if (currentState == SpikeState.Down)
        {
            beatCounter++;

            if (beatCounter == 1 && warningIndicator != null)
            {
                warningIndicator.SetActive(true);
            }

            if (beatCounter >= beatsTrigger)
            {
                TriggerSpike();
            }
        }
    }

    private void TriggerSpike()
    {
        // Hide warning indicator
        if (warningIndicator != null)
        {
            warningIndicator.SetActive(false);
        }

        // Change state to emerging
        currentState = SpikeState.Emerging;

        // Play emerging sound if available
        if (emergingSound != null && audioSource != null)
        {
            audioSource.clip = emergingSound;
            audioSource.Play();
        }
    }

    // Add a collider trigger for player damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.CompareTag("Player"))
        {
            // PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            //  if (playerHealth != null)
            //{
            // Apply damage to player
            //      playerHealth.TakeDamage(1);
            //}
        }
    }

    public void ResetSpike()
    {
        currentState = SpikeState.Down;
        transform.position = downPosition;
        beatCounter = 0;

        if (warningIndicator != null)
        {
            warningIndicator.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the up position
        Gizmos.color = Color.red;
        Vector3 upPos = Application.isPlaying ? upPosition : transform.position + Vector3.up * moveDistance;
        Gizmos.DrawWireSphere(upPos, 0.2f);

        // Draw the down position
        Gizmos.color = Color.green;
        Vector3 downPos = Application.isPlaying ? downPosition : transform.position;
        Gizmos.DrawWireSphere(downPos, 0.2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(downPos, upPos);
    }
}