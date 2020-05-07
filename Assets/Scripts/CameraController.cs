using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject player;

    public float height;
    public float distance;

    private Vector3 offset;

    void Start ()
    {   
        offset = new Vector3(distance, height, 0);
    }

    void LateUpdate ()
    {
        transform.position = player.transform.position + offset;
    }

    
}
