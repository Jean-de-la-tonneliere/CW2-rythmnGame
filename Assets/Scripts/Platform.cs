using UnityEngine;

public class Platform : MonoBehaviour
{
    // Reference to the beat synchronizer
    private BeatSynchronizer beatSync;

    // Platform settings
    public int activeBeatCount = 2;  // How many beats to stay active
    public int inactiveBeatCount = 2; // How many beats to stay inactive

    // Track state
    private bool isActive = true;
    private int stateCounter = 0;

    // Platform components
    private Collider2D platformCollider;
    private SpriteRenderer platformRenderer;

    // Visual feedback
    public Color activeColor = Color.white;
    public Color warningColor = Color.yellow;
    public Color inactiveColor = new Color(1, 1, 1, 0.3f); // Semi-transparent

    private void Start()
    {
        // Setup beat synchronizer
        beatSync = gameObject.AddComponent<BeatSynchronizer>();
        beatSync.OnBeatAction += OnBeat;

        // Get components
        platformCollider = GetComponent<Collider2D>();
        platformRenderer = GetComponent<SpriteRenderer>();

        // Initial state
        UpdateVisuals();
    }

    private void Update()
    {
        // For smooth transitions, pulse with the beat
        if (beatSync != null && platformRenderer != null)
        {
            // Calculate a pulse based on beat progress (grows and shrinks slightly)
            float beatProgress = beatSync.GetBeatProgress();
            float pulse = 1f + Mathf.Sin(beatProgress * Mathf.PI) * 0.1f;

            // Apply the pulse to the platform scale
            transform.localScale = new Vector3(pulse, pulse, 1f);

            // If platform is about to change state, flash as warning
            if (isActive && stateCounter >= activeBeatCount - 1)
            {
                // About to disappear - flash warning color based on beat progress
                float warningFlash = Mathf.PingPong(beatProgress * 2f, 1f);
                platformRenderer.color = Color.Lerp(activeColor, warningColor, warningFlash);
            }
        }
    }

    private void OnBeat()
    {
        stateCounter++;

        // Toggle between active and inactive states based on beat counts
        if (isActive && stateCounter >= activeBeatCount)
        {
            isActive = false;
            stateCounter = 0;
            UpdateVisuals();
        }
        else if (!isActive && stateCounter >= inactiveBeatCount)
        {
            isActive = true;
            stateCounter = 0;
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        // Enable/disable the collider
        if (platformCollider != null)
        {
            platformCollider.enabled = isActive;
        }

        // Update the visuals
        if (platformRenderer != null)
        {
            platformRenderer.color = isActive ? activeColor : inactiveColor;
        }
    }
}