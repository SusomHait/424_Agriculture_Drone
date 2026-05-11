using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro; 

public class SimUI : MonoBehaviour
{
    public TMP_Text counter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // end the survey
    public void endSurvey()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        float fps = 1f / Time.unscaledDeltaTime;
        counter.text = "FPS: " + Mathf.RoundToInt(fps);
    }
}
