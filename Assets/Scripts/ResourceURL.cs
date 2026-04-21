using UnityEngine;
using TMPro;

public class ResourceURL : MonoBehaviour
{
    public TMP_Text label; 

    public string buttonTxt;
    public string link;

    void Start()
    {
        if (label != null)
        {
            label.text = buttonTxt;
        }
    }

    public void OpenLink()
    {
        if (!string.IsNullOrEmpty(link))
        {
            Application.OpenURL(link);
        }
    }
}