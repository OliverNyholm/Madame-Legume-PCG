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
        generatedLevels.Sort(SortByFitness);
        generatedLevels.Reverse();
        

        Level best = generatedLevels[0];
        return best;
    }

    public void ClearLevels()
    {
        generatedLevels.Clear();
    }
}
