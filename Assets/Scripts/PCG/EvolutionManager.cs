using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{

    struct Level
    {
        internal string LHS;
        internal List<Vector2> objectPositions;
        internal float fitness;
    }

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
        level.objectPositions = new List<Vector2>();
        foreach (GameObject o in list)
            level.objectPositions.Add(o.transform.position);
        level.fitness = fitness;

        generatedLevels.Add(level);
    }
}
