using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public float timer;
    public TMP_Text  timertext;
    private float TotalTime=120f;
    private int score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


  public  void IncreaseScore()
  {
      score++;
        if (score >= 5)
        {
            SceneManager.LoadScene(1);
        }
    }

    void PlayerDead()
    {
        SceneManager.LoadScene(3);
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float remainingTime = TotalTime - timer;
       remainingTime= Mathf.Floor(remainingTime);
        
        if(timertext!=null)
        timertext.SetText(remainingTime.ToString());
        if (timer >= 120f)
        {
            PlayerDead();
        }
        
    }
}
