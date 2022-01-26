using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PeopleCompanions : MonoBehaviour
{
    [SerializeField] List<People> peoples;

    public List<People> Peoples
    {
        get
        {
            return peoples;
        }
    }

    private void Start()
    {
        foreach (var people in peoples)
        {
            people.Init();
        }
    }

    public People GetHealthyPeople()
    {
        return peoples.Where(x => x.HP > 0).FirstOrDefault();
    }



}
