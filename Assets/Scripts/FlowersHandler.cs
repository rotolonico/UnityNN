using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowersHandler : MonoBehaviour
{
    public static FlowersHandler Instance;
    
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

    public void RemoveFlower(Vector3 mousePosition)
    {
        var flower = Physics2D.OverlapCircle(mousePosition, 0.01f);
        if (flower != null && (flower.CompareTag("RedFlower") || flower.CompareTag("BlueFlower"))) Destroy(flower.gameObject);
    }

    public void ClearFlowers()
    {
        foreach (var flower in GameObject.FindGameObjectsWithTag("RedFlower")) Destroy(flower);
        foreach (var flower in GameObject.FindGameObjectsWithTag("BlueFlower")) Destroy(flower);
    }
}
