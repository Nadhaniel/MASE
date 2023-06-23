using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class NEATUtils
{
    //Genome comparison tools
    public static float NetworkComparison(float c1, float c2, float c3, List<ConnectionGene> connectionGenes1, List<ConnectionGene> connectionGenes2)
    {
        float CD = 0f;
        int Disjoint = Number_of_dis_excess(connectionGenes1, connectionGenes2)[0];
        int Excess = Number_of_dis_excess(connectionGenes1, connectionGenes2)[1];
        float WeightAvg = WeightAverage(connectionGenes1, connectionGenes2);
        int N = 1;
        if (connectionGenes1.Count >= connectionGenes2.Count)
        {
            N = connectionGenes1.Count;
        }
        else
        {
            N = connectionGenes2.Count;
        }

        CD = ((c1 * Excess) / N) + ((c2 * Disjoint) / N) + c3 * WeightAvg;

        return CD;
    }

    public static int[] Number_of_dis_excess(List<ConnectionGene> connectionGenes1, List<ConnectionGene> connectionGenes2) //holds the number of disjoint genes and excess genes
    {
        int disjoint = 0;
        int excess = 0;
        int breakpoint = 0;
        int parent1endInnov = 0;
        int parent2endInnov = 0;
        if (connectionGenes1.Count != 0 )
        {
            parent1endInnov = connectionGenes1[connectionGenes1.Count - 1].innovation_no;
        }
        if (connectionGenes2.Count != 0)
        {
            parent2endInnov = connectionGenes2[connectionGenes2.Count - 1].innovation_no;
        }

        if (parent1endInnov >= parent2endInnov)
        {
            breakpoint = parent2endInnov;
        }
        else if (parent2endInnov > parent1endInnov)
        {
            breakpoint = parent1endInnov;
        }

        foreach (ConnectionGene connectionGene in connectionGenes1)
        {
            if (connectionGene.disabled == false)
            {
                if (connectionGene.innovation_no <= breakpoint)
                {
                    disjoint += DisjointCheck(connectionGenes2, connectionGene.innovation_no, connectionGenes1);
                }
                else
                {
                    excess += InnovNumFind(connectionGenes2, connectionGene.innovation_no) ? 0 : 1;
                }
            }
        }

        int[] disjointandexcess = { disjoint, excess };
        return disjointandexcess;
    }

    private static bool InnovNumFind(List<ConnectionGene> connectionGenes, int innov_num)
    {
        for (int i = 0; i < connectionGenes.Count; i++)
        {
            if (connectionGenes[i].innovation_no == innov_num)
            {
                return true;
            }
        }

        return false;
    }

    private static int DisjointCheck(List<ConnectionGene> connectionGenes, int comparing_innov_num, List<ConnectionGene> base_connectionGenes)
    {
        if (InnovNumFind(connectionGenes, comparing_innov_num))
        {
            ConnectionGene comparing_gene = null;
            ConnectionGene base_gene = null;
            foreach (ConnectionGene gene in connectionGenes)
            {

                if (gene.innovation_no == comparing_innov_num)
                {
                    comparing_gene = gene;
                    break;
                }
            }
            foreach (ConnectionGene gene in base_connectionGenes)
            {
                if (gene.innovation_no == comparing_innov_num)
                {
                    base_gene = gene;
                    break;
                }
            }
            if (comparing_gene.SourceNode == base_gene.SourceNode && comparing_gene.ReceivingNode == base_gene.ReceivingNode)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            return 1;
        }
    }

    private static float WeightAverage(List<ConnectionGene> connectionGenes1, List<ConnectionGene> connectionGenes2)
    {
        float weightAverage = 0;
        int count = 0;
        List<ConnectionGene> MatchingInnovGenes1 = new List<ConnectionGene>();
        List<ConnectionGene> MatchingInnovGenes2 = new List<ConnectionGene>();
        foreach (ConnectionGene gene in connectionGenes1)
        {
            if (InnovNumFind(connectionGenes2, gene.innovation_no))
            {
                MatchingInnovGenes1.Add(gene);
                MatchingInnovGenes2.Add(connectionGenes2.Find(x => x.innovation_no == gene.innovation_no));
            }
        }

        for (int i = 0; i < MatchingInnovGenes1.Count; i++)
        {
            weightAverage += Math.Abs(MatchingInnovGenes1[i].weight - MatchingInnovGenes2[i].weight);
        }

        weightAverage = weightAverage / MatchingInnovGenes1.Count;

        return weightAverage;
    }

    //JSON conversion tools
    public static (List<NodeGeneJson>, List<ConnectionGeneJson>) SetGenomeJSON(NNGenome genome)
    {
        List<NodeGeneJson> nodeGeneJsons = new List<NodeGeneJson>();
        List<ConnectionGeneJson> connectionGeneJsons = new List<ConnectionGeneJson>();
        (List<NodeGeneJson>, List<ConnectionGeneJson>) lists;
        lists.Item1 = nodeGeneJsons;
        lists.Item2 = connectionGeneJsons;
        foreach (NodeGene gene in genome.nodeGenes)
        {
            NodeGeneJson nodeGeneJson = new NodeGeneJson();
            nodeGeneJson.id = gene.id;
            nodeGeneJson.type = gene.type;
            nodeGeneJsons.Add(nodeGeneJson);
        }
        foreach (ConnectionGene connection in genome.connectionGenes)
        {
            ConnectionGeneJson connectionGeneJson = new ConnectionGeneJson();
            connectionGeneJson.SourceNode = connection.SourceNode;
            connectionGeneJson.ReceivingNode = connection.ReceivingNode;
            connectionGeneJson.weight = connection.weight;
            connectionGeneJson.disabled = connection.disabled;
            connectionGeneJson.innovation_no = connection.innovation_no;
            connectionGeneJsons.Add(connectionGeneJson);
        }
        return lists;

    }

    public static NNGenome SetGenome(NNGenomeJson genome)
    {
        List<NodeGene> nodeGenes = new List<NodeGene>();
        List<ConnectionGene> connectionGenes = new List<ConnectionGene>();
        (List<NodeGene>, List<ConnectionGene>) lists;
        lists.Item1 = nodeGenes;
        lists.Item2 = connectionGenes;
        foreach (NodeGeneJson geneJson in genome.nodeGenes)
        {
            NodeGene node = new NodeGene(geneJson.id, geneJson.type);
            nodeGenes.Add(node);
        }
        foreach (ConnectionGeneJson connectionJson in genome.connectionGenes)
        {
            ConnectionGene connectionGene = new ConnectionGene(connectionJson.SourceNode, connectionJson.ReceivingNode, connectionJson.weight, connectionJson.disabled, connectionJson.innovation_no);

            connectionGenes.Add(connectionGene);
        }
        NNGenome savedGenome = new NNGenome();
        savedGenome.nodeGenes = lists.Item1;
        savedGenome.connectionGenes = lists.Item2;
        savedGenome.species_num = genome.species_num;
        return savedGenome;
    }
}
