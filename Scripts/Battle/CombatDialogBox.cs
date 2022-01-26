using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatDialogBox : MonoBehaviour
{

    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;

    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject combatMoveSelector;
    [SerializeField] GameObject combatMoveDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> combatMoveTexts;

    [SerializeField] Text staminaText;
    [SerializeField] Text classSetText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableCombatMoveSelector(bool enabled)
    {
        combatMoveSelector.SetActive(enabled);
        combatMoveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }

    }
    public void UpdateCombatMoveSelection(int selectedCombatMove, CombatMove combatMove)
    {
        for (int i = 0; i < combatMoveTexts.Count; ++i)
        {
            if (i == selectedCombatMove)
                combatMoveTexts[i].color = highlightedColor;
            else
                combatMoveTexts[i].color = Color.black;
        }

        staminaText.text = $"STA: {combatMove.Stamina}/{combatMove.Template.Stamina}";
        classSetText.text = combatMove.Template.ClassSet.ToString();

        if (combatMove.Stamina == 0)
            staminaText.color = Color.red;
        else
            staminaText.color = Color.black;
    }

    public void SetCombatMoveNames(List<CombatMove> combatMoves)
    {
        for (int i = 0; i < combatMoveTexts.Count; ++i)
        {
            if (i < combatMoves.Count)
                combatMoveTexts[i].text = combatMoves[i].Template.Name;
            else
                combatMoveTexts[i].text = "-";
        }
    }
}