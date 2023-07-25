using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using System.Runtime.CompilerServices;

public struct CreatureJob : IJob
{
    ////Checks
    //public bool TouchingCreature;
    //public GameObject touchedCreature;

    ////statistics
    //public float fitness;
    //public float energyDrain;
    //public int foodcollected;

    ////Physical attributes
    //public Brain brain;
    //public DNA dna;
    //public bool isdead;
    //public float energy;
    //public float size;
    //public float view_distance;
    //public float speedMultiplier;
    //private FieldOfVision fov;
    //Vector3 scaleChange;
    //private Color creatureColor;

    ////Constants
    //public float growthAmount;
    ////States
    //public float hunger;
    //public float health;
    //public float speed;
    //public float maturity;
    ////Vision
    //public float dist_creature;
    //public float angle_creature;
    //public float dist_food;
    //public float angle_food;
    //public float touchingwater;
    ////Clock
    //public float time_alive;
    ////attached components
    //private Rigidbody rb;
    ////Brain settings
    //public int InputNodes;
    //public float[] inputValues;
    //public float[] outputs;
    public Creature creatureData;
    public Vector3 lastpos;


    public CreatureJob(Creature creature)
    {
        //TouchingCreature = TouchedCreature;
        //dist_creature = 0f;
        //angle_creature = 0f;
        //dist_food = 0f;
        //angle_food = 0f;
        //touchingwater = 0;
        //InputNodes = 10;

        //touchedCreature = TouchedCreature;
        //brain = Brain;
        //dna = Dna;
        //fov = Fov;
        //rb = Rb;
        //scaleChange = ScaleChange;
        //creatureColor = CreatureColor;
        //foodcollected = Foodcollected;
        //health = Health;
        //hunger = Hunger;
        //growthAmount = GrowthAmount;
        //speedMultiplier = SpeedMultiplier;
        //view_distance = View_distance;
        //size = Size;
        //energy = Energy;
        //isdead = Isdead;
        //foodcollected = Foodcollected;
        //energyDrain = EnergyDrain;
        //fitness = Fitness;
        //speed = Speed;
        //maturity = Maturity;
        //dist_creature = Dist_creature; dist_food = Dist_food;
        //angle_creature = Angle_creature; angle_food = Angle_Food;
        //touchingwater = TouchingWater;
        //time_alive = Time_alive;
        //inputValues = InputValues;
        //outputs = Outputs;
        creatureData = creature;
        lastpos = creatureData.RB.transform.position;
        
    }

    //public void Init()
    //{
    //    growthAmount = 0.25f;
    //    if (SimulationManager.spawnfromsave == false)
    //    {
    //        foodcollected = 0;
    //        energy = 100f;
    //        hunger = 100f;
    //        health = 100f;
    //        maturity = 0f;
    //        size = dna.getGene("Size");
    //    }
    //    speedMultiplier = dna.getGene("Speed");
    //    view_distance = dna.getGene("View_Distance");
    //    scaleChange = new Vector3(size, size, size);
    //    creatureColor = new Color(dna.getGene("Red_Color"), dna.getGene("Green_Color"), dna.getGene("Blue_Color"));
    //    fov.viewRadius = view_distance;
    //    inputValues = new float[InputNodes];
    //    isdead = false;
    //    //Initializing stats
    //    rb.transform.localScale = scaleChange;
    //    rb.GetComponent<MeshRenderer>().material.color = creatureColor;
    //    time_alive = 0f;
    //    inputValues[0] = hunger;
    //    inputValues[1] = health;
    //    inputValues[2] = speed;
    //    inputValues[3] = maturity;
    //    inputValues[4] = dist_creature;
    //    inputValues[5] = angle_creature;
    //    inputValues[6] = dist_food;
    //    inputValues[7] = angle_food;
    //    inputValues[8] = time_alive;
    //    inputValues[9] = touchingwater;
    //    PhysicalTick();
    //    outputs = brain.BrainTick_FF(inputValues);
    //}

    public void Execute()
    {
        Transform ClosestFood = ClosestTarget(creatureData.FOV.visibleTargets, "Food");
        Transform ClosestCreature = ClosestTarget(creatureData.FOV.visibleTargets, "Creature");
        if (ClosestFood != null)
        {
            creatureData.Dist_food = Vector3.Distance(creatureData.RB.transform.position, ClosestFood.position);
            creatureData.Angle_food = CalculateAngleToTarget(ClosestFood);
        }
        if (ClosestCreature != null)
        {
            creatureData.Dist_creature = Vector3.Distance(creatureData.RB.transform.position, ClosestCreature.position);
            creatureData.Angle_creature = CalculateAngleToTarget(ClosestCreature);
        }

        creatureData.InputValues[0] = creatureData.Hunger;
        creatureData.InputValues[1] = creatureData.Health;
        creatureData.InputValues[2] = creatureData.Speed;
        creatureData.InputValues[3] = creatureData.Maturity;
        creatureData.InputValues[4] = creatureData.Dist_creature;
        creatureData.InputValues[5] = creatureData.Angle_creature;
        creatureData.InputValues[6] = creatureData.Dist_food;
        creatureData.InputValues[7] = creatureData.Angle_food;
        creatureData.InputValues[8] = creatureData.Time_alive;
        creatureData.InputValues[9] = creatureData.Touchingwater;
        PhysicalTick();
        creatureData.Outputs = creatureData.Brain.BrainTick_FF(creatureData.InputValues);
        if (creatureData.IsDead == false)
        {
            Movement(creatureData.Outputs[0], creatureData.Outputs[1], creatureData.Outputs[2], creatureData.Outputs[3], creatureData.Outputs[4]);
        }
        if (creatureData.Outputs[5] > 0 && creatureData.Maturity >= 100f && creatureData.Energy > 80f)
        {
            if (creatureData.TouchingCreature == true)
            {
                if (creatureData.touchedCreature != null)
                {
                    if (creatureData.touchedCreature.GetComponent<Creature>().Maturity >= 100f)
                    {
                        Reproduce(creatureData.touchedCreature.transform);
                        creatureData.Energy -= 70f;
                    }
                }
            }
        }
        if (creatureData.Health > 0)
        {
            creatureData.Time_alive = creatureData.Time_alive + (Time.deltaTime);
        }
        creatureData.Fitness = (creatureData.Time_alive * creatureData.FoodCollected) / 100;
        creatureData.Brain.fitness = creatureData.Fitness;
    }

    //Tick Methods
    public void PhysicalTick()
    {
        if (creatureData.IsDead == false)
        {
            if (creatureData.Energy > 0)
            {
                if (creatureData.Maturity < 100f)
                {
                    creatureData.Maturity += creatureData.GrowthAmount * (creatureData.Energy * 0.02f);
                }
                if (creatureData.Size < creatureData.DNA.getGene("Max_Size"))
                {
                    creatureData.Size += creatureData.GrowthAmount * (creatureData.Energy * 0.02f);
                    if (creatureData.Size > creatureData.DNA.getGene("Max_Size"))
                    {
                        creatureData.Size = creatureData.DNA.getGene("Max_Size");
                    }
                    creatureData.RB.transform.localScale = new Vector3(creatureData.Size, creatureData.Size, creatureData.Size);
                }
            }
            if (creatureData.Energy < 1)
            {
                creatureData.Energy -= 1f;
            }
            if (creatureData.Energy <= 0)
            {
                creatureData.IsDead = true;
                creatureData.RB.GetComponent<MeshRenderer>().material.color = Color.red;
                SimulationManager.instance.creatures.Remove(creatureData.RB.gameObject);
                creatureData.RB.gameObject.SetActive(false);
            }
            if (creatureData.Health < 100f && creatureData.Energy > 1)
            {
                creatureData.Health += 0.5f;
            }
            if (creatureData.Energy > 100f)
            {
                creatureData.Energy = 100f;
            }
            EnergyFoodDrain(creatureData.Speed, creatureData.Size);
        }
    }
    public void EnergyFoodDrain(float Speed, float Size)
    {
        creatureData.EnergyDrain = 0f;
        float FoodDrain = 0f;
        if (Speed == 0)
        {
            creatureData.EnergyDrain = creatureData.Size * 0.02f;
            if (creatureData.Energy > 0)
            {
                creatureData.Energy -= creatureData.EnergyDrain;
            }
        }
        else
        {
            creatureData.EnergyDrain = Speed / 2 * (creatureData.Size * 0.02f);
            if (creatureData.Energy > 0)
            {
                creatureData.Energy -= creatureData.EnergyDrain;
            }
        }
        if (creatureData.Energy <=0 )
        {
            creatureData.Energy = 0;
        }

        if (creatureData.Energy < 85)
        {
            FoodDrain = 0.25f;
            creatureData.Hunger += FoodDrain;
        }
        else if (creatureData.Energy < 50)
        {
            FoodDrain = 0.5f;
            creatureData.Hunger += FoodDrain;
        }
        else if (creatureData.Energy < 25)
        {
            FoodDrain = 1.0f;
            creatureData.Hunger += FoodDrain;
        }
        if (creatureData.Hunger > 100f)
        {
            creatureData.Hunger = 100f;
        }
        if (creatureData.Hunger <= 0f)
        {
            creatureData.Hunger = 0f;
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
            offspring.GetComponent<Creature>().DNA = newDNA;
            if (creatureData.Brain.fitness >= parent.GetComponent<Creature>().Brain.fitness)
            {
                offspring.GetComponent<Creature>().Brain = creatureData.Brain;
            }
            else
            {
                offspring.GetComponent<Creature>().Brain = parent.GetComponent<Creature>().Brain;
            }
            
            //int popcount = SimulationManager.instance.creatures.Length;
            //offspring.name = "Creature: " + (popcount + 1);
            if (UnityEngine.Random.Range(0, parent.GetComponent<Creature>().DNA.getGene("Mutation_Chance")) == 1)
            {
                offspring.GetComponent<Creature>().Brain.MutateNN(1);
                offspring.GetComponent<Creature>().DNA.Combine(creatureData.DNA.Genes, parent.GetComponent<Creature>().DNA.Genes);
                offspring.GetComponent<Creature>().DNA.RandomizeGeneSet(creatureData.DNA.getGene("Mutation_Size"));
            }
            else
            {
                offspring.GetComponent<Creature>().DNA.Combine(creatureData.DNA.Genes, parent.GetComponent<Creature>().DNA.Genes);
            }
            offspring.GetComponent<Creature>().Init();
            SimulationManager.instance.creatures.Add(offspring);
        }
    }

    public void Movement(float Forward, float Backward, float Left, float Right, float rotation)
    {
        //Vector3 movement = transform.forward * (speedMultiplier * Forward) + -transform.forward * (speedMultiplier * Backward) + -transform.right * (speedMultiplier * Left) + transform.right * (speedMultiplier * Right);
        if (Forward != float.NaN)
        {
           creatureData.RB.AddForce(creatureData.RB.transform.forward * (creatureData.SpeedMultiplier * Forward));
        }
        if (Backward != float.NaN)
        {
            creatureData.RB.AddForce(-creatureData.RB.transform.forward * (creatureData.SpeedMultiplier * Backward));
        }
        if (Left != float.NaN)
        {
            creatureData.RB.AddForce(-creatureData.RB.transform.right * (creatureData.SpeedMultiplier * Left));
        }
        if (Right != float.NaN)
        {
            creatureData.RB.AddForce(creatureData.RB.transform.right * (creatureData.SpeedMultiplier * Right));
        }
        Vector3 rotationdir = new Vector3(0, rotation, 0);
        creatureData.RB.transform.Rotate(rotationdir * creatureData.SpeedMultiplier * Time.deltaTime);
    }

    public float CalculateAngleToTarget(Transform targetTransform)
    {
        Vector3 direction = targetTransform.position - creatureData.RB.transform.position;
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
        if (creatureData.Outputs.Length > 0)
        {
            if (other.gameObject.tag == "Food" && creatureData.Outputs[6] > 0)
            {
                if (creatureData.Energy < 100f)
                {
                    other.GetComponent<Plant>().health -= 20f;
                    creatureData.Energy += 5f;
                    creatureData.Hunger -= 5f;
                    creatureData.FoodCollected += 1;
                }
            }
        }
        if (other.gameObject.tag == "Creature")
        {
            creatureData.TouchingCreature = true;
            creatureData.touchedCreature = other.gameObject;
        }
        if (other.gameObject.tag == "water")
        {
            creatureData.Touchingwater = 1f;
            creatureData.Health -= 5f;
        }
        else
        {
            creatureData.Touchingwater = 0f;
        }
    }

    public float CalcSpeed()
    {
        // Calculate the displacement vector
        float speed = 0f;
        Vector3 displacement = creatureData.RB.transform.position - lastpos;

        // Calculate the distance traveled
        float distance = displacement.magnitude;

        // Calculate the speed using distance and time
        speed = distance / Time.deltaTime;

        // Update the last position for the next frame
        lastpos = creatureData.RB.transform.position;
        return speed;
    }

    public void OnMouseDown() //For camera following
    {
        CameraController.instance.FollowTransform = creatureData.RB.transform;
        InfoGetter.instance.isSelected = true;
        InfoGetter.instance.selectedCreature =  creatureData.RB.gameObject;
        InfoGetter.instance.HideRevealPanel();
        if (BrainView.instance != null && BrainView.instance.gameObject.activeSelf)
        {
            BrainView.instance.ClearBrain();
            BrainView.instance.Init();
        }
    }
}
