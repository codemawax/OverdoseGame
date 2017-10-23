using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public GameObject[] hazards;
    public Vector3 spawnValues;
    public float startWait = 2.0F;
    public float spawnWait = 2.0F;
    public float waveWait = 5.0F;
    public int startLives = 3;
    public float median = 2.5F;
    private int lives = 3;
    private Level currentLevel;
    private AudioSource audioSource;
    private Hazard nextHazard;
    private int hazardNumber = 0;
    private bool isPlaying = false;
    private bool gameStarted = false;
    private bool immunity = false;
    private float songTimer = 0.0F;
    private float beatIndex = 0;
    public float immunityTime = 2.0F;
    private int levelIndex;
    private bool coroutineRunning = false;

    public Light whiteSpot;
    public Light redSpot;

    public Text livesText;
    public Text beatCounterText;

    private int differentHazardNb;

    private GameObject playerInstance;
    private Material playerMaterial;

    public GameObject prefabButton;
    public RectTransform parentPanel;
    public RectTransform endMenuPanel;

    // Data struct for a level
    public struct Level
    {
        public int id;
        public string name;
        public string audioFile;
        public float bpm;
        public float offset;
        public ArrayList levelArray;
    };

    // Data struct for Hazards
    public struct Hazard
    {
        public float bar;
        public ArrayList hazardObjects;
    };

    // Data struct for each objects in a Hazard
    public struct HazardObject
    {
        public string objectName;
        public string spawnSide;
        public float spawnPosition;
        public float speed;
        public float rotation;
        public string color;
    };


    void Start ()
    {
       
        // Create level buttons dynamically
        int levelCount = LevelCount();
        for (int i = 0; i < levelCount; i++)
        {
            AddMenuButton(i);
        }

       
    }

    // Counts how many ".xml" files there are in Resources directory
    public static int LevelCount()
    {
        //TODO dynamic count with WebGL
        //DirectoryInfo d = new DirectoryInfo("Assets/Resources");
        //FileInfo[] fis = d.GetFiles("*.xml");
        return 2;
    }

    // Adds button to menu panel
    void AddMenuButton(int i)
    {
        GameObject goButton = (GameObject)Instantiate(prefabButton);
        goButton.transform.SetParent(parentPanel, false);
        goButton.transform.localScale = new Vector3(1, 1, 1);

        Button tempButton = goButton.GetComponent<Button>();
        int tempInt = i + 1;
        Text tempText = tempButton.GetComponentInChildren<Text>();
        tempText.text = "Level " + tempInt;
        tempButton.onClick.AddListener(() => StartLevelButton(tempInt));
    }


    void StartLevelButton(int buttonIndex)
    {
        levelIndex = buttonIndex;
        StartCoroutine(DragMenu(parentPanel, new Vector3(0, -700, 0), 2.0F));
        StartLevel();
    }


    // Start level by loading a level, starting music and transform the cursor
    void StartLevel ()
    {
        playerInstance = Instantiate(Resources.Load("Player", typeof(GameObject))) as GameObject;
        playerMaterial = playerInstance.GetComponent<MeshRenderer>().material;

        lives = startLives;
        livesText.text = lives.ToString();
        beatIndex = 0;
        hazardNumber = 0;
        LoadLevel(levelIndex);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = (AudioClip)Resources.Load("Audio/" + currentLevel.audioFile);
        audioSource.Play();
        isPlaying = true;
        gameStarted = true;
    }

    // Retry level button
    public void RestartLevel()
    {
        StartCoroutine(DragMenu(endMenuPanel, new Vector3(0, 700, 0), 2.0F));
        StartLevel();
    }

    // Back to menu button
    public void BackToMenu()
    {
        // TODO Coroutines problem
        StartCoroutine(DragMenu(endMenuPanel, new Vector3(0, 700, 0), 2.0F));
        StartCoroutine(DragMenu(parentPanel, new Vector3(0, 0, 0), 2.0F));
    }




    // Move the menu away
    IEnumerator DragMenu(RectTransform panel, Vector3 targetPosition, float dragTime)
    {
        while (coroutineRunning)
        {
            yield return new WaitForSeconds(0.1f);
        }
        coroutineRunning = true;
        float elapsedTime = 0;
        while (elapsedTime < dragTime)
        {
            panel.localPosition = Vector3.Lerp(panel.localPosition, targetPosition, (elapsedTime / dragTime));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        coroutineRunning = false;
    }



    // Load level data into Level struct
    void LoadLevel(int levelId)
    {
        XmlParser.GetLevel(levelId, ref currentLevel);
        nextHazard = (Hazard)currentLevel.levelArray[0];
    }

    // Spawn all objects for specified Hazard
    void SpawnHazard(Hazard hazard)
    {
        for (int objectIndex = 0; objectIndex < hazard.hazardObjects.Count; ++objectIndex)
        {
            Vector3 spawnPosition = new Vector3();
            Vector3 spawnRotation = new Vector3();

            HazardObject tempHazardObject = (HazardObject)hazard.hazardObjects[objectIndex];

            // Gets origin side and adjusts spawn rotation
            switch (tempHazardObject.spawnSide)
            {
                case "top":
                    spawnRotation = new Vector3(0,0,180);
                    spawnPosition = new Vector3(tempHazardObject.spawnPosition, 6, 0);
                    break;

                case "right":
                    spawnRotation = new Vector3(0, 0, 90);
                    spawnPosition = new Vector3(6, tempHazardObject.spawnPosition, 0);
                    break;

                case "bottom":
                    spawnRotation = new Vector3(0, 0, 0);
                    spawnPosition = new Vector3(tempHazardObject.spawnPosition, -6, 0);
                    break;

                case "left":
                    spawnRotation = new Vector3(0, 0, 270);
                    spawnPosition = new Vector3(-6, tempHazardObject.spawnPosition, 0);
                    break;

                default:
                    Debug.Log("Wrong side : " + tempHazardObject.spawnSide);
                    break;
            }
            Quaternion spawnQuaternion = Quaternion.Euler(spawnRotation);

            // Instantiate object at right position and rotation
            GameObject instance = Instantiate(Resources.Load(tempHazardObject.objectName, typeof(GameObject)), spawnPosition, spawnQuaternion) as GameObject;

            if (tempHazardObject.color != null)
            {
                Color newColor;
                ColorUtility.TryParseHtmlString("#"+tempHazardObject.color, out newColor);
                instance.GetComponent<MeshRenderer>().material.SetColor("_Color", newColor);
            }

            if (tempHazardObject.speed > 0)
            {
                instance.GetComponent<SimpleMove>().speed = tempHazardObject.speed;
            }

            if ( (tempHazardObject.rotation > 0) && (instance.GetComponent<SimpleRotator>() != null) )
            {
                instance.GetComponent<SimpleRotator>().speed.z = tempHazardObject.rotation;
            }


        }
    }
    

    void Update ()
    {
        if (gameStarted)
        {
            // Get song timer
            songTimer = audioSource.time;

            // If song timer on a beat, call Beat()
            if (isPlaying && (songTimer - (currentLevel.offset / 1000.0) > (beatIndex * 60.0F / currentLevel.bpm)))
            {
                beatIndex  += 0.25F;
                StartCoroutine("Beat");
                if (beatIndex % 1 == 0)
                {
                    StartCoroutine("ChangeSpot");
                    //StartCoroutine("CameraEffect");
                    beatCounterText.text = beatIndex.ToString();
                }
            }
    
            // If end of song, end game
            else if (!isPlaying && !audioSource.isPlaying)
            {
 
            }
        }
    }


    // Called on a beat
    IEnumerator Beat()
    {
        // If no more hazards to spawn, end beat detector
        if (hazardNumber > currentLevel.levelArray.Count - 2)
        {
            Debug.Log("End");
            isPlaying = false;
            BackToMenu();
        }
        // Else, spawn Hazard and increment Hazard index
        else if (nextHazard.bar <= beatIndex)
        {
            SpawnHazard(nextHazard);
            ++hazardNumber;
            nextHazard = (Hazard)currentLevel.levelArray[hazardNumber];
        }
        yield return null;
    }
    
    // Spotlights effect
    IEnumerator ChangeSpot()
    {
        if (redSpot.isActiveAndEnabled)
        {
            redSpot.enabled = false;
            whiteSpot.enabled = true;
        }
        else
        {
            redSpot.enabled = true;
            whiteSpot.enabled = false;
        }
        
        yield return null;
    }


    // TODO Camera effect on beat
    IEnumerator CameraEffect()
    {
       
        yield return null;
    }


   // When a player is hurt and gets immunity
    IEnumerator Hurt()
    {
        Color playerColor = playerMaterial.GetColor("_Color");
        Color tempColor = new Color(playerColor.r, playerColor.g, playerColor.b, 0F);
        playerMaterial.SetColor("_Color", tempColor);
   
        yield return new WaitForSeconds(immunityTime);

        tempColor = new Color(playerColor.r, playerColor.g, playerColor.b, 1F);
        playerMaterial.SetColor("_Color", tempColor);

        immunity = false;
    }
    

    // Remove life and detect Death case
    public void RemoveLife()
    {
        if (!immunity)
        {
            --lives;

            if (lives < 1)
            {
                Death();
            }

            livesText.text = lives.ToString();
            immunity = true;

            StartCoroutine("Hurt");

        }

    }


    // TODO Loss screen, retry/go to menu option
    void Death()
    {
        isPlaying = false;
        audioSource.Stop();
       
        lives = startLives;
        livesText.text = lives.ToString();
        Destroy(playerInstance);
        Cursor.visible = true;
        StartCoroutine(DragMenu(endMenuPanel, new Vector3(0, 0, 0), 2.0F));
    }


}
