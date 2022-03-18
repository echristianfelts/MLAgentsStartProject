using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RacerAgent_001 : Agent
{
    //  [SerializeField] private Transform targetTransform;
    //  [SerializeField] private Transform buttonTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    public float moveSpeed = 3f;
    public float smooth = 5f;
    public float rotMultiplier = 100f;
    private float rot = 0f;



    public override void OnEpisodeBegin()
    {
        //  rot = 0f;
        transform.localPosition = new Vector3(3.5f, 4f, 2.5f);
        //  targetTransform.localPosition = new Vector3(3.5f + Random.Range(2.5f, -2.5f), 4f, 2.5f);
        //  buttonTransform.localPosition = new Vector3(3.5f + Random.Range(-6f, 6f), 2.5f, 5f + Random.Range(0f, 5f));
    }


    // how the AI takes in or views actions...
    public override void CollectObservations(VectorSensor sensor)
    {
        //  Where is the Agent...?
        sensor.AddObservation(transform.localPosition);

        //  Where is the Target..?
        //  sensor.AddObservation(targetTransform.localPosition);

        //What is the Agents Rotation..?
        sensor.AddObservation(transform.rotation.y);




    }


    public override void OnActionReceived(ActionBuffers actions)
    {

        float moveZ = actions.ContinuousActions[1];
        float rotY = actions.ContinuousActions[0];
        float reward = GetCumulativeReward();
        if (moveZ > 0f && reward < 10)
        {
            //  moveZ = moveZ * 3f;
            AddReward(0.2f);

            /*
            if (GetCumulativeReward() < 1)
            {
                AddReward(0.05f);
            }*/
        }
        if (moveZ == 0)
        {
            rotY = 0;
            AddReward(-0.1f);
        }

        if (transform.localPosition.y < -10)
        {
            SetReward(-10f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }


        //  Debug.Log("Speed Multiplier Val="+ moveZ);
        Debug.Log("Cumulitive Reward =" + GetCumulativeReward());
        //  float a= GetCumulativeReward();



        transform.localPosition += transform.TransformDirection(Vector3.forward * moveZ * moveSpeed * 0.1f);
        rot = rot + (rotY * rotMultiplier);
        transform.rotation = Quaternion.Euler(0, rot, 0);


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
            AddReward(20f);
            floorMeshRenderer.material = winMaterial;
            Debug.Log("Cumulitive Reward =" + GetCumulativeReward());
            EndEpisode();
        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-10f);
            floorMeshRenderer.material = loseMaterial;
            Debug.Log("Cumulitive Reward =" + GetCumulativeReward());
            EndEpisode();
        }

        if (other.TryGetComponent<WayPoint>(out WayPoint waypoint))
        {
            AddReward(20f);
            floorMeshRenderer.material = loseMaterial;
            Debug.Log("Cumulitive Reward =" + GetCumulativeReward());
            //  EndEpisode();
        }

    }
}
