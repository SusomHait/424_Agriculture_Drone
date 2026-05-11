using System.Collections.Generic;
using System.Collections;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.SceneManagement;

public class ImgLibManager : MonoBehaviour
{
    public Transform scrollView; 
    public GameObject ImageCardPrefab;

    public Button exportBtn;
    public Button delBtn;

    public TMP_Dropdown exportFormatDropdown;
    public TMP_Text successAlert;

    private string selectedImg; 
    private Outline selectedOutline;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadImages();

        exportBtn.interactable = false;
        delBtn.interactable = false;
        successAlert.gameObject.SetActive(false);
    }

    public void LoadImages()
    {
        // clear the list to begin with
        foreach (Transform child in scrollView)
        {
            RawImage img = child.GetComponentInChildren<RawImage>();

            if (img != null && img.texture != null)
            {
                Destroy(img.texture);
            }

            Destroy(child.gameObject);
        }

        // set up the path to the images stored by the survey
        string folder = Path.Combine(Application.persistentDataPath, "Images");

        // create the directory if it doesn't exist
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        // get an array of the images and their paths
        string[] imagePaths = Directory.GetFiles(folder, "*.png");

        // iterate through each and make an interface element
        foreach (string path in imagePaths)
        {
            // make an image lib card
            GameObject buttonObj = Instantiate(ImageCardPrefab, scrollView);

            // get the txt element and set it to the file name
            TMP_Text nameLbl = buttonObj.GetComponentInChildren<TMP_Text>();
            if (nameLbl != null)
            {
                nameLbl.text = Path.GetFileName(path);
            }

            // load the image
            byte[] imageBytes = File.ReadAllBytes(path);

            Texture2D thumbnail = new Texture2D(2, 2);
            thumbnail.LoadImage(imageBytes);

            RawImage thumbImg = buttonObj.GetComponentInChildren<RawImage>();
            if (thumbImg != null)
            {
                thumbImg.texture = thumbnail;
            }

            // get the outline and disable it
            Outline btnOutline = buttonObj.GetComponentInChildren<Outline>();
            if (btnOutline != null)
            {
                btnOutline.enabled = false;
            }

            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => SelectImage(path, buttonObj));
        }
    }

    public void SelectImage(string path, GameObject card)
    {
        // store the new path as the selected img
        selectedImg = path;

        // the btn's should be enabled since we have a target
        exportBtn.interactable = true;
        delBtn.interactable = true;

        // turn off the outline for the current button
        if (selectedOutline != null)
        {
            selectedOutline.enabled = false;
        }

        // get the outline for the new button and turn it on
        selectedOutline = card.GetComponentInChildren<Outline>();
        if (selectedOutline != null)
        {
            selectedOutline.enabled = true;
        }
    }

    public void DeleteImage()
    {
        // check if the string has a value and the file exists
        if (string.IsNullOrEmpty(selectedImg) || !File.Exists(selectedImg))
        {
            return;
        }

        // delete the file
        File.Delete(selectedImg);

        // reset the variables
        selectedImg = null;
        selectedOutline = null;
        exportBtn.interactable = false;
        delBtn.interactable = false;

        StartCoroutine(ShowAlert("Success"));
        // reload the images
        LoadImages();
    }

    public void ExportImage()
    {
        // check if the string has a value and the file exists
        if (string.IsNullOrEmpty(selectedImg) || !File.Exists(selectedImg))
        {
            return;
        }

        // load the image
        byte[] imageBytes = File.ReadAllBytes(selectedImg);

        Texture2D thumbnail = new Texture2D(2, 2);
        thumbnail.LoadImage(imageBytes);

        // grab the selected export format
        string exportFormat = exportFormatDropdown.options[exportFormatDropdown.value].text;

        // do the export
        byte[] exportBytes;
        string extension;

        
        if (exportFormat == "JPG")
        {
            exportBytes = thumbnail.EncodeToJPG(95);
            extension = ".jpg";
        } 
        else
        {
            exportBytes = thumbnail.EncodeToPNG();
            extension = ".png";
        }

        // put the export into an export folder
        string folder = Path.Combine(Application.persistentDataPath, "Exports"); 

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string fname = Path.GetFileNameWithoutExtension(selectedImg) + extension;
        string newPath = Path.Combine(folder, fname);

        File.WriteAllBytes(newPath, exportBytes);
        StartCoroutine(ShowAlert("Success"));
    }

    IEnumerator ShowAlert(string msg)
    {
        successAlert.text = msg;
        successAlert.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        successAlert.gameObject.SetActive(false);
    }

    public void goBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
