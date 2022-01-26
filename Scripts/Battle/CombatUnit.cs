using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CombatUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] CombatUI combatui;

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
    }

    public CombatUI Combatui
    {
        get { return combatui; }
    }

    public People People { get; set; }

    Image image;
    Vector3 originalPos;
    Color originalColor;
    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(People people)
    {
        People = people;
        if (isPlayerUnit)
            image.sprite = People.Template.BackSprite;
        else
            image.sprite = People.Template.FrontSprite;

        combatui.SetData(people);

        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(originalPos.x, 400f);
        else
            image.transform.localPosition = new Vector3(originalPos.x, 400f);

        image.transform.DOLocalMoveY(originalPos.y, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayWoundedAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}
