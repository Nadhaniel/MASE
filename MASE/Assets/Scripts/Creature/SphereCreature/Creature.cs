using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Creature : MonoBehaviour
{
    //Checks
    public bool TouchingCreature = false;
    public GameObject touchedCreature;

    //statistics
    private float fitness;
    private float energyDrain;
    private int foodcollected;

    //Physical attributes
    private Brain brain;
    private DNA dna;
    private bool isdead;
    private float energy;
    private float size;
    private float view_distance;
    private float speedMultiplier;
    private FieldOfVision fov;
    private Vector3 scaleChange;
    private Color creatureColor;

    //Constants
    private float growthAmount;
    //States
    private float hunger;
    private float health;
    private float speed;
    private float maturity;
    //Vision
    private float dist_creature = 0f;
    private float angle_creature = 0f;
    private float dist_food = 0f;
    private float angle_food = 0f;
    private float touchingwater = 0;
    //Clock
    private float time_alive;
    //attached components
    private Rigidbody rb;
    //Brain settings
    private int InputNodes = 10;
    private float[] inputValues;
    private float[] outputs;

    public Brain Brain
    {
        get { return brain; }
        set { brain = value; }
    }

    public DNA DNA
    {
        get { return dna; }
        set { dna = value; }
    }

    public float Fitness
    {
        get { return fitness; }
        set { fitness = value; }
    }

    public float EnergyDrain
    {
        get { return energyDrain; }
        set { energyDrain = value; }
    }

    public int FoodCollected
    {
        get { return foodcollected; }
        set { foodcollected = value; }
    }

    public bool IsDead
    {
        get { return isdead; }
        set { isdead = value; }
    }

    public float Energy
    {
        get { return energy; }
        set { energy = value; }
    }

    public float Size
    {
        get { return size; }
        set { size = value; }
    }
    public float View_distance
    {
        get { return view_distance; }
        set { view_distance = value; }
    }
    public float SpeedMultiplier
    {
        get {return speedMultiplier;}
        set { speedMultiplier = value;}
    }

    public FieldOfVision FOV
    { get { return fov; } set {  fov = value; } }

    public Vector3 ScaleChange
    {
        get { return scaleChange; }
        set { scaleChange = value; }
    }
    public Color CreatureColor
    {
        get { return creatureColor; }
        set { creatureColor = value; }
    }
    
    public float GrowthAmount
    {
        get { return growthAmount; }
        set
        {
            growthAmount = value;
        }
    }

    public float Hunger
    {
        get {return hunger;
        }
        set { hunger = value; }
    }

    public float Health
    {
        get { return health; }
        set { health = value; }
    }
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    public float Maturity
    {
        get { return maturity; }
        set { maturity = value; }
    }

    public float Dist_creature
    {
        get { return dist_creature; }
        set { dist_creature = value; }
    }
    public float Angle_creature
    {
        get { return angle_creature; }
        set { angle_creature = value; }
    }
    public float Dist_food
    {
        get { return dist_food; }
        set { dist_food = value; }
    }
    public float Angle_food
    {
        get { return Angle_food; }
        set { angle_food = value; }
    }
    public float Touchingwater
    {
        get { return touchingwater; }
        set { touchingwater = value; }
    }

    public float Time_alive
    {
        get { return time_alive; }
        set { time_alive = value; }
    }

    public Rigidbody RB
    {
        get { return rb; }
    }

    public int Inputnodes
    {
        get { return InputNodes; }
        set { InputNodes = value; }
    }
    public float[] InputValues
    {
        get { return inputValues; }
        set { inputValues = value; }
    }
    public float[] Outputs
    {
        get { return outputs; }
        set { outputs = value; }
    }

    public void Init()
    {
        growthAmount = 0.25f;
        if (SimulationManager.spawnfromsave == false)
        {
            foodcollected = 0;
            energy = 100f;
            hunger = 100f;
            health = 100f;
            maturity = 0f;
            size = dna.getGene("Size");
        }
        speedMultiplier = dna.getGene("Speed");
        view_distance = dna.getGene("View_Distance");
        scaleChange = new Vector3(size, size, size);
        creatureColor = new Color(dna.getGene("Red_Color"), dna.getGene("Green_Color"), dna.getGene("Blue_Color"));
        fov = this.transform.GetComponent<FieldOfVision>();
        fov.viewRadius = view_distance;
        inputValues = new float[InputNodes];
        StartCoroutine(CalcSpeed());
        isdead = false;
        rb = this.GetComponent<Rigidbody>();
        //Initializing stats
        this.transform.localScale = scaleChange;
        this.GetComponent<MeshRenderer>().material.color = creatureColor;
        time_alive = 0f;
        inputValues[0] = hunger;
        inputValues[1] = health;
        inputValues[2] = speed;
        inputValues[3] = maturity;
        inputValues[4] = dist_creature;
        inputValues[5] = angle_creature;
        inputValues[6] = dist_food;
        inputValues[7] = angle_food;
        inputValues[8] = time_alive;
        inputValues[9] = touchingwater;
        PhysicalTick();
        outputs = brain.BrainTick_FF(inputValues);
    }

    private void FixedUpdate()
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

        inputValues[0] = hunger;
        inputValues[1] = health;
        inputValues[2] = speed;
        inputValues[3] = maturity;
        inputValues[4] = dist_creature;
        inputValues[5] = angle_creature;
        inputValues[6] = dist_food;
        inputValues[7] = angle_food;
        inputValues[8] = time_alive;
        inputValues[9] = touchingwater;
        PhysicalTick();
        outputs = brain.BrainTick_FF(inputValues);
        if (isdead == false)
        {
            Movement(outputs[0], outputs[1], outputs[2], outputs[3], outputs[4]);
        }
        if (outputs[5] > 0 && maturity >= 100f && energy > 80f)
        {
            if (TouchingCreature == true)
            {
                if (touchedCreature != null)
                {
                    if (touchedCreature.GetComponent<CreatureJobMove>().maturity >= 100f)
                    {
                        Reproduce(touchedCreature.transform);
                        energy -= 70f;
                    }
                }
            }
        }
        if (health > 0)
        {
            time_alive = time_alive + (Time.deltaTime);
        }
        Fitness = (time_alive * foodcollected) / 100;
        brain.fitness = Fitness;
    }

    //Tick Methods
    public void PhysicalTick()
    {
        if (isdead == false)
        {
            if (energy > 0)
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
                    this.transform.localScale = new Vector3(size, size, size);
                }
            }
            if (energy < 1)
            {
                health -= 1f;
            }
            if (health <= 0)
            {
                isdead = true;
                this.GetComponent<MeshRenderer>().material.color = Color.red;
                SimulationManager.instance.creatures.Remove(this.gameObject);
                this.gameObject.SetActive(false);
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
        if (energy <=0 )
        {
            energy = 0;
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
        if (hunger <= 0f)
        {
            hunger = 0f;
        }
    }

    //Movement Methods

    public void Reproduce(Transform parent)
    {
        GameObject temp = CreaturePool.instance.GetPooledObject();
        if (temp != null)
        {
            temp.SetActive(true);
            GameObject offspring = temp;
            offspring.transform.position = parent.position;
            DNA newDNA = new DNA();
            offspring.GetComponent<CreatureJobMove>().dna = newDNA;
            if (this.brain.fitness >= parent.GetComponent<CreatureJobMove>().brain.fitness)
            {
                offspring.GetComponent<CreatureJobMove>().brain = this.GetComponent<CreatureJobMove>().brain;
            }
            else
            {
                offspring.GetComponent<CreatureJobMove>().brain = parent.GetComponent<CreatureJobMove>().brain;
            }
            
            //int popcount = SimulationManager.instance.creatures.Length;
            //offspring.name = "Creature: " + (popcount + 1);
            if (UnityEngine.Random.Range(0, parent.GetComponent<CreatureJobMove>().dna.getGene("Mutation_Chance")) == 1)
            {
                offspring.GetComponent<CreatureJobMove>().brain.MutateNN(1);
                offspring.GetComponent<CreatureJobMove>().dna.Combine(this.GetComponent<CreatureJobMove>().dna.Genes, parent.GetComponent<CreatureJobMove>().dna.Genes);
                offspring.GetComponent<CreatureJobMove>().dna.RandomizeGeneSet(this.transform.GetComponent<CreatureJobMove>().dna.getGene("Mutation_Size"));
            }
            else
            {
                offspring.GetComponent<CreatureJobMove>().dna.Combine(this.GetComponent<CreatureJobMove>().dna.Genes, parent.GetComponent<CreatureJobMove>().dna.Genes);
            }
            offspring.GetComponent<CreatureJobMove>().Init();
            SimulationManager.instance.creatures.Add(offspring);
        }
    }

    public void Movement(float Forward, float Backward, float Left, float Right, float rotation)
    {
        //Vector3 movement = transform.forward * (speedMultiplier * Forward) + -transform.forward * (speedMultiplier * Backward) + -transform.right * (speedMultiplier * Left) + transform.right * (speedMultiplier * Right);
        if (Forward != float.NaN)
        {
           rb.AddForce(transform.forward * (speedMultiplier * Forward));
        }
        if (Backward != float.NaN)
        {
            rb.AddForce(-transform.forward * (speedMultiplier * Backward));
        }
        if (Left != float.NaN)
        {
            rb.AddForce(-transform.right * (speedMultiplier * Left));
        }
        if (Right != float.NaN)
        {
            rb.AddForce(transform.right * (speedMultiplier * Right));
        }
        Vector3 rotationdir = new Vector3(0, rotation, 0);
        this.transform.Rotate(rotationdir * speedMultiplier * Time.deltaTime);
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
        if (outputs.Length > 0)
        {
            if (other.gameObject.tag == "Food" && outputs[6] > 0)
            {
                if (energy < 100f)
                {
                    other.GetComponent<Plant>().health -= 20f;
                    energy += 5f;
                    hunger -= 5f;
                    foodcollected += 1;
                }
            }
        }
        if (other.gameObject.tag == "Creature")
        {
            TouchingCreature = true;
            touchedCreature = other.gameObject;
        }
        if (other.gameObject.tag == "water")
        {
            touchingwater = 1f;
            health -= 5f;
        }
        else
        {
            touchingwater = 0f;
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
        InfoGetter.instance.isSelected = true;
        InfoGetter.instance.selectedCreature = this.gameObject;
        InfoGetter.instance.HideRevealPanel();
        if (BrainView.instance != null && BrainView.instance.gameObject.activeSelf)
        {
            BrainView.instance.ClearBrain();
            BrainView.instance.Init();
        }
    }
}
