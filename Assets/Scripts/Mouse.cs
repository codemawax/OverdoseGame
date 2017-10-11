using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour
{

    public float distanceFromCamera = 10.0F;
    public float screenSize = 5.0F;

    private GameController gameController;

    void Start ()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }

        Cursor.visible = false;
    }

    void Update ()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCamera);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(Mathf.Clamp(mousePos.x, -screenSize, screenSize), Mathf.Clamp(mousePos.y, -screenSize, screenSize), 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hazard"))
        {
            gameController.RemoveLife();
        }
    }

 
}