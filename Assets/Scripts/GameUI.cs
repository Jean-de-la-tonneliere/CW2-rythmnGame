using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] PlayerController Player;
    [SerializeField] private GameObject targetEnd;  //add target in inspector 
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject endUI;
    [SerializeField] public GameObject player;
    [SerializeField] private TextMeshProUGUI livesCountInGame;
    [SerializeField] private TextMeshProUGUI livesCountEndLevel;


    private void Start()
    {
        deathUI.SetActive(false);
        endUI.SetActive(false);
        livesCountInGame.text = ":X" + (Player.getLivesCount() - 1);
    }

    void Update()
    {

        UpdatelivesCountInGame();
        if (Player.getLivesCount() == 0)
        {
            //Debug.Log("Go back to menu");
            deathUI.SetActive(true);
            player.SetActive(false);
        }

    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene(0); //Change scene as set in the build
    }

    public void GoToNextLevel()
    {
        if((SceneManager.GetActiveScene().buildIndex + 1) <= SceneManager.sceneCount)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void UpdatelivesCountInGame()
    {
        livesCountInGame.text = ":X" + (Player.getLivesCount() - 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        livesCountEndLevel.text = livesCountInGame.text;
        if (other.CompareTag("Player"))     //Set tag and collider in scene
        {
            endUI.SetActive(true);
            player.SetActive(false);
        }
    }
}
