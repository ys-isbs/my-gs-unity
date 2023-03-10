using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject menuUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject ruleCanvas;
    [SerializeField] GameObject goalText;
    [SerializeField] GameObject goalSphereRoleModel;
    [SerializeField] GameObject rewardsButtons;
    [SerializeField] GameObject readyButton;
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] GameObject characterSelect;
    [SerializeField] Transform characters;
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TextMeshProUGUI rankingBoard;

    // Start is called before the first frame update
    private void Start()
    {
        inGameUI.SetActive(false);
        goalText.SetActive(false);
        goalSphereRoleModel.SetActive(false);
        rewardsButtons.SetActive(false);
        readyButton.SetActive(false);
        countdownText.text = "";
        UpdateSelectedChara(0);
        rankingBoard.text = "";
    }

    public void OnStartGame()
    {
        menuUI.SetActive(false);
        inGameUI.SetActive(true);
        characterSelect.SetActive(false);
    }

    public void ActiveGoalText(bool value)
    {
        goalText.SetActive(value);
    }

    public void ActiveGoalSphereRoleModel(bool value, RuntimeAnimatorController animatorController)
    {
        Animator animator = goalSphereRoleModel.GetComponentInChildren<Animator>();
        animator.runtimeAnimatorController = animatorController;
        goalSphereRoleModel.SetActive(value);
    }

    public void ActiveRewardsButtons(bool value)
    {
        goalText.GetComponent<TextMeshProUGUI>().text = "CLEAR!";
        rewardsButtons.SetActive(value);
    }

    public void ActiveReadyButton(bool value)
    {
        readyButton.SetActive(value);
    }

    public void OnClickCloseRuleButton()
    {
        ruleCanvas.SetActive(false);
    }

    public void UpdateCountdown(string count)
    {
        countdownText.text = count;
    }

    public int UpdateSelectedChara(int index)
    {
        var newIndex = index;
        if (newIndex < 0)
        {
            newIndex = characters.childCount - 1;
        }

        if (newIndex >= characters.childCount)
        {
            newIndex = 0;
        }

        foreach(Transform obj in characters)
        {
            obj.gameObject.SetActive(false);
        }
        characters.GetChild(newIndex).gameObject.SetActive(true);

        return newIndex;
    }

    public string GetPlayerName()
    {
        if (string.IsNullOrWhiteSpace(nameInputField.text))
        {
            return "No Name";
        }
        else
        {
            return nameInputField.text;
        }
    }

    public void UpdateRanking(string[] playerNames)
    {
        int rank = 1;
        string newRanking = "";
        foreach (string name in playerNames)
        {
            newRanking += $"No.{rank} - {name}\n";
            rank++;
        }

        rankingBoard.text = newRanking;
    }
}
