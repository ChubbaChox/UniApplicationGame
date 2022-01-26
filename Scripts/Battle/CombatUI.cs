using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject xpBar;

    People _people;

    public void SetData(People people)
    {
        _people = people;

        nameText.text = people.Template.Name;
        SetLevel();
        hpBar.SetHP((float)people.HP / people.MaxHp);
        SetXp();
    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + _people.Level;
    }

    public void SetXp()
    {
        if (xpBar == null) return;

        float normalizedXp = GetNormalizedXp();
        xpBar.transform.localScale = new Vector3(normalizedXp, 1, 1);
    }

    public IEnumerator SetXpSmooth(bool reset=false)
    {
        if (xpBar == null) yield break;

        if (reset)
            xpBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedXp = GetNormalizedXp();
        yield return xpBar.transform.DOScaleX(normalizedXp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedXp()
    {
        int currLevelXp = _people.Template.GetXpForLevel(_people.Level);
        int nextLevelXp = _people.Template.GetXpForLevel(_people.Level + 1);

        float normalizedXp = (float)(_people.Xp - currLevelXp) / (nextLevelXp - currLevelXp);
        return Mathf.Clamp01(normalizedXp);
    }

    public void OnHeal()
    {
        StartCoroutine(UpdateHPHeal());
    }
    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float)_people.HP / _people.MaxHp);
    }

    public IEnumerator UpdateHPHeal()
    {
        yield return hpBar.SetHPSmoothGain((float)_people.HP / _people.MaxHp);
    }
}
