using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMove
{

    public CombatMoveTemplate Template { get; set; }
    public int Stamina { get; set; }

    public CombatMove(CombatMoveTemplate pTemplate)
    {
        Template = pTemplate;
        Stamina = pTemplate.Stamina;
    }
}
