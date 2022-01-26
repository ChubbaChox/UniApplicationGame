using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class People
{
    [SerializeField] PeopleTemplate template;
    [SerializeField] int level;
 
    public PeopleTemplate Template
    {
        get
        {
            return template;
        }
    }
    public int Level
    {
        get
        {
            return level;
        }
    }

    public event System.Action OnHeal;
    public int Xp { get; set; }
    public int HP { get; set; }

    public List<CombatMove> CombatMoves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();

    public void Init()
    {
        CombatMoves = new List<CombatMove>();
        foreach (var combatMove in Template.LearnableCombatMoves)
        {
            if (combatMove.Level <= Level)
                CombatMoves.Add(new CombatMove(combatMove.Template));
            // Generate CombatMoves

            if (CombatMoves.Count >= 4)
                break;
        }
        Xp = Template.GetXpForLevel(Level);
    

        CalculateStats();
        HP = MaxHp;

    
        ResetStatBoost();
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Template.Attack * Level) / 100f) + 10);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Template.Defense * Level) / 175f) + 5);
        Stats.Add(Stat.StrongAttack, Mathf.FloorToInt((Template.StrongAttack * Level) / 100f) + 15); //works out stats based on level, allows for scaling
        Stats.Add(Stat.Agility, Mathf.FloorToInt((Template.Agility * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Template.MaxHp * Level) / 30f) + 10;

    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.StrongAttack, 0},
            {Stat.Agility, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        // Apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1.5f, 2f, 2,5f, 3f, 3.5f, 4f, 4.5f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 4);

            if (boost > 0)
                StatusChanges.Enqueue($"{Template.Name}  has increased their {stat}");
            else
                StatusChanges.Enqueue($"{Template.Name} has lost some {stat}");

            Debug.Log($"{stat} has changed to {StatBoosts[stat]}");

        }
    }

    public bool CheckForLevelUp()
    {
        if (Xp > Template.GetXpForLevel(level + 1))
        {
            ++level;
            CalculateStats();
            Heal(10); //small heal after each level up
            return true;
        }

        return false;
    }

   

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int StrongAttack
    {
        get { return GetStat(Stat.StrongAttack); }
    }

    public int Agility
    {
        get
        {
            return GetStat(Stat.Agility);
        }
    }

    public int MaxHp { get; private set; }

    public DamageDetails TakeDamage(CombatMove combatMove, People attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 4.5f)
            critical = 2f;

        float classSet = ClassSetChart.GetEffectiveness(combatMove.Template.ClassSet, this.Template.ClassSet1) * ClassSetChart.GetEffectiveness(combatMove.Template.ClassSet, this.Template.ClassSet2);

        var damageDetails = new DamageDetails()
        {
            ClassSetEffectiveness = classSet,
            Critical = critical,
            Wounded = false
        };

        float attack = (combatMove.Template.Category == CombatMoveCategory.StrongAttackType) ? attacker.StrongAttack : attacker.Attack;
        float defense = (combatMove.Template.Category == CombatMoveCategory.StrongAttackType) ? Defense : Defense;

        float modifiers = Random.Range(0.80f, 1f) * classSet * critical;
        float a = (2 * attacker.Level + 5) / 250f;
        float d = a * combatMove.Template.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Wounded = true;
        }

        return damageDetails;
    }
    
    public void Heal(int amount)
    {
        
        HP = Mathf.Clamp(HP + amount, 0, MaxHp);
        
    }

    public CombatMove GetRandomCombatMove()
    {
        int r = Random.Range(0, CombatMoves.Count);
        return CombatMoves[r];
    }

    public void OnAmbushedOver()
    {
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public float ClassSetEffectiveness { get; set; }
    public float Critical { get; set; }
    public bool Wounded { get; set; }
}

