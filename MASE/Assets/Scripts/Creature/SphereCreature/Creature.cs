using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour
{
    //Checks
    public bool isChild = false;
    public bool TouchingCreature = false;
    public GameObject touchedCreature;

    //statistics
    public float Fitness;
    public float EnergyDrain;

    //Physical attributes
    public Brain brain;
    private DNA dna;
    public bool isdead;
    public float energy;
    public float size;
    public float view_distance;
    public float speedMultiplier;
    private FieldOfVision fov;
    Vector3 scaleChange;
    Color creatureColor;

    //Constants
    public float growthAmount;
    //States
    public float hunger;
    public float health;
    public float speed;
    public float maturity;
    //Vision
    public float dist_creature = 0f;
    public float angle_creature = 0f;
    public float dist_food = 0f;
    public float angle_food = 0f;
    //Clock
    public float Time_Alive;
    
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
            fov.viewRadius = view_distance;

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
            OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Want to lay
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
            fov.viewRadius = view_distance;
        }
    }

    private void Start()
    {
        //Initializing some basic stuff
        fov = this.transform.GetComponent<FieldOfVision>();
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
            Transform ClosestFood = ClosestTarget(fov.visibleTargets, "Food");
            Transform ClosestCreature = ClosestTarget(fov.visibleTargets, "Creature");
            if (ClosestFood != null)
            {
                dist_food = Vector3.Distance(this.transform.position, ClosestFood.position);
                angle_food = CalculateAngleToTarget(ClosestFood);
            }
            if (ClosestCreature != null)
            {
                dist_creature = Vector3.Distance(this.transform.position, ClosestCreature.position);
                angle_creature = CalculateAngleToTarget(ClosestCreature);
            }

            if (brain != null)
            {
                brain.Network[0].NodeValue = hunger;
                brain.Network[1].NodeValue = health;
                brain.Network[2].NodeValue = speed;
                brain.Network[3].NodeValue = maturity;
                brain.Network[4].NodeValue = dist_creature;
                brain.Network[5].NodeValue = angle_creature;
                brain.Network[6].NodeValue = dist_food;
                brain.Network[7].NodeValue = angle_food;
                brain.Network[8].NodeValue = Time_Alive;

                PhysicalTick();
                brain.BrainTick();

            }

            if (health > 0)
            {
                Time_Alive = Time_Alive + (Time.unscaledDeltaTime);
            }
            if (brain.Network[14].NodeValue > 0 && maturity >= 100f && energy > 80f)
            {
                if (TouchingCreature == true)
                {
                    if (touchedCreature != null)
                    {
                        Reproduce(touchedCreature.transform);
                        energy -= 70f;
                    }
                }
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
            if (health <= 0)
            {
                isdead = true;
                this.GetComponent<MeshRenderer>().material.color = Color.red;
                Destroy(this.gameObject);
            }
            if (health < 100f && energy > 1)
            {
                health += 0.5f;
            }
            if (energy > 100f)
            {
                energy = 100f;
            }
            EnergyFoodDrain(speed, size);
        }
    }
    public void EnergyFoodDrain(float Speed, float Size)
    {
        EnergyDrain = 0f;
        float FoodDrain = 0f;
        if (Speed == 0)
        {
            EnergyDrain = size * 0.02f;
            if (energy > 0)
            {
                energy -= EnergyDrain;
            }
        }
        else
        {
            EnergyDrain = Speed / 2 * (size * 0.02f);
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
        if (hunger > 100f)
        {
            hunger = 100f;
        }
    }

    //Movement Methods

    public void Reproduce(Transform parent)
    {
        GameObject offspring = Instantiate(this.gameObject, parent.position, Quaternion.identity);
        DNA newDNA = new DNA();
        offspring.GetComponent<Creature>().dna = newDNA;
        offspring.GetComponent<Creature>().brain = this.GetComponent<Creature>().brain;
        offspring.GetComponent<Creature>().isChild = true;
        if (UnityEngine.Random.Range(0, parent.GetComponent<Creature>().dna.getGene("Mutation_Chance")) == 1)
        {
            offspring.GetComponent<Creature>().brain.MutateBrain();
            offspring.GetComponent<Creature>().dna.Combine(this.GetComponent<Creature>().dna.Genes, parent.GetComponent<Creature>().dna.Genes);
            offspring.GetComponent<Creature>().dna.RandomizeGeneSet(this.transform.GetComponent<Creature>().dna.getGene("Mutation_Size"));
        }
        else
        {
            offspring.GetComponent<Creature>().dna.Combine(this.GetComponent<Creature>().dna.Genes, parent.GetComponent<Creature>().dna.Genes);
        }
    }

    public void Movement(float Forward, float Backward, float Left, float Right)
    { 
        //Vector3 movement = transform.forward * (speedMultiplier * Forward) + -transform.forward * (speedMultiplier * Backward) + -transform.right * (speedMultiplier * Left) + transform.right * (speedMultiplier * Right);

        rb.AddForce(transform.forward * (speedMultiplier * Forward));
        rb.AddForce(-transform.forward * (speedMultiplier * Backward));
        rb.AddForce(-transform.right * (speedMultiplier * Left));
        rb.AddForce(transform.right * (speedMultiplier * Right));
    }

    public float CalculateAngleToTarget(Transform targetTransform)
    {
        Vector3 direction = targetTransform.position - this.transform.position;
        //Debug.DrawRay(transform.position, direction, Color.cyan);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -90;

        return angle;
    }

    public Transform ClosestTarget (List<(Transform, float)> visibleTargets, string Tag)
    {
        visibleTargets.OrderByDescending(x => x.Item2);
        Transform closestTarget = null;

        for (int i = 0; i < visibleTargets.Count; i++)
        {
            if (visibleTargets[i].Item1 != null && visibleTargets[i].Item1.tag == Tag)
            {
                closestTarget = visibleTargets[i].Item1;
            }
        }

        return closestTarget;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Food" && brain.Network[13].NodeValue > 0)
        {  
            if (energy < 100f)
            {
                other.GetComponent<Plant>().health -= 20f;
                energy += 5f;
                hunger -= 5f;
            }
        }
        if (other.gameObject.tag == "Creature")
        {
            TouchingCreature = true;
            touchedCreature = other.gameObject;
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

    public void OnMouseDown() //For camera following
    {
        CameraController.instance.FollowTransform = transform;
    }
}
