using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameController : MonoBehaviour
{
    public enum CurrentState { FreeRoamLand, Combat } 
    CurrentState state;

    public static GameController Instance { get; private set; }
    [SerializeField] Respawn respawner;
    [SerializeField] CombatSystem combatSystem;
    [SerializeField] Camera landCamera;
    [SerializeField] DetectorCombat detectorCombat;
    [SerializeField] DetectorCombat detectorCombat2;
    [SerializeField] DetectorCombat detectorCombat3;
    [SerializeField] BoxDetect boxDetect;
    [SerializeField] PlayerController playerController;

    public void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        playerController.OnAmbushed += BeginFight;
        detectorCombat.OnAmbushed += BeginFight;
        detectorCombat2.OnAmbushed += BeginFight;
        detectorCombat3.OnAmbushed += BeginFight;
        combatSystem.OnAmbushedOver += EndFight;
    }

    void BeginFight()
    {
        state = CurrentState.Combat;
        combatSystem.gameObject.SetActive(true);
        detectorCombat.gameObject.SetActive(false);
        detectorCombat2.gameObject.SetActive(false);
        detectorCombat3.gameObject.SetActive(false); //sets up the combat screen and changes state
        boxDetect.gameObject.SetActive(false);
        landCamera.gameObject.SetActive(false);

        var playerCompanions = playerController.GetComponent<PeopleCompanions>();
        var ambushPeople = FindObjectOfType<MapController>().GetComponent<MapController>().GetRandomAmbusherPeople();

        combatSystem.BeginFight(playerCompanions, ambushPeople);
    }

   public void BeginBossFight(BoxDetect boss)
    {
        state = CurrentState.Combat;
        combatSystem.gameObject.SetActive(true);
       
        boxDetect.gameObject.SetActive(false); //boxdetector has a seperate selection of enemies to fight, so you have a more challenging zone to fight in
        landCamera.gameObject.SetActive(false);

        var playerCompanions = playerController.GetComponent<PeopleCompanions>();
        var bossCompanions = boss.GetComponent<PeopleCompanions>();

        combatSystem.BeginBossFight(playerCompanions, bossCompanions);
    }

    void EndFight(bool won)
    {
        state = CurrentState.FreeRoamLand;
        combatSystem.gameObject.SetActive(false);
        landCamera.gameObject.SetActive(true);
        StartRespawnTimer();
        respawner.SpawnChaser(); //will respawn the chasers and select a random pos for them to do so
    }
    public void StartRespawnTimer()
    {
        StartCoroutine(RespawnTimer(5));
    }

    private IEnumerator RespawnTimer(float duration)
    {
        detectorCombat.gameObject.SetActive(false);
        detectorCombat2.gameObject.SetActive(false);
        detectorCombat3.gameObject.SetActive(false);
        boxDetect.gameObject.SetActive(false);
        yield return new WaitForSeconds(duration); //Made a respawn timer so that the player wouldnt be dragged into another fight straight away
        detectorCombat.gameObject.SetActive(true);
        detectorCombat2.gameObject.SetActive(true);
        detectorCombat3.gameObject.SetActive(true);
        boxDetect.gameObject.SetActive(true);

    }
       
    private void Update()
    {
        
        if (state == CurrentState.FreeRoamLand)
        {
            playerController.HandleUpdate();
        }
        else if (state == CurrentState.Combat)
        {
            combatSystem.HandleUpdate();
        }
    }
}
