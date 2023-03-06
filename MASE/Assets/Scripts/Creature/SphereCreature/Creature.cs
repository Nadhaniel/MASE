using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Creature : MonoBehaviour
{
    //Checks
    public bool isChild = false;

    //statistics
    public float Fitness;

    //Physical attributes
    public Brain brain;
    public DNA dna;
    public bool isdead;
    public float energy;
    public float size;
    public float view_distance;
    public float speedMultiplier;
    Vector3 scaleChange;
    Color creatureColor;

    //Constants
    private float growthAmount;
    //States
    private float hunger;
    private float health;
    private float speed;
    private float maturity;
    //Vision
    private float dist_creature;
    private float angle_creature;
    private float dist_food;
    private float angle_food;
    //Clock
    private float Time_Alive;
    
    private Rigidbody rb;
    LinkedList<Node> InputNodes = new LinkedList<Node>();
    LinkedList<Node> OutputNodes = new LinkedList<Node>();


    public void Init()
    {
        if (isChild == false)
        {
            dna = new DNA();
            growthAmount = 0.25f;
            hunger = 100f;
            health = 100f;
            maturity = 0f;
            size = dna.getGene("Size");
            speedMultiplier = dna.getGene("Speed");
            view_distance = dna.getGene("View_Distance");
            scaleChange = new Vector3(size, size, size);
            creatureColor = new Color(dna.getGene("Red_Color"), dna.getGene("Green_Color"), dna.getGene("Blue_Color"));

            InputNodes.AddLast(new Node(NodeTypes.Input, hunger));
            InputNodes.AddLast(new Node(NodeTypes.Input, health));
            InputNodes.AddLast(new Node(NodeTypes.Input, speed));
            InputNodes.AddLast(new Node(NodeTypes.Input, maturity));
            InputNodes.AddLast(new Node(NodeTypes.Input, dist_creature));
            InputNodes.AddLast(new Node(NodeTypes.Input, angle_creature));
            InputNodes.AddLast(new Node(NodeTypes.Input, dist_food));
            InputNodes.AddLast(new Node(NodeTypes.Input, angle_food));
            InputNodes.AddLast(new Node(NodeTypes.Input, Time_Alive));

            OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Move forward
            OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Move left
            OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Move right
            OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Move backward
            OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Want to eat
            energy = 100f;

            brain = new Brain(InputNodes, OutputNodes);
            brain.MutateBrain();
        }
        else
        {
            growthAmount = 0.25f;
            hunger = 100f;
            health = 100f;
            maturity = 0f;
            size = dna.getGene("Size");
            speedMultiplier = dna.getGene("Speed");
            view_distance = dna.getGene("View_Distance");
            scaleChange = new Vector3(size, size, size);
            creatureColor = new Color(dna.getGene("Red_Color"), dna.getGene("Green_Color"), dna.getGene("Blue_Color"));
        }
    }

    private void Start()
    {
        //Initializing some basic stuff
        Init();
        StartCoroutine(CalcSpeed());
        isdead = false;
        rb = this.GetComponent<Rigidbody>();
        //Initializing stats
        this.transform.localScale = scaleChange;
        this.GetComponent<MeshRenderer>().material.color = creatureColor;
        Time_Alive = 0f;
    }

    private void Update()
    {
        float disttofood = 0f;
        if (Physics.Raycast(rb.position, this.transform.TransformDirection(Vector3.forward), out RaycastHit hit, view_distance))
        {
            //Debug.Log("Hit something");
            Debug.DrawRay(rb.position, this.transform.TransformDirection(Vector3.left) * view_distance, Color.red);
            if (hit.collider.tag == "Food")
            {
                disttofood = hit.distance;
            }
        }
        else 
        {
            //Debug.Log("Hit Nothing");
            Debug.DrawRay(rb.position, this.transform.TransformDirection(Vector3.left) * view_distance, Color.black);
        }
        if (brain != null)
        {
            brain.Network[0].NodeValue = hunger;
            brain.Network[1].NodeValue = health;
            brain.Network[2].NodeValue = speed;
            brain.Network[3].NodeValue = maturity;
            brain.Network[6].NodeValue = disttofood;
            brain.Network[8].NodeValue = Time_Alive;

            PhysicalTick();
            brain.BrainTick();
        }

        if (health > 0)
        {
            Time_Alive = Time_Alive + (Time.unscaledDeltaTime);
        }
        Fitness = Time_Alive;
    }

    private void FixedUpdate()
    {
        if (brain != null && isdead == false)
        {
            Movement(brain.Network[9].NodeValue, brain.Network[10].NodeValue, brain.Network[11].NodeValue, brain.Network[12].NodeValue);
        }
    }

    //Tick Methods
    public void PhysicalTick()
    {
        if (isdead == false)
        {
            if (maturity < 100f)
            {
                maturity += growthAmount * (energy * 0.02f);
            }
            if (size < dna.getGene("Max_Size"))
            {
                size += growthAmount * (energy * 0.02f);
                if (size > dna.getGene("Max_Size"))
                {
                    size = dna.getGene("Max_Size");
                }
                this.transform.localScale.Set(size, size, size);
            }
            if (energy < 1)
            {
                health -= 1f;
            }
            if (health == 0)
            {
                isdead = true;
                this.GetComponent<MeshRenderer>().material.color = Color.red;
                //Destroy(this.transform.gameObject);
            }
            if (health < 100f && energy > 1)
            {
                health += 0.5f;
            }
            EnergyFoodDrain(speed, size);
        }
    }
    public void EnergyFoodDrain(float Speed, float Size)
    {
        float EnergyDrain = 0f;
        float FoodDrain = 0f;
        if (Speed == 0)
        {
            EnergyDrain = size * 0.2f;
            if (energy > 0)
            {
                energy -= EnergyDrain;
            }
        }
        else
        {
            EnergyDrain = Speed / 2 * (size * 0.2f);
            if (energy > 0)
            {
                energy -= EnergyDrain;
            }
        }

        if (energy < 85)
        {
            FoodDrain = 0.25f;
            hunger += FoodDrain;
        }
        else if (energy < 50)
        {
            FoodDrain = 0.5f;
            hunger += FoodDrain;
        }
        else if (energy < 25)
        {
            FoodDrain = 1.0f;
            hunger += FoodDrain;
        }
    }

    //Movement Methods

    public void Reproduce()
    {
        
    }

    public void Movement(float Forward, float Backward, float Left, float Right)
    { 
        //Vector3 movement = transform.forward * (speedMultiplier * Forward) + -transform.forward * (speedMultiplier * Backward) + -transform.right * (speedMultiplier * Left) + transform.right * (speedMultiplier * Right);

        rb.AddForce(transform.forward * (speedMultiplier * Forward));
        rb.AddForce(-transform.forward * (speedMultiplier * Backward));
        rb.AddForce(-transform.right * (speedMultiplier * Left));
        rb.AddForce(transform.right * (speedMultiplier * Right));
    }
    

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Food" && brain.Network[13].NodeValue > 0)
        {
            other.GetComponent<Plant>().health -= 2f;
            hunger -= 2f;
            energy += 10f;
        }
    }

    IEnumerator CalcSpeed()
    {
        bool isPlaying = true;

        while (isPlaying)
        {
            Vector3 prevPos = transform.position;

            yield return new WaitForFixedUpdate();

            speed = Mathf.Round(Vector3.Distance(transform.position, prevPos) / Time.fixedUnscaledDeltaTime);
        }
    }
}
