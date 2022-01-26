using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatState { Start, ActionSelection, CombatMoveSelection, PerformCombatMove, Busy, CompanionsMenu, CombatOver }

public class CombatSystem : MonoBehaviour
{
    [SerializeField] CombatUnit playerUnit;
    [SerializeField] CombatUnit enemyUnit;
    [SerializeField] CombatDialogBox dialogBox;
    [SerializeField] CompanionsMenu companionsMenu;

    public event Action<bool> OnAmbushedOver;

    CombatState state;
    int currentAction;
    int currentCombatMove;
    int currentMember;

    PeopleCompanions playerCompanions;
    PeopleCompanions bossCompanions;
    People ambushPeople;

    bool isBossCombat = false;
    PlayerController player;
    int escapeAttempts;
    BoxDetect boss;

    public void BeginFight(PeopleCompanions playerCompanions, People ambushPeople)
    {
        this.playerCompanions = playerCompanions;
        this.ambushPeople = ambushPeople;
        StartCoroutine(SetupCombat());
    }

    public void BeginBossFight(PeopleCompanions playerCompanions, PeopleCompanions bossCompanions)
    {
        this.playerCompanions = playerCompanions;
        this.bossCompanions = bossCompanions;
        isBossCombat = true;
        player = playerCompanions.GetComponent<PlayerController>(); //begins to set up a boss battle instead of ambush, passes the boolean as true
        boss = bossCompanions.GetComponent<BoxDetect>();
        StartCoroutine(SetupCombat());
    }

    public IEnumerator SetupCombat()
    {

        if (!isBossCombat)
        {
            playerUnit.Setup(playerCompanions.GetHealthyPeople());
            enemyUnit.Setup(ambushPeople); //if its not a boss triggered battle then set up a ambush battle
            dialogBox.SetCombatMoveNames(playerUnit.People.CombatMoves);

            yield return dialogBox.TypeDialog($"Ambush! A {enemyUnit.People.Template.Name} approaches.");
        }
        else
        {
            enemyUnit.gameObject.SetActive(true);
            var enemyPeople = bossCompanions.GetHealthyPeople();
            enemyUnit.Setup(enemyPeople);
            yield return dialogBox.TypeDialog($"An enemy {enemyPeople.Template.Name}"); //if it is a boss triggered battle then load the other set of enemies under the bossCompanions list

            playerUnit.gameObject.SetActive(true);
            var playerPeople = playerCompanions.GetHealthyPeople();
            playerUnit.Setup(playerPeople);
            yield return dialogBox.TypeDialog($"Charge {playerPeople.Template.Name}");
            dialogBox.SetCombatMoveNames(playerUnit.People.CombatMoves);
        }
       

        companionsMenu.Init();

       

        ChooseFirstTurn();
        escapeAttempts = 0;
    }

    void ChooseFirstTurn()
    {
        if (playerUnit.People.Agility >= enemyUnit.People.Agility)
            ActionSelection();
        else
            StartCoroutine(EnemyCombatMove());
    }

    void CombatOver(bool won)
    {
        state = CombatState.CombatOver;
        playerCompanions.Peoples.ForEach(p => p.OnAmbushedOver());
        OnAmbushedOver(won);
    }

    void ActionSelection()
    {
        state = CombatState.ActionSelection;
        dialogBox.SetDialog("Well, what's your plan?");
        dialogBox.EnableActionSelector(true);
    }

    void OpenCompanionsMenu()
    {
        state = CombatState.CompanionsMenu;
        companionsMenu.SetCompanionsData(playerCompanions.Peoples);
        companionsMenu.gameObject.SetActive(true);
    }

    void CombatMoveSelection()
    {
        state = CombatState.CombatMoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableCombatMoveSelector(true);
    }

    IEnumerator PlayerCombatMove()
    {
        state = CombatState.PerformCombatMove;

        var combatMove = playerUnit.People.CombatMoves[currentCombatMove];
        yield return RunCombatMove(playerUnit, enemyUnit, combatMove);

        // If the battle stat was not changed by RunCombatMove, then go to next step
        if (state == CombatState.PerformCombatMove)
            StartCoroutine(EnemyCombatMove());
    }

    IEnumerator EnemyCombatMove()
    {
        state = CombatState.PerformCombatMove;

        var combatMove = enemyUnit.People.GetRandomCombatMove();
        yield return RunCombatMove(enemyUnit, playerUnit, combatMove);

        // If the battle stat was not changed by RunCombatMove, then go to next step
        if (state == CombatState.PerformCombatMove)
            ActionSelection();
    }

    IEnumerator RunCombatMove(CombatUnit sourceUnit, CombatUnit targetUnit, CombatMove combatMove)
    {
        combatMove.Stamina--;
        yield return dialogBox.TypeDialog($"{sourceUnit.People.Template.Name} used {combatMove.Template.Name}");

        if (CheckIfCombatMoveHits(combatMove, sourceUnit.People, targetUnit.People))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            if (combatMove.Template.Category == CombatMoveCategory.Status)
            {
                yield return RunCombatMoveEffects(combatMove, sourceUnit.People, targetUnit.People);
            }
            else
            {
                var damageDetails = targetUnit.People.TakeDamage(combatMove, sourceUnit.People);
                yield return targetUnit.Combatui.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (targetUnit.People.HP <= 0)
            {
                yield return HandlePeopleWounded(targetUnit);

                CheckForCombatOver(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.People.Template.Name}'s attack missed");
        }
    }

    bool CheckIfCombatMoveHits(CombatMove combatMove, People source, People target)
    {
        if (combatMove.Template.AlwaysHits)
            return true;

        float combatMoveAccuracy = combatMove.Template.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            combatMoveAccuracy *= boostValues[accuracy];
        else
            combatMoveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            combatMoveAccuracy /= boostValues[evasion];
        else
            combatMoveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= combatMoveAccuracy;
    }

    IEnumerator RunCombatMoveEffects(CombatMove combatMove, People source, People target)
    {
        var effects = combatMove.Template.Effects;
        if (effects.Boosts != null)
        {
            if (combatMove.Template.Target == CombatMoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator ShowStatusChanges(People people)
    {
        while (people.StatusChanges.Count > 0)
        {
            var message = people.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }
  
    IEnumerator HandlePeopleWounded(CombatUnit woundedUnit)
    {
        yield return dialogBox.TypeDialog($"{woundedUnit.People.Template.Name} Has Been Wounded");
        woundedUnit.PlayWoundedAnimation();
        yield return new WaitForSeconds(2f);

        if (!woundedUnit.IsPlayerUnit)
        {
            // Xp Gain
            int xpAvailable = woundedUnit.People.Template.XpAvailable;
            int enemyLevel = woundedUnit.People.Level;
           

            int xpGain = Mathf.FloorToInt((xpAvailable * enemyLevel) / 7);
            playerUnit.People.Xp += xpGain;
            yield return dialogBox.TypeDialog($"{playerUnit.People.Template.Name} gained {xpGain}xp");
            yield return playerUnit.Combatui.SetXpSmooth();
           

            // Check Level Up
            while (playerUnit.People.CheckForLevelUp())//using while instead of if as this allows for multiple level ups if player gains enough xp as the script is run until CheckForLevelUp=false
            {

                playerUnit.Combatui.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.People.Template.Name} has reached Level {playerUnit.People.Level}");

                yield return new WaitForSeconds(1f);

                yield return dialogBox.TypeDialog($"Your HP has been refilled thanks to your level up");

                yield return playerUnit.Combatui.SetXpSmooth(true);

            }

            yield return new WaitForSeconds(1f);
        }
       
        CheckForCombatOver(woundedUnit);
    }


    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A FATAL STRIKE!");

        if (damageDetails.ClassSetEffectiveness > 1f)
            yield return dialogBox.TypeDialog("A wounding hit!");
        else if (damageDetails.ClassSetEffectiveness < 1f)
            yield return dialogBox.TypeDialog("'Twas but a scratch!");
    }


    void CheckForCombatOver(CombatUnit woundedUnit)
    {
        if (woundedUnit.IsPlayerUnit)
        {
            var nextPeople = playerCompanions.GetHealthyPeople();
            if (nextPeople != null)
                OpenCompanionsMenu();
            else
                CombatOver(false);
        }
        else
            CombatOver(true);
    }

    public void HandleUpdate()
    {
        if (state == CombatState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == CombatState.CombatMoveSelection)
        {
            HandleCombatMoveSelection();
        }
        else if (state == CombatState.CompanionsMenu)
        {
            HandleCompanionsSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 2);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fight
                CombatMoveSelection();
            }
            else if (currentAction == 1)
            {
                // People
                OpenCompanionsMenu();
            }
            else if (currentAction == 2)
            {
                // Run
                StartCoroutine(EscapeAttempt());
            }

        }
        
    }

    IEnumerator EscapeAttempt()
    {
        state = CombatState.Busy;

        ++escapeAttempts;
        int enemyAgility = enemyUnit.People.Agility;
        int playerAgility = playerUnit.People.Agility;
        
        if (enemyAgility < playerAgility)
        {
            yield return dialogBox.TypeDialog($"We live to fight another day!");
            CombatOver(true);
        }
        else
        {
            float f = (playerAgility * 128) / enemyAgility + 30 * escapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"We live to fight another day!");
                CombatOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"It's no use running, they'd catch us!");
                StartCoroutine(EnemyCombatMove());
            }
        }
    }

    void HandleCombatMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentCombatMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentCombatMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentCombatMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentCombatMove -= 2;

        currentCombatMove = Mathf.Clamp(currentCombatMove, 0, playerUnit.People.CombatMoves.Count - 1);
        dialogBox.UpdateCombatMoveSelection(currentCombatMove, playerUnit.People.CombatMoves[currentCombatMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var combatMove = playerUnit.People.CombatMoves[currentCombatMove];
            if (combatMove.Stamina == 0) return;

            dialogBox.EnableCombatMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerCombatMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableCombatMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandleCompanionsSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerCompanions.Peoples.Count - 1);
        companionsMenu.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerCompanions.Peoples[currentMember];
            if (selectedMember.HP <= 0)
            {
                companionsMenu.SetMessageText("You can't send out corpses!");
                return;
            }
            if (selectedMember == playerUnit.People)
            {
                companionsMenu.SetMessageText("They're already fighting, open up your eyes!");
                return;
            }

            companionsMenu.gameObject.SetActive(false);
            state = CombatState.Busy;
            StartCoroutine(ChangePeople(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            companionsMenu.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator ChangePeople(People newPeople)
    {
        bool currentPeopleWounded = true;
        if (playerUnit.People.HP > 0)
        {
            currentPeopleWounded = false;
            yield return dialogBox.TypeDialog($"Fallback {playerUnit.People.Template.Name}!");
            playerUnit.PlayWoundedAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPeople);
        dialogBox.SetCombatMoveNames(newPeople.CombatMoves);
        yield return dialogBox.TypeDialog($"Quickly now {newPeople.Template.Name}, get out there!");

        if (currentPeopleWounded)
            ChooseFirstTurn();
        else
            StartCoroutine(EnemyCombatMove());
    }

   
}
