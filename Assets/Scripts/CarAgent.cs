using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
//using UnityEngine.PhysicsModule;

public class CarAgent : Agent {

    private Rigidbody rb;
    private bool onGround;
    private int boost;

    public float speed;
    public float rotationSpeed;
    public Ball ball;
    public Arena arena;

    [Header("Vehicle Physics")]
    [Tooltip("The transform that determines the position of the Kart's mass.")]
    public Transform CenterOfMass;

    [Tooltip("The physical representations of the Kart's wheels.")]
    public Transform[] Wheels;

    // Initialize is called at when the environment first starts running and never again
    public override void Initialize()
    {
        base.Initialize();
        rb = GetComponent<Rigidbody>();
        //arena = GetComponentInParent<Arena>();
        onGround = true;
        boost = 100;
    }
    
    /// <summary>
    /// Perform actions based on a vector of numbers
    /// </summary>
    /// <param name="vectorAction">The list of actions to take</param>
    /// <item item=0> The forward movement of the car (can be negative) </item>
    /// <item item=1> The turning or yaw/pitch movement of the car (can be negative) </item>
    /// <item item=2> Jump  </item>
    /// <item item=3> Boost  </item>
    /// <item item=4> Drift  </item>
    /// <item item=5> Roll (can be negative)  </item>
    public override void OnActionReceived(float[] vectorAction) {

        // Convert the first action to forward movement if the vehicle is on the ground
        float forwardAmount = 0f;
        if(onGround){
            if (vectorAction[0] == 1f) {
                forwardAmount = 1f;
            }
            else if (vectorAction[0] == 2f) {
                forwardAmount = -1f;
            }
        }

        // Convert the second action to turning left or right
        float turnAmount = 0f;
        if (vectorAction[1] == 1f) {
            turnAmount = -1f;
        }
        else if (vectorAction[1] == 2f) {
            turnAmount = 1f;
        }
        
        // Jump if the vehicle is on the ground
        if (vectorAction[2] == 1f) {
            Jump();
        }

        if ((boost > 0) && vectorAction[3] == 1f){
            Boost();
        }

        // Apply movement
        //rb.MovePosition(transform.position + transform.forward * forwardAmount * speed * Time.fixedDeltaTime);
        //transform.position += transform.forward * forwardAmount * speed * Time.fixedDeltaTime);
        transform.Rotate(0, rotationSpeed*turnAmount*Time.fixedDeltaTime, 0, Space.Self);
        transform.Translate(forwardAmount*speed*Time.fixedDeltaTime, 0, 0, Space.Self);


        // Apply a tiny negative reward every step to encourage action
        if (MaxStep > 0) AddReward(-1f / MaxStep);
    }


    /// <summary>
    /// Read inputs from the keyboard and convert them to a list of actions.
    /// This is called only when the player wants to control the agent and has set
    /// Behavior Type to "Heuristic Only" in the Behavior Parameters inspector.
    /// </summary>
    /// <returns>A vectorAction array of floats that will be passed into <see cref="AgentAction(float[])"/></returns>
    public override void Heuristic(float[] actionsOut){
        float forwardAction = 0f;
        float turnAction = 0f;
        float jumpAction = 0f;
        float boostAction = 0f;

        if (Input.GetKey(KeyCode.W)){
            // move forward
            forwardAction = 1f;
        } else if (Input.GetKey(KeyCode.S)){
            forwardAction = 2f;
        }

        if (Input.GetKey(KeyCode.A)){
            // turn left
            turnAction = 1f;
        } else if (Input.GetKey(KeyCode.D)){
            // turn right
            turnAction = 2f;
        }

        if (Input.GetKey(KeyCode.Space)){
            jumpAction = 1f;
        }

        if (Input.GetKey(KeyCode.LeftShift)){
            boostAction = 1f;
        }

        // Put the actions into an array and return
        actionsOut[0] = forwardAction;
        actionsOut[1] = turnAction;
        actionsOut[2] = jumpAction;
        actionsOut[3] = boostAction;
    }


    private void Jump(){
        if(onGround){
            rb.velocity = transform.TransformDirection(Vector3.up)*500;
        }
    }

    private void Boost(){
        rb.AddForce(transform.TransformDirection(Vector3.right)*10000, ForceMode.Impulse);
        boost -= 1;
    }


    /// <summary>
    /// Reset the agent and arena
    /// </summary>
    public override void OnEpisodeBegin()
    {
        arena.ResetArena();
        boost = 100;
        onGround = true;
    }


    /// <summary>
    /// Collect all non-Raycast observations
    /// </summary>
    public override void CollectObservations(VectorSensor sensor) {

        // Distance to the ball
        sensor.AddObservation(Vector3.Distance(ball.transform.position, transform.position));

        // Direction to ball (1 Vector3 = 3 values)
        sensor.AddObservation((ball.transform.position - transform.position).normalized);

        //add direction to goal
        //add distance to goal
        //add direction to opponent
        //add distance to opponent

        // 1 + 3 = 4 total values
    }


    //Called at the beginning of every step
    private void FixedUpdate() {
        // Request a decision every 5 steps. RequestDecision() automatically calls RequestAction(),
        // but for the steps in between, we need to call it explicitly to take action using the results
        // of the previous decision

        IsOnGround();
        Debug.Log(onGround);
        if(onGround){
            rb.useGravity = false;
            rb.AddForce(transform.TransformDirection(Vector3.down)*10000, ForceMode.Impulse);
        } else {
            rb.useGravity = true;
        }
        if (StepCount % 5 == 0) {
            RequestDecision();
            boost += 1;
        }
        else {
            RequestAction();
        }
    }


    private void IsOnGround() {                       //out float minHeight) {
        //minHeight = float.MaxValue;

        int groundedCount = 0;

        for (int i = 0; i < Wheels.Length; i++)
        {
            Transform current = Wheels[i];
            RaycastHit hit;
            groundedCount += Physics.Raycast(current.position, transform.TransformDirection(Vector3.down), out hit, 50) ? 1 : 0;
            //Debug.DrawRay(current.position, transform.TransformDirection(Vector3.down)*1000, Color.red);
            //if (hit.distance > 0)
            //{
            //    minHeight = Mathf.Min(hit.distance, minHeight);
            //}
        }

        onGround = groundedCount == 4;
    }
}