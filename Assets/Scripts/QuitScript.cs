using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuitScript : MonoBehaviour
{
 public   Button quitButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quitButton.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
