using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Level
{
    public string LHS;
    public List<Transform> objectPositions;
    public float fitness;
}

public class EvolutionManager : MonoBehaviour
{



    List<Level> generatedLevels = new List<Level>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InsertLevelData(string LHS, List<GameObject> list, float fitness)
    {
        Level level = new Level();
        level.LHS = LHS;
        level.objectPositions = new List<Transform>();
        foreach (GameObject o in list)
            level.objectPositions.Add(o.transform);
        level.fitness = fitness;

        generatedLevels.Add(level);
    }

    static int SortByFitness(Level l1, Level l2)
    {
        return l1.fitness.CompareTo(l2.fitness);
    }

    public Level CreateBestLevel()
    {
        generatedLevels = TournamentSelection();
        CreateNewPopulation();
        //Build levels for last 50 and calculate fitness
        generatedLevels.Sort(SortByFitness);
        generatedLevels.Reverse();
        

        Level best = generatedLevels[0];
        return best;
    }

    void CreateNewPopulation()
    {
        List<Level> tempList = new List<Level>();
        tempList.AddRange(generatedLevels.GetRange(0, generatedLevels.Count));

        while(tempList.Count != 0)
        {
            int randomPos1 = Random.Range(0, tempList.Count);

            Level temp1 = tempList[randomPos1];
            tempList.RemoveAt(randomPos1);

            int randomPos2 = Random.Range(0, tempList.Count);
            while (randomPos2 == randomPos1)
            {
                if (tempList.Count == 1)
                    break;
                randomPos2 = Random.Range(0, tempList.Count);
            }

            Level temp2 = tempList[randomPos2];
            tempList.RemoveAt(randomPos2);

            CrossOver(temp1, temp2);
        }
    }

    void TwoPointCrossover(Level dad, Level mom)
    {
        int platformPosDad = Random.Range(0, dad.LHS.IndexOf('E'));
        int platformPosMom = Random.Range(0, mom.LHS.IndexOf('E'));

        int fruitPosDad = Random.Range(dad.LHS.IndexOf('E') + 1, dad.LHS.Length) - dad.LHS.IndexOf('E');
        int fruitPosMom = Random.Range(mom.LHS.IndexOf('E') + 1, mom.LHS.Length) - mom.LHS.IndexOf('E');

        Level child1 = dad;
        child1.LHS = child1.LHS.Remove(0, platformPosDad + 1);
        child1.LHS = child1.LHS.Insert(0, mom.LHS.Substring(0, platformPosMom));
        child1.objectPositions.RemoveRange(0, platformPosDad + 1);
        child1.objectPositions.InsertRange(0, mom.objectPositions.GetRange(0, platformPosMom));

        child1.LHS = child1.LHS.Remove(child1.LHS.IndexOf('E') + 1, child1.LHS.IndexOf('E') + fruitPosDad + 1);
        child1.LHS = child1.LHS.Insert(child1.LHS.IndexOf('E') + 1, mom.LHS.Substring(mom.LHS.IndexOf('E') + 1, mom.LHS.IndexOf('E') + fruitPosMom));
        child1.objectPositions.RemoveRange(child1.LHS.IndexOf('E') + 1, child1.LHS.IndexOf('E') + fruitPosDad + 1);
        child1.objectPositions.InsertRange(child1.LHS.IndexOf('E') + 1, mom.objectPositions.GetRange(mom.LHS.IndexOf('E') + 1, mom.LHS.IndexOf('E') + fruitPosMom));
    }

    void CrossOver(Level dad, Level mom) //Uniform
    {
        int platformPos1 = Random.Range(1, dad.LHS.IndexOf('E'));
        while(dad.LHS[platformPos1] == 'b')
            platformPos1 = Random.Range(1, dad.LHS.IndexOf('E'));

        int platformPos2 = Random.Range(1, mom.LHS.IndexOf('E'));
        while (mom.LHS[platformPos2] == 'b')
            platformPos2 = Random.Range(1, mom.LHS.IndexOf('E'));

        Level child1 = SwitchGenes(dad, mom, platformPos1, platformPos2);
        Level child2 = SwitchGenes(mom, dad, platformPos2, platformPos1);

        generatedLevels.Add(child1);
        generatedLevels.Add(child2);
    }

    Level SwitchGenes(Level parent1, Level parent2, int childIndex, int parentIndex)
    {
        Level child = parent1;

        int removeLength = 1;
        while (child.LHS[childIndex + removeLength] == 'b')
            removeLength++;

        int addLength = 1;
        while (parent2.LHS[parentIndex + addLength] == 'b')
            addLength++;

        child.LHS = child.LHS.Remove(childIndex, removeLength);
        child.LHS = child.LHS.Insert(childIndex, parent2.LHS.Substring(parentIndex, addLength));
        child.objectPositions.RemoveRange(childIndex, removeLength);
        child.objectPositions.InsertRange(childIndex, parent2.objectPositions.GetRange(parentIndex, addLength));

        return child;
    }

    List<Level> TournamentSelection()
    {
        List<Level> bestPopulation = new List<Level>();

        while(generatedLevels.Count != 0)
        {
            int randomPos1 = Random.Range(0, generatedLevels.Count);

            Level temp1 = generatedLevels[randomPos1];
            generatedLevels.RemoveAt(randomPos1);

            int randomPos2 = Random.Range(0, generatedLevels.Count);
            while (randomPos2 == randomPos1)
            {
                if (generatedLevels.Count == 1)
                    break;
                randomPos2 = Random.Range(0, generatedLevels.Count);
            }

            Level temp2 = generatedLevels[randomPos2];
            generatedLevels.RemoveAt(randomPos2);

            bestPopulation.Add(CompareStrongestLevel(temp1, temp2));
        }

        return bestPopulation;
    }

    Level CompareStrongestLevel(Level first, Level second)
    {
        if (first.fitness > second.fitness)
            return first;

        return second;
    }

    public void ClearLevels()
    {
        generatedLevels.Clear();
    }
}
