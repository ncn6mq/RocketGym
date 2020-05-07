using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Unity.MLAgents;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;
    private int orangeScore;
    private int blueScore;

    private Vector3 ballStartPos;
    private Vector3 blueTeam1StartPos;


    public Text orangeText;
    public Text blueText;
    public float impulseMultiplier;

    //[Tooltip("Agent1 on the orange team")]
    //public Car orangeTeam1;

    [Tooltip("Agent1 on the blue team")]
    public CarAgent blueTeam1;

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 6.0F; //maximum radians per second (we know this is correct)

        orangeScore = 0;
        blueScore = 0;
        orangeText.text = "Score: " + orangeScore.ToString();
        blueText.text = "Score: " + blueScore.ToString();

        ballStartPos = transform.position;
        blueTeam1StartPos = blueTeam1.transform.position;

    }


    void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("OrangeGoal")){
            blueScore++;
            blueText.text = "Score: " + blueScore.ToString();
            ResetScene();
        }

        if (other.gameObject.CompareTag("BlueGoal")){
            orangeScore++;
            orangeText.text = "Score: " + orangeScore.ToString();
            ResetScene();
        }

    }

    void ResetScene(){
        rb.angularVelocity = new Vector3(0, 0 , 0);
        rb.velocity = new Vector3(0, 0 , 0);
        transform.position = ballStartPos;
        blueTeam1.transform.position = blueTeam1StartPos;
    }

    /*
    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Car")){
            rb.AddForce(collision.impulse * -1000);
        }
    }
    */
}
