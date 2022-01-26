using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatMove", menuName = "People/CombatMove")]
public class CombatMoveTemplate : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] PeopleClassSet classSet;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int stamina;
    [SerializeField] CombatMoveCategory category;
    [SerializeField] CombatMoveEffects effects;
    [SerializeField] CombatMoveTarget target;

    public string Name
    {
        get { return name; }
    }

    public PeopleClassSet ClassSet
    {
        get { return classSet; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }

    public int Stamina
    {
        get { return stamina; }
    }

    public CombatMoveCategory Category
    {
        get { return category; }
    }

    public CombatMoveEffects Effects
    {
        get { return effects; }
    }

    public CombatMoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class CombatMoveEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum CombatMoveCategory
{
    Physical, StrongAttackType, Status,
}

public enum CombatMoveTarget
{
    Opponent, Self
}
