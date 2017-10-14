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
    private float songTimer = 0.0F;
    private int beatIndex = 0;

    public Light whiteSpot;
    public Light redSpot;

    public Text livesText;
    public Text beatCounterText;

    private int differentHazardNb;

    public GameObject prefabButton;
    public RectTransform parentPanel;


    public struct Level
    {
        public int id;
        public string name;
        public string audioFile;
        public float bpm;
        public float offset;
        public ArrayList levelArray;
    };

    public struct Hazard
    {
        public float bar;
        public ArrayList hazardObjects;
    };

    public struct HazardObject
    {
        public string objectName;
        public string spawnSide;
        public float spawnPosition;
    };




    void Start ()
    {
        int levelCount = LevelCount();
        for (int i = 0; i < levelCount; i++)
        {
            AddMenuButton(i);
        }

       
    }


    public static int LevelCount()
    {
        DirectoryInfo d = new DirectoryInfo("Assets/Resources");
        FileInfo[] fis = d.GetFiles("*.xml");
        return fis.Length;
    }

    void AddMenuButton(int i)
    {
        GameObject goButton = (GameObject)Instantiate(prefabButton);
        goButton.transform.SetParent(parentPanel, false);
        goButton.transform.localScale = new Vector3(1, 1, 1);

        Button tempButton = goButton.GetComponent<Button>();
        int tempInt = i + 1;
        Text tempText = tempButton.GetComponentInChildren<Text>();
        tempText.text = "Level " + tempInt;
        tempButton.onClick.AddListener(() => StartLevel(tempInt));
    }




    void StartLevel (int levelIndex)
    {
        StartCoroutine("DragMenu");

        lives = startLives;
        livesText.text = lives.ToString();

        LoadLevel(levelIndex);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = (AudioClip)Resources.Load("Audio/" + currentLevel.audioFile);
        audioSource.Play();
        isPlaying = true;
        gameStarted = true;
    }



    IEnumerator DragMenu()
    {
        float elapsedTime = 0;
        float menuSpeed = 8.0F;
        Vector3 menuTargetPosition = new Vector3(0, -500, 0);

        while (elapsedTime < menuSpeed)
        {
            parentPanel.localPosition = Vector3.Lerp(parentPanel.localPosition, menuTargetPosition, (elapsedTime / menuSpeed));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }




    void LoadLevel(int levelId)
    {
        XmlParser.GetLevel(levelId, ref currentLevel);
        nextHazard = (Hazard)currentLevel.levelArray[0];
    }


    void SpawnHazard(Hazard hazard)
    {
        for (int objectIndex = 0; objectIndex < hazard.hazardObjects.Count; ++objectIndex)
        {
            Vector3 spawnPosition = new Vector3();
            Vector3 spawnRotation = new Vector3();

            HazardObject tempHazardObject = (HazardObject)hazard.hazardObjects[objectIndex];

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

            GameObject instance = Instantiate(Resources.Load(tempHazardObject.objectName, typeof(GameObject)), spawnPosition, spawnQuaternion) as GameObject;
        }
    }
    

    void Update ()
    {
        if (gameStarted)
        {
            songTimer = audioSource.time;
            if (isPlaying && (songTimer - (currentLevel.offset / 1000.0) > (beatIndex * 60.0F / currentLevel.bpm)))
            {
                ++beatIndex;
                StartCoroutine("Beat");
                StartCoroutine("ChangeSpot");
                beatCounterText.text = beatIndex.ToString();
            }
            else if (!isPlaying && !audioSource.isPlaying)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }


    IEnumerator Beat()
    {
        if (hazardNumber > currentLevel.levelArray.Count - 2)
        {
            Debug.Log("End");
            isPlaying = false;
        }
        else if (nextHazard.bar <= beatIndex)
        {
            SpawnHazard(nextHazard);
            ++hazardNumber;
            nextHazard = (Hazard)currentLevel.levelArray[hazardNumber];
        }
        yield return null;
    }
    

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


    IEnumerator Hurt()
    {
      
        // TODO flash red when hurt

        yield return null;
    }
    

    public void RemoveLife()
    {
        --lives;
        
        if (lives < 0)
        {
            Death();
        }

        StartCoroutine("Hurt");

        livesText.text = lives.ToString();
    }


    void Death()
    {
        lives = startLives;
        livesText.text = lives.ToString();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
