using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TalkingAndQuest : MonoBehaviour
{
    public List<string> TalkText;
    public GameObject TalkingUI;
	public GameObject QuestUI_Done;
	public GameObject QuestUI_Claim;
	public GameObject QuestUI_Questing;
    public GameObject QuestUI_Required;
    private int TextIndex = 0;

    public float RadiusActive = 1.2f;
	public GameObject Shop;
	public List<QuestWithTalk> QuestList;
	public List<QuestRequiredWithTalk> QuestRequired;

	SaveSystem SaveSystem = SaveSystem.Instance;
    GeneralInformation GeneralInformation = GeneralInformation.Instance;

    public Quest EventQuestGive;
    public bool IsEventTalk = false;
    private bool IsTexting = false;
    private Coroutine Texting;

	private void Start()
	{
		
	}


	public void ShowTalk()
    {
        if(IsEventTalk)
        {
            TalkingUI.SetActive(true);
            GeneralInformation.Actioning = "Talking";
            if (EventQuestGive != null)
            SaveSystem.saveLoad.questClaim.AddQuest(EventQuestGive);
            IsEventTalk = false;
        }
        if (GeneralInformation.Actioning != "Playing" && GeneralInformation.Actioning != "Talking") return;
        if (!TalkingUI.activeSelf)
		{   
            if(QuestList.Count > 0)
			while (QuestList[0].quest.Status == "Done" || QuestList[0].quest.Status == "Canceled")
			{
                QuestList.RemoveAt(0);
                if (QuestList.Count == 0) break;
			}
			if(CheckRequired() != null)
			{
				TalkText = CheckRequired();
                RemoveRequired();
			}	
			else
			{
                if (QuestList.Count > 0)
                {
                    if (QuestList[0].quest.Status == "UnClaimed")
                    {
                        if (QuestList[0].quest.CheckQuestAvailable() && QuestList[0].quest.IsEventClaimed == false)
                        {
                            TalkText = QuestList[0].ClaimQuestTexts;
                            SaveSystem.saveLoad.questClaim.AddQuest(QuestList[0].quest);
                        }
                    }
                    else if (QuestList[0].quest.Status == "Questing")
                    {
                        TalkText = QuestList[0].QuestingTexts;
                    }
                    else if (QuestList[0].quest.Status == "Reach")
                    {
                        TalkText = QuestList[0].DoneQuestTexts;
                        SaveSystem.saveLoad.ClaimQuestReward(QuestList[0].quest);
                    }
                }
            }
            TalkingUI.SetActive(true);
            GeneralInformation.Actioning = "Talking";
        }

        if (TextIndex >= TalkText.Count)
        {
			TalkingUI.SetActive(false);
            GeneralInformation.Actioning = "Playing";
            TextIndex = 0;
			if(Shop != null)
			{
				Shop.SetActive(true);
                GeneralInformation.Actioning = "Shoping";
            }
        }
		else
		{
            GameObject TextObj = TalkingUI.transform.Find("Text").gameObject;
            TMP_Text text = TextObj.GetComponent<TMP_Text>();
            text.text = "";
            Texting = StartCoroutine(ShowTalkDelay(text));
        }	
    }

    IEnumerator ShowTalkDelay(TMP_Text textshow)
    {
        IsTexting = true;
        int i = 0;
        while(i < TalkText[TextIndex].Count())
        {
            textshow.text += TalkText[TextIndex][i];
            i++;
            yield return new WaitForSeconds(0.05f);
        }
        IsTexting = false;
        TextIndex++;
    }


    // Update is called once per frame
    void RemoveRequired()
    {
        foreach( var item in QuestRequired ) 
        {
            SaveSystem.saveLoad.questClaim.CheckTalkRequiredAllQuest(item.QuestRequired);
        }
    }
    List<string> CheckRequired()
	{
        for (int i = 0; i < SaveSystem.saveLoad.questClaim.quests.Count; i++)
        {
            if (SaveSystem.saveLoad.questClaim.quests[i].Status == "Questing")
            {
				foreach (QuestRequiredWithTalk q in QuestRequired)
				{
					foreach (string s in SaveSystem.saveLoad.questClaim.quests[i].QuestRequired)
					{
						if(q.QuestRequired == s)
						{
                            return q.TalkForQuest;
                        }	
					}	
				}	
            }
        }
		return null;
    }	
    void Update()
    {
        if(QuestList.Count > 0)
        {
            while (QuestList[0].quest.Status == "Done" || QuestList[0].quest.Status == "Canceled")
            {
                QuestList.RemoveAt(0);
                if (QuestList.Count == 0) break;
            }
        }
        
        QuestUI_Required.SetActive(false);
        QuestUI_Claim.SetActive(false);
        QuestUI_Done.SetActive(false);
        QuestUI_Questing.SetActive(false);
        if (CheckRequired() != null)
		{
            QuestUI_Required.SetActive(true);
        }
		else 
		{ 
			QuestUI_Required.SetActive(false);
            if (QuestList.Count > 0)
            {
                if (QuestList[0].quest.Status == "UnClaimed")
                {
                    if (QuestList[0].quest.CheckQuestAvailable() && QuestList[0].quest.IsEventClaimed == false)
                    {
                        QuestUI_Claim.SetActive(true);
                    }
                    else QuestUI_Claim.SetActive(false);
                }
                else if (QuestList[0].quest.Status == "Questing")
                {
                    QuestUI_Questing.SetActive(true);
                }
                else if (QuestList[0].quest.Status == "Reach")
                {
                    QuestUI_Done.SetActive(true);
                }
            }
        }

        Transform player = GameObject.Find("Player").transform;
		float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < RadiusActive || TalkingUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.J) || ActionButton.Instance.IsPress || (TalkingUI.activeSelf && Input.GetMouseButtonDown(0)))
		{
            ActionButton.Instance.ResetIsPress();
            if(!TalkingUI.activeSelf)
            {
                
                    if (IsTexting == false)
                    {
                        ShowTalk();
                    }
                    else
                    {
                        IsTexting = false;
                        StopCoroutine(Texting);
                        GameObject TextObj = TalkingUI.transform.Find("Text").gameObject;
                        TMP_Text text = TextObj.GetComponent<TMP_Text>();
                        text.text = TalkText[TextIndex];
                        TextIndex++;
                    }
                
            }
            else
            {
                if (IsTexting == false)
                {
                    ShowTalk();
                }
                else
                {
                    IsTexting = false;
                    StopCoroutine(Texting);
                    GameObject TextObj = TalkingUI.transform.Find("Text").gameObject;
                    TMP_Text text = TextObj.GetComponent<TMP_Text>();
                    text.text = TalkText[TextIndex];
                    TextIndex++;
                }
            }    
        }
        }
    }
}

[System.Serializable]
public class QuestWithTalk
{
	public Quest quest;
    public List<string> ClaimQuestTexts;
    public List<string> QuestingTexts;
	public List<string> DoneQuestTexts;
}

[System.Serializable]
public class QuestRequiredWithTalk
{
	public string QuestRequired;
	public List<string> TalkForQuest;
}
