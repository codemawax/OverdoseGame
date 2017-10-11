using UnityEngine;
using System.Collections;
using UnityEngine.UI;


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
    private float songTimer = 0.0F;
    private int beatIndex = 0;

    public Light whiteSpot;
    public Light redSpot;


    public Text livesText;
    public Text beatCounterText;

    private int differentHazardNb;


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
        lives = startLives;
        livesText.text = lives.ToString();

        LoadLevel(2);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = (AudioClip)Resources.Load("Audio/" + currentLevel.audioFile);
        audioSource.Play();
        isPlaying = true;
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
        songTimer = audioSource.time;
        if (isPlaying && (songTimer - (currentLevel.offset / 1000.0) > (beatIndex * 60.0F / currentLevel.bpm) ) )
        {
            ++beatIndex;
            StartCoroutine("Beat");
            StartCoroutine("BeatFlash");
            StartCoroutine("ChangeSpot");
            beatCounterText.text = beatIndex.ToString();
        }
    }


    IEnumerator Beat()
    {
        if (hazardNumber > currentLevel.levelArray.Count - 1)
        {
            isPlaying = true;
        }
        else if (nextHazard.bar <= beatIndex)
        {
            SpawnHazard(nextHazard);
            ++hazardNumber;
            nextHazard = (Hazard)currentLevel.levelArray[hazardNumber];
        }
        yield return null;
    }


    IEnumerator BeatFlash()
    {
      
        //TODO flash en rythme

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
    }


}
