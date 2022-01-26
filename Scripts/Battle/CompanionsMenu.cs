using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanionsMenu : MonoBehaviour
{
    [SerializeField] Text companionsSelectText;

    CompanionsMemberUI[] memberSlots;
    List<People> peoples;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<CompanionsMemberUI>();
    }

    public void SetCompanionsData(List<People> peoples)
    {
        this.peoples = peoples;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < peoples.Count)
                memberSlots[i].SetData(peoples[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        companionsSelectText.text = "Choose a Warrior";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < peoples.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        companionsSelectText.text = message;
    }
}
