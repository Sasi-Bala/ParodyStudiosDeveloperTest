using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button TryAgainButton;
  
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        TryAgainButton.onClick.AddListener(TryAgain);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }



    
    void Update()
    {
        
    }

    public   void TryAgain()
    {
        Debug.Log("pressed!");

        SceneManager.LoadScene(2);
    }
}
