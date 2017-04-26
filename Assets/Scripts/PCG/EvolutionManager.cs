using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public string LHS;
    public List<Vector3> objectPositions;
    public List<Quaternion> objectRotations;
    public float fitness;

    public Level()
    {
        fitness = 0;
    }
}

public class EvolutionManager : MonoBehaviour
{
    GE_LevelGenerator geGenerator;
    List<Level> generatedLevels = new List<Level>();
    char[] vegetables = new char[3];
    private int populationSize;

    // Use this for initialization
    void Start()
    {
        vegetables[0] = 'C';
        vegetables[1] = 'T';
        vegetables[2] = 'B';
        populationSize = 100;
        geGenerator = gameObject.GetComponent<GE_LevelGenerator>();
    }

    void Awake()
    {
        populationSize = 100;
        geGenerator = gameObject.GetComponent<GE_LevelGenerator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SaveLevelToTxt(int index)
    {
        //System.IO.File.WriteAllText("C:/Users/Datorlabbet/Documents/Madame-Legume-PCG/Assets/Levels/LevelTxt/level.txt", 
        //    generatedLevels[index].LHS + ", " + generatedLevels[index].objectPositions.ToString() + ", " + generatedLevels[index].objectRotations.ToString());

        const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789"; //add the characters you want
        string levelID = "";
        int charAmount = Random.Range(0, glyphs.Length); //set those to the minimum and maximum length of your string
        for (int i = 0; i < 10; i++)
        {
            levelID += glyphs[Random.Range(0, glyphs.Length)];
        }

        System.IO.StreamWriter saveFile = new System.IO.StreamWriter(Application.dataPath + "/Levels/LevelTxt/level_" + levelID + ".txt", false);

        saveFile.WriteLine(generatedLevels[index].LHS);
        saveFile.WriteLine(",");
        foreach (Vector3 v in generatedLevels[index].objectPositions)
        {
            saveFile.WriteLine(v);
        }
        saveFile.WriteLine("|");
        foreach (Quaternion q in generatedLevels[index].objectRotations)
        {
            saveFile.WriteLine(q);
        }
        saveFile.Close();
    }

    public void InsertLevelData(string LHS, List<GameObject> list, float fitness)
    {
        Level level = new Level();
        level.LHS = LHS;
        level.objectPositions = new List<Vector3>();
        level.objectRotations = new List<Quaternion>();
        foreach (GameObject o in list)
        {
            level.objectPositions.Add(o.transform.position);
            level.objectRotations.Add(o.transform.rotation);
        }
        level.fitness = fitness;

        generatedLevels.Add(level);
    }

    static int SortByFitness(Level l1, Level l2)
    {
        return l1.fitness.CompareTo(l2.fitness);
    }

    public Level CreateBestLevel()
    {
        if (generatedLevels.Count > populationSize)
        {
            generatedLevels.Sort(SortByFitness);
            generatedLevels.Reverse();
            generatedLevels.RemoveRange(populationSize, generatedLevels.Count - populationSize);

            return generatedLevels[0];
        }


        //generatedLevels = TournamentSelection();
        CreateNewPopulation();
        for (int i = 0; i < generatedLevels.Count; ++i) //recalculate fitness for every level
            generatedLevels[i].fitness = gameObject.GetComponent<GE_LevelGenerator>().GetCandiateFitness(generatedLevels[i]);


        generatedLevels.Sort(SortByFitness);
        generatedLevels.Reverse();

        Level best = generatedLevels[0];
        return best;
    }

    public Level GetLevel(int index)
    {
        Level level = generatedLevels[index];
        return level;
    }

    void CreateNewPopulation()
    {
        #region Old Tournamentsystem
        List<Level> oldPopulation = new List<Level>();
        oldPopulation.AddRange(generatedLevels);
        generatedLevels.RemoveRange(10, 90);
        List<int> tournamentList = new List<int>();

        while (generatedLevels.Count < populationSize)
        {
            tournamentList.Clear();

            for (int i = 0; i < 8; ++i)
            {
                int randomPos = Random.Range(0, oldPopulation.Count);
                while(tournamentList.Contains(randomPos)) //Stop same level from entering tournament 
                    randomPos = Random.Range(0, oldPopulation.Count);
                tournamentList.Add(randomPos); //Adds random element from oldPopulation
            }

            TournamentSelection(ref tournamentList, ref oldPopulation);

            int crossoverChance = 7; //70%
            if (crossoverChance > Random.Range(1, 11))
            {
                OnePointCrossover(oldPopulation[tournamentList[0]], oldPopulation[tournamentList[1]]);
                if(tournamentList[0] < tournamentList[1]) //If tournamentlist[1] will move one stpa back after tournamenstlist[0] is deleted
                {
                    oldPopulation.Remove(oldPopulation[tournamentList[0]]);
                    oldPopulation.Remove(oldPopulation[tournamentList[1] - 1]);
                }
                else
                {
                    oldPopulation.Remove(oldPopulation[tournamentList[0]]);
                    oldPopulation.Remove(oldPopulation[tournamentList[1]]);
                }

            }
            else
            {
                generatedLevels.Add(oldPopulation[tournamentList[0]]);
                generatedLevels.Add(oldPopulation[tournamentList[1]]);
                if (tournamentList[0] < tournamentList[1])
                {
                    oldPopulation.Remove(oldPopulation[tournamentList[0]]);
                    oldPopulation.Remove(oldPopulation[tournamentList[1] - 1]);
                }
                else
                {
                    oldPopulation.Remove(oldPopulation[tournamentList[0]]);
                    oldPopulation.Remove(oldPopulation[tournamentList[1]]);
                }
            }
        }
        //for (int i = 0; i < 8; i++) //Create 20 new levels each iteration
        //    geGenerator.GenerateLevel();
        #endregion

        #region SwitchRegion

        //generatedLevels.Sort(SortByFitness);
        //generatedLevels.Reverse();
        //generatedLevels.RemoveRange(40, 60); //Save top 2/5 of population

        //for (int i = 0; i < 40; i += 2) //Adds 4 children per loop. 2 from crossover, and 2 more after crossover and mutation
        //    OnePointCrossover(generatedLevels[i], generatedLevels[i + 1]);


        //for (int i = 0; i < 20; i++) //Create 20 new levels each iteration
        //    geGenerator.GenerateLevel();
        #endregion
    }

    //void TwoPointCrossover(Level dad, Level mom)
    //{
    //    int platformPosDad = Random.Range(0, dad.LHS.IndexOf('E'));
    //    int platformPosMom = Random.Range(0, mom.LHS.IndexOf('E'));

    //    int fruitPosDad = Random.Range(dad.LHS.IndexOf('E') + 1, dad.LHS.Length) - dad.LHS.IndexOf('E');
    //    int fruitPosMom = Random.Range(mom.LHS.IndexOf('E') + 1, mom.LHS.Length) - mom.LHS.IndexOf('E');

    //    Level child1 = dad;
    //    child1.LHS = child1.LHS.Remove(0, platformPosDad + 1);
    //    child1.LHS = child1.LHS.Insert(0, mom.LHS.Substring(0, platformPosMom));
    //    child1.objectPositions.RemoveRange(0, platformPosDad + 1);
    //    child1.objectPositions.InsertRange(0, mom.objectPositions.GetRange(0, platformPosMom));

    //    child1.LHS = child1.LHS.Remove(child1.LHS.IndexOf('E') + 1, child1.LHS.IndexOf('E') + fruitPosDad + 1);
    //    child1.LHS = child1.LHS.Insert(child1.LHS.IndexOf('E') + 1, mom.LHS.Substring(mom.LHS.IndexOf('E') + 1, mom.LHS.IndexOf('E') + fruitPosMom));
    //    child1.objectPositions.RemoveRange(child1.LHS.IndexOf('E') + 1, child1.LHS.IndexOf('E') + fruitPosDad + 1);
    //    child1.objectPositions.InsertRange(child1.LHS.IndexOf('E') + 1, mom.objectPositions.GetRange(mom.LHS.IndexOf('E') + 1, mom.LHS.IndexOf('E') + fruitPosMom));
    //} // not working, not used. YET

    void OnePointCrossover(Level dad, Level mom)
    {
        int dadCrossoverPoint = dad.LHS.Length / 2;
        while (dad.LHS[dadCrossoverPoint] == 'b')
            dadCrossoverPoint--;

        int momCrossoverPoint = mom.LHS.Length / 2;
        while (mom.LHS[momCrossoverPoint] == 'b')
            momCrossoverPoint--;

        Level child1 = new Level();
        child1.objectPositions = new List<Vector3>();
        child1.objectRotations = new List<Quaternion>();

        child1.LHS = dad.LHS.Substring(0, dadCrossoverPoint);
        child1.LHS = child1.LHS.Insert(dadCrossoverPoint, mom.LHS.Substring(momCrossoverPoint));
        child1.objectPositions.AddRange(dad.objectPositions.GetRange(0, dadCrossoverPoint));
        child1.objectPositions.AddRange(mom.objectPositions.GetRange(momCrossoverPoint, mom.objectPositions.Count - momCrossoverPoint));
        child1.objectRotations.AddRange(dad.objectRotations.GetRange(0, dadCrossoverPoint));
        child1.objectRotations.AddRange(mom.objectRotations.GetRange(momCrossoverPoint, mom.objectRotations.Count - momCrossoverPoint));

        Level child2 = new Level();
        child2.objectPositions = new List<Vector3>();
        child2.objectRotations = new List<Quaternion>();

        child2.LHS = mom.LHS.Substring(0, momCrossoverPoint);
        child2.LHS = child2.LHS.Insert(momCrossoverPoint, dad.LHS.Substring(dadCrossoverPoint));
        child2.objectPositions.AddRange(mom.objectPositions.GetRange(0, momCrossoverPoint));
        child2.objectPositions.AddRange(dad.objectPositions.GetRange(dadCrossoverPoint, dad.objectPositions.Count - dadCrossoverPoint));
        child2.objectRotations.AddRange(mom.objectRotations.GetRange(0, momCrossoverPoint));
        child2.objectRotations.AddRange(dad.objectRotations.GetRange(dadCrossoverPoint, dad.objectRotations.Count - dadCrossoverPoint));


        if (Random.Range(0, 2) == 0) //Mutates one and only does crossover on the other
        {
            generatedLevels.Add(child1);

            child2 = Mutation(child2);
            generatedLevels.Add(child2);
        }
        else
        {
            generatedLevels.Add(child2);

            child1 = Mutation(child1);
            generatedLevels.Add(child1);
        }

        //generatedLevels.Add(child1); //Add second lot with mutation
        //generatedLevels.Add(child2);
    }

    void CrossOver(Level dad, Level mom) //Uniform
    {
        int platformPos1 = Random.Range(1, dad.LHS.IndexOf('E'));
        while (dad.LHS[platformPos1] == 'b')
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
        Level child = new Level(); //Set first child to fathers' genes
        child.LHS = parent1.LHS;
        child.objectPositions = new List<Vector3>();
        child.objectRotations = new List<Quaternion>();
        for (int i = 0; i < parent1.objectPositions.Count; ++i) //Need to manually add them because else they reference to the same thing....
        {
            child.objectPositions.Add(parent1.objectPositions[i]);
            child.objectRotations.Add(parent1.objectRotations[i]);
        }

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
        child.objectRotations.RemoveRange(childIndex, removeLength);
        child.objectRotations.InsertRange(childIndex, parent2.objectRotations.GetRange(parentIndex, addLength));

        return child;
    }

    void TournamentSelection(ref List<int> tournamentPopulation, ref List<Level> oldPopulation)
    {
        List<int> semiFinal = new List<int>();

        semiFinal.Add(CompareStrongestLevel(tournamentPopulation[0], tournamentPopulation[1], oldPopulation)); //Adds4 best to semifinal
        semiFinal.Add(CompareStrongestLevel(tournamentPopulation[2], tournamentPopulation[3], oldPopulation));
        semiFinal.Add(CompareStrongestLevel(tournamentPopulation[4], tournamentPopulation[5], oldPopulation));
        semiFinal.Add(CompareStrongestLevel(tournamentPopulation[6], tournamentPopulation[7], oldPopulation));

        tournamentPopulation.Clear(); //Clear old list

        tournamentPopulation.Add(CompareStrongestLevel(semiFinal[0], semiFinal[1], oldPopulation)); //Add the two best
        tournamentPopulation.Add(CompareStrongestLevel(semiFinal[2], semiFinal[3], oldPopulation));
    }

    Level CompareStrongestLevel(Level first, Level second)
    {
        if (first.fitness == 0)
            return second;
        if (second.fitness == 0)
            return first;

        if (first.fitness > second.fitness)
            return first;

        return second;
    }

    int CompareStrongestLevel(int first, int second, List<Level> population)
    {
        if (population[first].fitness == 0)
            return second;
        if (population[second].fitness == 0)
            return first;

        if (population[first].fitness > population[second].fitness)
            return first;

        return second;
    }

    Level Mutation(Level child)
    {
        int mutationChance = 1;
        int randomInt = Random.Range(0, 11);

        if (mutationChance >= randomInt) //Move random object
        {
            int randomObject = Random.Range(0, child.objectPositions.Count);
            while (child.LHS[randomObject] == 'b')
                randomObject = Random.Range(0, child.objectPositions.Count);

            int range = 1;
            if (randomObject + 1 != child.LHS.Length) //Safety check incase randomObject is end platform
            {
                while (child.LHS[randomObject + range] == 'b') //Gets value of how many blades need to be moved as well if needed
                    range++;
            }

            Vector3 randomPos = new Vector3(Random.Range(0, 6) - 3, Random.Range(0, 6) - 3);
            for (int i = 0; i < range; ++i)
                child.objectPositions[randomObject + i] += randomPos;
        }

        #region Add/Remove fruits and platforms
        //randomInt = Random.Range(0, 11);
        //if (mutationChance >= randomInt) //Add Platform
        //{
        //    int randomPlatform = Random.Range(1, 3);
        //    child.LHS = child.LHS.Insert(1, randomPlatform.ToString()); // Position 1 in front of start
        //    child.objectPositions.Insert(1, gameObject.GetComponent<GE_LevelGenerator>().getRandomInstatiatePosition(randomPlatform == 1 ? '1' : '2'));
        //    child.objectRotations.Insert(1, new Quaternion(0, 0, 0, 0));
        //}

        //randomInt = Random.Range(0, 11);
        //if (mutationChance >= randomInt) //Remove Platform
        //{
        //    int firstFruit = child.LHS.IndexOfAny(vegetables, 0);

        //    if (firstFruit == -1)
        //        firstFruit = child.LHS.Length - 1;

        //    int platformPos = Random.Range(1, firstFruit);
        //    int range = 1;
        //    if (platformPos + 1 != child.LHS.Length) //Safety check incase platformPos is end platform. Dunno how it could happen...
        //    {
        //        if (child.LHS[platformPos + range] == 'b')
        //            range++;

        //        child.LHS = child.LHS.Remove(platformPos, range);
        //        child.objectPositions.RemoveRange(platformPos, range);
        //        child.objectRotations.RemoveRange(platformPos, range);
        //    }
        //}

        //randomInt = Random.Range(0, 11);
        //if (mutationChance >= randomInt) //Add Vegetable
        //{
        //    int randomFruit = Random.Range(0, 3);

        //    string fruit = "C";
        //    if (randomFruit == 0)
        //        fruit = "C";
        //    if (randomFruit == 1)
        //        fruit = "T";
        //    if (randomFruit == 1)
        //        fruit = "B";

        //    child.LHS = child.LHS.Insert(child.LHS.Length - 1, fruit); // -1, add fruit in front of End
        //    child.objectPositions.Insert(child.objectPositions.Count - 2, gameObject.GetComponent<GE_LevelGenerator>().getRandomInstatiatePosition('1'));
        //    child.objectRotations.Insert(child.objectRotations.Count - 2, new Quaternion(0, 0, 0, 0));
        //}

        //randomInt = Random.Range(0, 11);
        //if (mutationChance >= randomInt) //Remove Vegetable
        //{
        //    int firstFruit = child.LHS.IndexOfAny(vegetables, 1);

        //    if (firstFruit == -1)
        //        return child;

        //    int fruitPos = Random.Range(firstFruit, child.LHS.Length - 1);

        //    child.LHS = child.LHS.Remove(fruitPos, 1);
        //    child.objectPositions.RemoveAt(fruitPos);
        //    child.objectRotations.RemoveAt(fruitPos);
        //}
        #endregion

        return child;
    }

    public void ClearLevels()
    {
        generatedLevels.Clear();
    }

    public int GetSize()
    {
        return generatedLevels.Count;
    }
}
