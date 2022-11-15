using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public BallRoller model;


    public void BallInTarget()
    {
        model.Reward();
    }

    public void ResetModel()
    {
        model.FailReset();
    }

    public void PlayerGrabBall()
    {
        model.BallGrabReset();
    }
}
