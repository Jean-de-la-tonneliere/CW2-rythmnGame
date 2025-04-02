using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BPMPulse : MonoBehaviour
{
    public float bpm = 120f; 
    public Transform target; 
    public Color baseColor = Color.white; // Default color
    public Color pulseColor = Color.red;  // Color on beat

    private float beatInterval;
    private Vector3 originalScale;
    private Image img; 

    void Start()
    {
        beatInterval = 60f / bpm; // Time between beats
        originalScale = target.localScale;

        img = target.GetComponent<Image>(); // Get Image if available

        StartCoroutine(PulseEffect());
    }

    IEnumerator PulseEffect()
    {
        while (true)
        {
            // Scale Up
            float elapsedTime = 0f;
            while (elapsedTime < beatInterval / 2)
            {
                target.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, elapsedTime / (beatInterval / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Scale Down
            elapsedTime = 0f;
            while (elapsedTime < beatInterval / 2)
            {
                target.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, elapsedTime / (beatInterval / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}