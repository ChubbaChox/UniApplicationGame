using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "People", menuName = "new peopleTemplate")]//allowing me to create instances of the class, in this case my NPCs 

public class PeopleTemplate : ScriptableObject//ScriptableObject allows me to store data that I can later call back on for such as the stats of a character 
{
   
    [SerializeField] string name;//due to ScriptableObject I can use SerializeField which makes the variables usable outside of this class without making a load of public variables

    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite frontSprite;//later I can apply chosen sprites into the inspector so when a battle begins it will choose said sprites

    [SerializeField] PeopleClassSet classSet1;
    [SerializeField] PeopleClassSet classSet2;
    // Template Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int strongAttack;
    [SerializeField] int agility;
    [SerializeField] int xpAvailable;
    [SerializeField] XpGainRate xpGainRate;

    [SerializeField] List<LearnableCombatMove> learnableCombatMoves;//list of learnable combatMoves

    public string Name//creating a property 
    {
        get { return name; }//using a get, whenever the value of the property is needed it will get the value of the name variable in this case
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public PeopleClassSet ClassSet1
    {
        get { return classSet1; }
    }

    public PeopleClassSet ClassSet2
    {
        get { return classSet2; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int StrongAttack
    {
        get { return strongAttack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int Agility
    {
        get { return agility; }
    }

    public int GetXpForLevel(int level)
    {
       
        if (xpGainRate == XpGainRate.Standard)
        {
            return level * level * level; 
        } 

        return -1;
    }

    

    public List<LearnableCombatMove> LearnableCombatMoves//property for the learnable combatMoves
    {
        get { return learnableCombatMoves; }
    }
    public int XpAvailable => xpAvailable;

    public XpGainRate XpGainRate => xpGainRate;
}

[System.Serializable]//allows learnable combatMoves to appear in the inspector
public class LearnableCombatMove//class to make it so that you have to be certain levels to learn combatMoves
{
    [SerializeField] CombatMoveTemplate combatMoveTemplate; //reference to combatMovebase and level
    [SerializeField] int level;

    public CombatMoveTemplate Template//property to get their values
    {
        get { return combatMoveTemplate; }
    }

    public int Level
    {
        get { return level; }
    }
}



public enum Stat
{
    Attack,
    StrongAttack,
    Defense,
    Agility,
   
    Accuracy,
    Evasion,

}

public enum XpGainRate
{
    Standard
}

public enum PeopleClassSet//using enum to create named constants
{
    None,
    Melee,
    Blade,
    Gunner,
    Bow,
    Shield,
    Pike,
    Spear,
    Bomb

}

public class ClassSetChart//a chart for the effectiveness of each class vs eachother. 
{
    static float[][] chart =
    {
        //                  MEL   BLA   GUN   BOW   SHI   PIK   SPE   BOM           
        /*MEL*/ new float[] {1f,   1f,   1f,   1f,   0.5f, 0.5f, 0.5f, 1.5f },
        /*BLA*/ new float[] {1.5f, 0.5f, 0.5f, 1f,   2f,   0.5f, 1f,   1.5f },
        /*GUN*/ new float[] {1f,   2f,   0.5f, 2f,   0.5f, 2f,   1.5f, 2f },
        /*BOW*/ new float[] {1.5f, 1.5f, 1f,   0.5f, 0.5f, 1.5f, 1f,   1.5f },
        /*SHI*/ new float[] {1f,   0.5f, 2f,   2f,   0.5f, 2f,   1.5f, 1.5f },
        /*PIK*/ new float[] {2f,   2f,   0.5f, 0.5f, 0.5f, 0.5f, 1.5f, 1.5f }, 
        /*SPE*/ new float[] {1.5f, 1,5f, 0.5f, 1f,   1f,   1f,   0.5f, 2f },
        /*BOM*/ new float[] {2f,   2f,   1f,   1f,   1.5f, 2f,   1.5f, 0.5f }

    };

    public static float GetEffectiveness(PeopleClassSet attackClassSet, PeopleClassSet defenseClassSet)
    {
        if (attackClassSet == PeopleClassSet.None || defenseClassSet == PeopleClassSet.None)
            return 1;

        int row = (int)attackClassSet - 1;
        int col = (int)defenseClassSet - 1;

        return chart[row][col];
    }
}
