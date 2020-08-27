using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RacingAgent : Agent
{

    public GameObject Targets; 
    public GameObject Base;  
    Rigidbody _rb;

    //Reset
    Vector3 spawnPosition;
    Quaternion spawnRotation;

    //Has goal
    bool hasGoal;


    // Start is called before the first frame update
    public override void Initialize()
    {
        _rb = GetComponent<Rigidbody>(); 
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions    
        foreach(Transform child in Targets.transform)
        {
            sensor.AddObservation(child.localPosition);

        }
        
        sensor.AddObservation(Base.transform.localPosition);    
        sensor.AddObservation(this.transform.position);
        // Agent velocity
        sensor.AddObservation(_rb.velocity.x);
        sensor.AddObservation(_rb.velocity.z);
    }

    public override void OnActionReceived(float[] action)
    {     
        //AddReward(-1f/MaxStep);   
        MoveAgent(action);        

        // Fell off platform
        if (this.transform.position.y < -1)
        {
            AddReward(-0.5f);                        
            ResetPos();
            //EndEpisode();
        }
    }
    
    public void MoveAgent(float[] act)
    {   
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = Mathf.FloorToInt(act[0]);
            switch (action)
            {
                case 1:
                    dirToGo = transform.forward * 1f;
                    break;
                case 2:
                    dirToGo = transform.forward * -1f;
                    break;
                case 3:
                    rotateDir = transform.up * 1f;
                    break;
                case 4:
                    rotateDir = transform.up * -1f;
                    break;
            }

        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        _rb.AddForce(dirToGo * 0.5f, ForceMode.VelocityChange);

            
    }

    public override void Heuristic(float[] actionsOut)
    {                
        actionsOut[0] = 0;
        if (Input.GetKey(KeyCode.D))
        {
            actionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            actionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            actionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            actionsOut[0] = 2;
        }
                 
        
    }

    public override void OnEpisodeBegin()
    {
        hasGoal = false;
        ResetPos();
        spawnTargets();
    }

    private void OnTriggerEnter(Collider c)
    {

        if (c.gameObject.CompareTag("Goal"))
        {
            if(!hasGoal){
                AddReward(1f);            
                c.gameObject.SetActive(false);             
                hasGoal = true;         
            }           

        }
        if (c.gameObject.CompareTag("Base") && hasGoal)
        {
            SetReward(1f);           
            hasGoal = false;            
            //spawnTargets();
            EndEpisode();
        }
    }

    private void spawnTargets()
    {

        foreach(Transform child in Targets.transform)
        {
            child.gameObject.SetActive(true);
            // Move the target to a new spot
            child.localPosition = new Vector3(Random.Range(-1.0f, 1.0f) * 20,
                                      1.5f,
                                      Random.Range(-1.0f, 1.0f) * 20);

        }
        //Target.SetActive(true);  
        
    }


    public void ResetPos() {
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;

            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

}
