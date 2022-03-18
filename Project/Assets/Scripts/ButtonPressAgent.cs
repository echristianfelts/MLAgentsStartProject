using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ButtonPressAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform avoidTransform;
    //  [SerializeField] private Transform buttonTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    public TextMesh textEpisode;
    public TextMesh textSuccess;
    public TextMesh textFailure;
    public TextMesh textReward;
    private int num_Ep = 0;
    private int num_Success = 0;
    private int num_Failure = 0;
    private int randDir;

    //  public bool hackSensorNear = false;
    //  public bool hackSensorFar = false;

    public float moveSpeed = 3f;
    public float smooth = 5f;
    public float rotMultiplier = 180f;
    private float rot = 0f;
 
    public override void OnEpisodeBegin()
    {
        randDir = Random.Range(0, 4);
        textEpisode.text = num_Ep.ToString();
        textSuccess.text = num_Success.ToString();
        textFailure.text = num_Failure.ToString();
        textReward.text = "0";
        //  rot = 0f;
        //  hackSensorNear = false;
        //  hackSensorFar = false;
        transform.localPosition = new Vector3(0F, 4f, 0f);
        //  transform.rotation = Quaternion.Euler(0f,0f,0f);
        if (randDir == 1)
        { targetTransform.localPosition = new Vector3(Random.Range(5f, -5f), 4.5f, 5f); }
        if (randDir == 2)
        { targetTransform.localPosition = new Vector3(Random.Range(5f, -5f), 4.5f, -5f); }
        if (randDir == 3)
        { targetTransform.localPosition = new Vector3(5f, 4.5f, Random.Range(5f, -5f)); }
        if (randDir == 4 || randDir == 0)
        { targetTransform.localPosition = new Vector3(-5f, 4.5f, Random.Range(5f, -5f)); }
        //  avoidTransform.localPosition = new Vector3(Random.Range(4f, -4f), 5.5f, -4f);
        rot = 0f;
        //  buttonTransform.localPosition = new Vector3(3.5f + Random.Range(-6f, 6f), 2.5f, 5f + Random.Range(0f, 5f));
    }


    // how the AI takes in or views actions...
    public override void CollectObservations(VectorSensor sensor)
    {
        //  Where is the Agent...?
        sensor.AddObservation(transform.localPosition);

        //  Where is the Target..?
        sensor.AddObservation(targetTransform.localPosition);

        //  Where is the Target..?
        sensor.AddObservation(avoidTransform.localPosition);

        //What is the Agents Rotation..?
        sensor.AddObservation(transform.rotation.y);
        //  sensor.AddObservation(hackSensorNear);
        //  sensor.AddObservation(hackSensorFar);

    }


    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveZ = actions.ContinuousActions[1];
        float rotY = actions.ContinuousActions[0];
        float reward = GetCumulativeReward();
        textReward.text = reward.ToString();
        if (moveZ > 0f && reward < .5f)
        {
            //  moveZ = moveZ * 3f;
            AddReward(0.125f);
            /*
            if (GetCumulativeReward() < 1)
            {
                AddReward(0.05f);
            }*/
        }
        if(moveZ==0)
        {
            rotY = 0;
            AddReward(-0.05f);
        }

        if (transform.localPosition.y<0)
        {
            num_Ep += 1;
            num_Failure += 1;
            SetReward(-10f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }

        if (targetTransform.localPosition.y < 0)
        {
            num_Ep += 1;
            num_Failure += 1;
            SetReward(-10f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }

        /*
        if (hackSensorFar)
        {
            AddReward(.2f);
        }

        if (hackSensorFar)
        {
            AddReward(.5f);
        }
        */
        AddReward(-.075f);

        //  Debug.Log("Speed Multiplier Val="+ moveZ);
        //  Debug.Log("Cumulitive Reward =" + GetCumulativeReward());
        //  float a= GetCumulativeReward();



        transform.localPosition += transform.TransformDirection(Vector3.forward * moveZ * moveSpeed * 0.1f);
        rot = rot + (rotY * rotMultiplier);
        transform.rotation = Quaternion.Euler(0, rot , 0);

    }

// This lets you override the ML decision making process and lets you control the agent into
// committing to the action tha twe want it to do.
//
// The Agent will "Remember" and learn from this expirience.
//
// It makes teaching complex tasks go faster.
//
// It's the "Hey, Dummy.  Do this thing...  Now next time do better" Override.

public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {

            AddReward(150f);
            floorMeshRenderer.material = winMaterial;
            num_Success += 1;
            num_Ep += 1;
            Debug.Log("Cumulitive Reward =" + GetCumulativeReward());
            //  transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            EndEpisode();
            

        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {

                SetReward(-10f);
                floorMeshRenderer.material = loseMaterial;
                num_Ep += 1;
                num_Failure += 1;
                Debug.Log("Cumulitive Reward =" + GetCumulativeReward());
                //  transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                EndEpisode();
            
        }

    }
      
}
