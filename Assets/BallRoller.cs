using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;


public class BallRoller : Agent
{
    Rigidbody2D rBody;


    public Transform Target;

    public Transform Player;
    public Transform HandAnagle;
    public Transform ArmAnagle;
    public SpriteRenderer WallSprite;
    public TextMeshPro text;

    public const float forceMultiplierMin = 5;
    public const float forceMultiplierMax = 9f;

    public float moveForceMultiplier = 10;

    [SerializeField]
    private Vector3 orgPos;
    private Quaternion armRoateion;
    private Color groundColor;
    private Vector3 playerStartPos;
    private float forceMultiplier = 0;

    int ballCatchCount = 0;

    float score = 0f;
    string scoreText = "score:";

    void Start()
    {
        Application.runInBackground = true;
        rBody = GetComponent<Rigidbody2D>();
        orgPos = this.transform.localPosition;
        playerStartPos = Player.transform.localPosition;

        armRoateion = ArmAnagle.transform.localRotation;

        groundColor = WallSprite.color;
    }

    private void FixedUpdate()
    {
        text.SetText(scoreText + score);
    }

    public override void OnEpisodeBegin()
    {
        Reset();
    }

    private void Reset()
    {
        //reset ball pos
        this.rBody.angularVelocity = 0;
        this.rBody.velocity = Vector3.zero;
        this.transform.position = HandAnagle.transform.position;

        //shoot ball
        forceMultiplier = Random.Range(forceMultiplierMin, forceMultiplierMax);

        Vector3 dir = (HandAnagle.transform.position - ArmAnagle.transform.position).normalized;
        rBody.velocity = dir * forceMultiplier;
    }

    public void FailReset()
    {
        //when player or ball is out of range
        //reset to starting pos
        Player.transform.localPosition = playerStartPos;
        //player arm reset
        ArmAnagle.localRotation = armRoateion;

        WallSprite.color = Color.red;

        ballCatchCount = 0;
        score = 0;

        SetReward(-1);
        EndEpisode();
    }

    public void BallGrabReset()
    {
        //grabBall shoot again
        Reset();

        ballCatchCount += 1;
        if(ballCatchCount > 1)
        {

            score += 0.05f;
            AddReward(0.05f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("hoop"))
        {
            Debug.Log("hit hoop");
            score += 0.1f;
            AddReward(0.1f);
        }
        else if (collision.transform.CompareTag("Wall"))
        {
            //reset to 0
            Debug.Log("hit wall");
            //score = 0;
            //SetReward(0);
            //EndEpisode();
        }
    }

    //When player shoot the ball to hole
    public void Reward()
    {
        WallSprite.color = Color.green;

        AddReward(1.0f);

        //reset
        score = 0;
        EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target, Agent and Player positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(Player.transform.localPosition.x);

        //Player arms angle
        sensor.AddObservation(ArmAnagle.transform.rotation.z);

        sensor.AddObservation(forceMultiplier);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.right;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];

        //Move
        Player.GetComponent<Rigidbody2D>().MovePosition(Player.position +new Vector3(controlSignal.x * moveForceMultiplier, 0, 0));

        //Rotate arm
        ArmAnagle.transform.Rotate(0, 0, controlSignal.z * 3.5f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}

