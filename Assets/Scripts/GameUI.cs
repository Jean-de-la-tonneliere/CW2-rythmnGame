using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] PlayerController Player;
    [SerializeField] private GameObject deathUI;
    [SerializeField] public GameObject player;
    [SerializeField] private TextMeshProUGUI livesCount;

    private void Start()
    {
        deathUI.SetActive(false);
        livesCount.text = ":X" + (Player.getLivesCount() - 1);
    }

    void Update()
    {

        UpdateLivesCount();
        if (Player.getLivesCount() == 0)
        {
            //Debug.Log("Go back to menu");
            deathUI.SetActive(true);
            player.SetActive(false);
        }

    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); //Change scene as set in the build
    }

    public void UpdateLivesCount()
    {
        livesCount.text = ":X" + (Player.getLivesCount() - 1);
    }
}
