using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{
    public static FlowerSpawner Instance;
    
    public Transform flowerContainer;
    public Transform redFlower;
    public Transform blueFlower;

    private void Awake() => Instance = this;

    public void SpawnRedFlower(Vector2 position)
    {
        var newRedFlower = Instantiate(redFlower, position, Quaternion.identity);
        newRedFlower.SetParent(flowerContainer, true);
    }
    
    public void SpawnBlueFlower(Vector2 position)
    {
        var newBlueFlower = Instantiate(blueFlower, position, Quaternion.identity);
        newBlueFlower.SetParent(flowerContainer, true);
    }
}
