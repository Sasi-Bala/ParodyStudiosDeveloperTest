using Unity.VisualScripting;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    
    PlayerManager _playerManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerManager=FindObjectOfType<PlayerManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        _playerManager.IncreaseScore();
        Destroy(gameObject);
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
