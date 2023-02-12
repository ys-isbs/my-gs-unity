using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    [SerializeField] UIManager uiManager;
    [SerializeField] FusionLauncher fusionLauncher;
    [SerializeField] GoalTrigger goalTrigger;
    [SerializeField] SoundManager soundManager;
    [SerializeField] StartGate startGate;

    public Transform StartSpawnPoint;

    private bool IsLocalPlayerGoaled;
    public int selectedCharaIndex;
    public string playerName;

    [Networked(OnChanged = nameof(OnChangedRanking))]
    [Capacity(10)]
    private NetworkLinkedList<PlayerController> playersRanking => default;

    private void Start()
    {
        goalTrigger.OnGoalEvent += OnGoal;
        startGate.OnCountEvent += OnCountdown;
    }

    public void SelectCharacter(int direction)
    {
        selectedCharaIndex += direction;
        selectedCharaIndex = uiManager.UpdateSelectedChara(selectedCharaIndex);
    }

    void OnCountdown(int count)
    {
        if (count > 0)
        {
            uiManager.UpdateCountdown(count.ToString());
        }
        else
        {
            startGate.OnCountEvent -= OnCountdown;
            StartCoroutine(FinishCountdown());
        }
    }

    IEnumerator FinishCountdown()
    {
        uiManager.UpdateCountdown("GO!");
        yield return new WaitForSeconds(1);
        uiManager.UpdateCountdown("");
    }

    public void OnReady()
    {
        uiManager.ActiveReadyButton(false);
        startGate.StartCountdown(5);
    }

    public void OnGoal(NetworkObject networkPlayer)
    {
        if (Runner.IsServer)
        {
            var player = networkPlayer.GetComponent<PlayerController>();
            if (!playersRanking.Contains(player))
            {
                playersRanking.Add(player);
            }
        }

        if (!IsLocalPlayerGoaled && networkPlayer.HasInputAuthority)
        {
            uiManager.ActiveGoalText(true);
            soundManager.PlayGoalSe();
            IsLocalPlayerGoaled = true;

            uiManager.EnableActiveRewardsButtons(true);
        }
    }

    public static void OnChangedRanking(Changed<GameManager> changed)
    {
        changed.Behaviour.UpdateRanking();
    }

    public void UpdateRanking()
    {
        var names = playersRanking.Select(player => player.Name).ToArray();
        uiManager.UpdateRanking(names);
    }

    public void StartHost()
    {
        playerName = uiManager.GetPlayerName();
        fusionLauncher.LaunchGame(Fusion.GameMode.Host);
        uiManager.OnStartGame();
        uiManager.ActiveReadyButton(true);
    }

    public void StartClient()
    {
        playerName = uiManager.GetPlayerName();
        fusionLauncher.LaunchGame(Fusion.GameMode.Client);
        uiManager.OnStartGame();
    }

    public void QuitGame()
    {
        fusionLauncher.QuitGame();
    }
}
