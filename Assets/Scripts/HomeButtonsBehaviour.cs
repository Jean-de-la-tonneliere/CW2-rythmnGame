using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeButtonsBehaviour : MonoBehaviour
{

    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider seSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*    void Start()
        {

        }*/

    // Update is called once per frame
    void Update()
    {
        //Load PlayerPrefs volume
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume", volumeSlider.value);

        if (Input.GetKey(KeyCode.Escape))
        {
            QuitApp();
        }

    }

    public void PlayApp()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Put Game Scene next to this one 
    }
    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application sandwiched");
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }

}
