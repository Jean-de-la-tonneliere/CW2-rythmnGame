using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    [SerializeField] PlayerController Player;
    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); 
    }
}
