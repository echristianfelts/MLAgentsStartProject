using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    public float moveSpeed = 3f;


    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(3.5f+Random.Range(-6f,6f),4f,7f + Random.Range(0f, 5f));
        targetTransform.localPosition = new Vector3(3.5f + Random.Range(-6f, 6f), 5.5f, 5f + Random.Range(0f, 5f));

    }


    // how the AI takes in or views actions...
    public override void CollectObservations(VectorSensor sensor)
    {
        if (sensor != null)
        {
            //  Where is the Agent...? (Vec3. Three Floats. Three Vector Observations.)
            sensor.AddObservation(transform.localPosition);
            //  Where is the Target..? (Vec3. Three Floats. Three Vector Observations.)
            sensor.AddObservation(targetTransform.localPosition);
        }
        


    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log(actions.ContinuousActions[0]);

        float moveX = actions.ContinuousActions[0];     //  Horizontal Movement
        float moveZ = actions.ContinuousActions[1];     //  Vertical Movement

        transform.localPosition += new Vector3(moveX, 0, moveZ ) * Time.deltaTime * moveSpeed;
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
            SetReward(1f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }

    }



}
