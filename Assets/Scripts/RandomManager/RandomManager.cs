using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class RandomManager : MonoSingleton<RandomManager>
{
    private Random random;
    private int randomCount;
    private int seed;

    public int Seed => seed;
    public int RandomCount => randomCount;

    public void Initialize(int seed, int randomCount)
    {
        Debug.Log($"Initializing RandomManager with seed: {seed} and randomCount: {randomCount}");
        this.seed = seed;
        this.randomCount = randomCount;
        random = new Random((uint)seed);
        for (int i = 0; i < randomCount; i++)
        {
            random.NextInt();
        }
    }

    public int GetRandomInt(int min, int max)
    {
        randomCount++;
        return random.NextInt(min, max);
    }

    public int GetRandomInt()
    {
        randomCount++;
        return random.NextInt();
    }

    public float GetRandomFloat(float min, float max)
    {
        randomCount++;
        return random.NextFloat(min, max);
    }
}
