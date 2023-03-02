using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class ScreenRecorder : MonoBehaviour
{
    // 4k = 3840 x 2160   1080p = 1920 x 1080
    public int captureWidth;
    public int captureHeight;

    // configure with raw, jpg, png, or ppm (simple raw format)
    public enum Format { RAW, JPG, PNG, PPM };
    public Format format = Format.PPM;

    public bool optimizeForManyScreenshots = true;
   // private int counter = 0; // image #

    // folder to write output (defaults to data path)
    public string folder;

    // private vars for screenshot
    private Rect rect;
    private RenderTexture renderTexture;

    private Texture2D screenShot;
    private Texture2D newTex;
    // commands



    //public GameObject
    // public GameObject testpanel;
    public GameObject SaveGuiPanel;
    public GameObject LoadGuiPanel;
    //
    public GameObject ButtonLeft;
    public GameObject ButtonRight;

    public int _panelIndex;

    public GameObject CurrentPanel;
    public Sprite _defaultImage;

    Sprite[] LoadedSprites = new Sprite[4];
    Texture2D[] textures = new Texture2D[4];

    private Camera cam;

    public void Start()
    {    
        _panelIndex = 0;
        CurrentPanel = SaveGuiPanel;
        rect = new Rect(0, 0, captureWidth, captureHeight);
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        FormFolder();
    }

    public void Load()
    {
        for (int i = 0; i < 4; i++) //4 - number of saves on page
        {
            int index = 4 * _panelIndex + i; //4 - number of saves on page

            string name = string.Format("{0}/screen_{1}x{2}_{3}.{4}", folder, (int)rect.width, (int)rect.height, index, format.ToString().ToLower());

            if (File.Exists(name))
            {
                byte[] fileData = File.ReadAllBytes(name);
                textures[i] = new Texture2D(2, 2);
                textures[i].LoadImage(fileData);
                textures[i].name = "loadedtex" + i;

                Sprite sprite = Sprite.Create(textures[i], rect, new Vector2(0.5f, 0.5f), 100.0f);
                sprite.name = "sprite" + i;

                LoadedSprites[i] = sprite;
            }
            else 
            {
                LoadedSprites[i] = _defaultImage;
                //textures[i] = _defaultImage.texture;
            }
        }
    }

    public void Unload()
    {
        for (int i = 0; i < 4; i++) //4 - number of saves on page
        {
            Destroy(textures[i]);
        }

        renderTexture.Release();
        renderTexture = null;
        Destroy(screenShot);
        
        Resources.UnloadUnusedAssets();
    }
    
    public void SetCurrentPanel(string panel)
    {
        _panelIndex = 0;
        switch (panel)
        {
            case "SaveGuiPanel":
                {
                    ButtonLeft = SaveGuiPanel.transform.Find("Left").gameObject;
                    ButtonRight = SaveGuiPanel.transform.Find("Right").gameObject;
                    CurrentPanel = SaveGuiPanel;
                    break;
                }
            case "LoadGuiPanel":
                {
                    ButtonLeft = LoadGuiPanel.transform.Find("Left").gameObject;
                    ButtonRight = LoadGuiPanel.transform.Find("Right").gameObject;
                    CurrentPanel = LoadGuiPanel;
                    break;
                }
            default:
                {
                    ButtonLeft = SaveGuiPanel.transform.Find("Left").gameObject;
                    ButtonRight = SaveGuiPanel.transform.Find("Right").gameObject;
                    CurrentPanel = SaveGuiPanel;
                    break;
                }
        }
        ButtonLeft.SetActive(false);
    }
    public void ChangePanel(int direction)
    {
        _panelIndex += direction;

        if (_panelIndex < 0)
        {
            _panelIndex = 0;
            return;
        }

        if(_panelIndex == 0)
        {
            ButtonLeft.SetActive(false);
            UpdatePanels(true);
            return;
        }

        if(_panelIndex == 4) //pages number
        {
            ButtonRight.SetActive(false);
            UpdatePanels(true);
            return;
        }

        if (_panelIndex > 4)
        {
            _panelIndex = 4;
            return;
        }

        ButtonLeft.SetActive(true);
        ButtonRight.SetActive(true);  
        UpdatePanels(true);
    }

    public void UpdatePanels(bool load)
    {
        if(load)
            Load();

        if (folder != null)
        {
            for (int i = 0; i < 4; i++)
            {
                CurrentPanel.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = LoadedSprites[i];
            }
        }
    }




    public void FormPng(int SaveToUse)
    {
     //   int SaveToUse = GameObject.Find("Scene").GetComponent<Player>().SaveToUse;

        // get our unique filename
        string filename = uniqueFilename((int)rect.width, (int)rect.height, _panelIndex * 4 + SaveToUse);

        // pull in our file header/data bytes for the specified image format (has to be done from main thread)
        byte[] fileHeader = null;
        byte[] fileData = null;
        if (format == Format.RAW)
        {
            fileData = screenShot.GetRawTextureData();
        }
        else if (format == Format.PNG)
        {
            fileData = screenShot.EncodeToPNG();
        }
        else if (format == Format.JPG)
        {
            fileData = screenShot.EncodeToJPG();
        }
        else // ppm
        {
            // create a file header for ppm formatted file
            string headerStr = string.Format("P6\n{0} {1}\n255\n", rect.width, rect.height);
            fileHeader = System.Text.Encoding.ASCII.GetBytes(headerStr);
            fileData = screenShot.GetRawTextureData();
        }

        /////////////////////
        // byte[] fileData = File.ReadAllBytes(name);
       // Destroy(textures[SaveToUse]);
        CurrentPanel.transform.GetChild(SaveToUse).gameObject.GetComponent<Image>().sprite = null;

        screenShot.name = "newScreenshot";

        //textures[SaveToUse] = screenShot;

        Sprite sprite = Sprite.Create(screenShot, rect, new Vector2(0.5f, 0.5f), 100.0f);

        LoadedSprites[SaveToUse] = sprite;

        CurrentPanel.transform.GetChild(SaveToUse).gameObject.GetComponent<Image>().sprite = sprite;






        // create new thread to save the image to file (only operation that can be done in background)
        new System.Threading.Thread(() =>
        {
            // create file and write optional header with image bytes
            var f = System.IO.File.Create(filename);
            if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
            f.Write(fileData, 0, fileData.Length);
            f.Close();
            Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, fileData.Length));
        }).Start();
    }

    

    public void CaptureScreenshot()
    {
        // create screenshot objects if needed
        if (renderTexture == null)
        {
            //Debug.Log("renderTexture == null");
            // creates off-screen render texture that can rendered into

            rect = new Rect(0, 0, captureWidth, captureHeight);

            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);  

            screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        }


        cam.targetTexture = renderTexture;
        cam.Render();

        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;
    }


    public void FormFolder()
    {
        if (folder == null || folder.Length == 0)
        {
            folder = Application.dataPath;
            if (Application.isEditor)
            {
                // put screenshots in folder above asset path so unity doesn't index the files
                var stringPath = folder + "/..";
                folder = Path.GetFullPath(stringPath);
            }
            folder += "/screenshots";

            // make sure directoroy exists
            System.IO.Directory.CreateDirectory(folder);
        }
    }

    private string uniqueFilename(int width, int height, int savetouse)
    {
        // use width, height, and counter for unique file name
        var filename = string.Format("{0}/screen_{1}x{2}_{3}.{4}", folder, width, height, savetouse, format.ToString().ToLower());

        // return unique filename
        return filename;
    }


}