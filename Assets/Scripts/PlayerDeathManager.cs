using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathManager : MonoBehaviour
{
 
    void OnCollisionEnter(Collision collision)
    {
        SceneManager.LoadScene(3);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
