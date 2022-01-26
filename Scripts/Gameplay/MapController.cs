using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] List<People> ambushPeoples;

    public People GetRandomAmbusherPeople()
    {
        var ambushPeople = ambushPeoples[Random.Range(0, ambushPeoples.Count)];
        ambushPeople.Init();
        return ambushPeople;
    }
}