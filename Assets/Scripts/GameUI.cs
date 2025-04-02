using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] PlayerController Player;
    [SerializeField] private GameObject deathUI;
    [SerializeField] public GameObject player;

    private void Start()
    {
        deathUI.SetActive(false);
    }

    void Update()
    {
        
        if (Player.getLivesCount() == 0)
        {
            //Debug.Log("Go back to menu");
            deathUI.SetActive(true);
            player.SetActive(false);
        }

    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); 
    }
}
