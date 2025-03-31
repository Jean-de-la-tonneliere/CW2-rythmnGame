using UnityEngine;
using System;

public class BeatSynchronizer : MonoBehaviour
{
    // Reference to the conductor
    private Conductor conductor;

    // The last beat this object has processed
    private int lastBeat = -1;

    // Reference point for timing
    private float lastHitPosition = 0f;

    // Track which beat number to act on (every beat, every 2nd beat, etc.)
    public int beatInterval = 1;

    // Whether to act on the first beat or not
    public bool actOnFirstBeat = true;

    // Event to subscribe to for beat actions
    public event Action OnBeatAction;

    private void Start()
    {
        // Get reference to conductor
        conductor = Conductor.instance;

        // Subscribe to conductor's beat event
        conductor.OnBeat += CheckBeat;
    }

    private void OnDestroy()
    {
        // Unsubscribe when object is destroyed
        if (conductor != null)
        {
            conductor.OnBeat -= CheckBeat;
        }
    }

    private void CheckBeat()
    {
        // If this is a new beat we haven't processed yet
        if (conductor.currentBeat > lastBeat)
        {
            lastBeat = conductor.currentBeat;

            // Check if we should act on this beat based on the interval
            if (actOnFirstBeat || conductor.currentBeat % beatInterval == 0)
            {
                lastHitPosition = lastBeat * conductor.crotchet;

                // Invoke the beat action event
                OnBeatAction?.Invoke();
            }
        }
    }

    public float GetBeatProgress()
    {
        float nextBeatPosition = lastHitPosition + (conductor.crotchet * beatInterval);
        return (conductor.songPosition - lastHitPosition) / (nextBeatPosition - lastHitPosition);
    }
}