using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public SettingsManager settingsManager;

    public GameObject startPanel; 
    public GameObject settingsPanel; 
    public GameObject resourcePanel; 

    public TMP_Dropdown currentSurveyChoice;

    public Button start;
    public Button edit_settings; 
    public Button resources;
    public Button image_library;

    bool startMenuOpen = false;
    bool settingsMenuOpen = false;
    bool resourcesOpen = false; 

    void Start()
    {
        startPanel.SetActive(false);
        settingsPanel.SetActive(false);
        resourcePanel.SetActive(false);

        toggle_button_interaction(true);
    }

    // control the start menu
    public void toggleStartMenu()
    {
        startMenuOpen = !startMenuOpen;
        
        startPanel.SetActive(startMenuOpen);
        toggle_button_interaction(!startMenuOpen);
    }

    // controll the settings menu
    public void toggleSettingsMenu()
    {
        settingsMenuOpen = !settingsMenuOpen;

        if (settingsMenuOpen) settingsManager.LoadSettings();

        settingsPanel.SetActive(settingsMenuOpen);
        toggle_button_interaction(!settingsMenuOpen);
    }

    // control the resource menu
    public void toggleResourceMenu()
    {
        resourcesOpen = !resourcesOpen; 

        resourcePanel.SetActive(resourcesOpen);
        toggle_button_interaction(!resourcesOpen);
    }

    // map the dropdown indicies to the scene names
    Dictionary<int, string> sceneMap = new Dictionary<int, string>()
    {
        { 0, "RowsPlot" },
        { 1, "CornCrops" },
    };

    // start the survey
    public void startSurvey()
    {
        int index = currentSurveyChoice.value;

        if (sceneMap.TryGetValue(index, out string sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void openImgLib()
    {
        SceneManager.LoadScene("ImageLibrary");
    }

    // disable the buttons when a menu is open or renable them when closed
    void toggle_button_interaction(bool state)
    {
        start.interactable = state;
        edit_settings.interactable = state;
        resources.interactable = state;
        image_library.interactable = state;
    }
}