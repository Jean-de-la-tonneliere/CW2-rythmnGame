using System;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    // Singleton pattern to access the conductor from anywhere
    public static Conductor instance;

    // Song beats per minute
    // This is determined by the song you're trying to sync up to
    public float songBpm;

    // The number of seconds for each song beat
    public float crotchet;

    // The current position in seconds of the song
    public float songPosition;

    // Time when the song started
    private double dspSongTime;

    // Audio Source attached to this GameObject
    public AudioSource musicSource;

    // Offset to account for audio file delay and initial beat position
    public float offset;

    // The most recent beat number in the song
    public int currentBeat = 0;
    public int currentMeasure = 0;

    // How many beats per measure (usually 4)
    public int beatsPerMeasure = 4;

    // Beat tolerance window (how much time before/after a beat still counts as "on beat")
    public float beatWindow = 0.1f; // in seconds

    public event Action OnBeat;
    public event Action OnMeasure;

    private void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Calculate the duration of a single beat
        crotchet = 60f / songBpm;

        // Start the song
        StartSong();
    }

    private void Update()
    {
        // Calculate the current position of the song
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - offset) * musicSource.pitch;

        // Check if we've crossed a new beat
        int lastBeat = currentBeat;
        currentBeat = Mathf.FloorToInt(songPosition / crotchet);

        // If we've moved to a new beat
        if (currentBeat > lastBeat)
        {
            OnBeat?.Invoke();

            // Check if we've moved to a new measure
            if (currentBeat % beatsPerMeasure == 0)
            {
                currentMeasure = currentBeat / beatsPerMeasure;
                OnMeasure?.Invoke();
            }
        }
    }

    public void StartSong()
    {
        // Record the time when the song starts
        dspSongTime = AudioSettings.dspTime;

        // Start the song
        musicSource.Play();
    }

    // Returns true if the current time is on a beat within the tolerance window
    public bool IsOnBeat()
    {
        float beatTime = GetClosestBeatTime();
        return Mathf.Abs(songPosition - beatTime) <= beatWindow;
    }

    // Get the closest beat time
    public float GetClosestBeatTime()
    {
        // Find the nearest beat (previous or next)
        float previousBeat = Mathf.FloorToInt(songPosition / crotchet) * crotchet;
        float nextBeat = previousBeat + crotchet;

        // Return the closest one
        return (songPosition - previousBeat < nextBeat - songPosition) ? previousBeat : nextBeat;
    }

    // Returns whether an input at the current time would be considered on-beat
    public bool CheckInputTiming(out float accuracy)
    {
        float beatTime = GetClosestBeatTime();
        accuracy = songPosition - beatTime;
        return Mathf.Abs(accuracy) <= beatWindow;
    }
}