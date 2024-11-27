using CodeMonkey;
using System.Collections;
using UnityEngine;

public class BloodPoolSpawner : MonoBehaviour
{
    public enum BloodPoolSize
    {
        SMALL,
        MEDIUM,
        LARGE
    }

    public static void SpawnBloodPool(Vector2 position, BloodPoolSize size)
    {
        GameObject chosenBloodPool = null;
        switch (size)
        {
            case BloodPoolSize.LARGE:
                chosenBloodPool = Assets.i.largeBloodPools[Random.Range(0, Assets.i.largeBloodPools.Length)];
                break;
            case BloodPoolSize.MEDIUM:
				chosenBloodPool = Assets.i.mediumBloodPools[Random.Range(0, Assets.i.mediumBloodPools.Length)];
				break;
            default: // SMALL
				chosenBloodPool = Assets.i.smallBloodPools[Random.Range(0, Assets.i.smallBloodPools.Length)];
				break;
        }

        Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));


        GameObject spawnedPool = Instantiate(chosenBloodPool, position, rotation);
        // blood pool prefab will handle scaling and fading
    }
}
