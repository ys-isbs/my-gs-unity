using System.Collections;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    [SerializeField] UIManager uiManager;
    [SerializeField] FusionLauncher fusionLauncher;
    [SerializeField] GoalTrigger goalTrigger;
    [SerializeField] SoundManager soundManager;
    [SerializeField] StartGate startGate;
    [SerializeField] RuntimeAnimatorController[] animatorControllers;
    [Networked] int animatorControllersIndex { get; set; }

    public Transform StartSpawnPoint;

    public int selectedCharaIndex;
    public string playerName;

    [Networked(OnChanged = nameof(OnChangedRule))]
    private NetworkBool ruleActive { get; set; }

    private bool IsLocalPlayerGoaled;
    [Networked(OnChanged = nameof(OnChangedGoalRanking))]
    [Capacity(10)]
    private NetworkLinkedList<PlayerController> playersGoalRanking => default;

    [Networked(OnChanged = nameof(OnChangedClearRanking))]
    [Capacity(10)]
    private NetworkLinkedList<PlayerController> playersClearRanking => default;

    private void Start()
    {
        goalTrigger.OnGoalEvent += OnGoal;
        startGate.OnCountEvent += OnCountdown;
    }

    public override void Spawned()
    {
        ruleActive = true;
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
        ruleActive = false;
    }

    public static void OnChangedRule(Changed<GameManager> changed)
    {
        changed.Behaviour.DeactivateRuleCanvas();
    }

    public void DeactivateRuleCanvas()
    {
        if (!ruleActive) uiManager.OnClickCloseRuleButton();
    }

    public void OnGoal(NetworkObject networkPlayer)
    {
        var player = networkPlayer.GetComponent<PlayerController>();

        if (Runner.IsServer)
        {
            if (playersGoalRanking.Count == 0)
            {
                animatorControllersIndex = Random.Range(0, animatorControllers.Length);
            }

            if (!playersGoalRanking.Contains(player))
            {
                playersGoalRanking.Add(player);
            }
        }

        if (!IsLocalPlayerGoaled && networkPlayer.HasInputAuthority)
        {
            uiManager.ActiveGoalText(true);
            soundManager.PlayGoalSe();
            IsLocalPlayerGoaled = true;
        }
    }

    public void CheckEmote(PlayerController player, string emoteName)
    {
        var networkPlayer = player.GetComponent<NetworkObject>();

        if (IsLocalPlayerGoaled && networkPlayer.HasInputAuthority)
        {
            var animatorController = animatorControllers[animatorControllersIndex];
            if (animatorController.name == emoteName && !playersClearRanking.Contains(player))
            {
                uiManager.ActiveRewardsButtons(true);
                soundManager.PlayGoalSe();
                RPC_OnClear(player);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_OnClear(PlayerController player)
    {
        playersClearRanking.Add(player);
    }

    public static void OnChangedGoalRanking(Changed<GameManager> changed)
    {
        changed.Behaviour.ActiveGoalSphere();
    }

    public static void OnChangedClearRanking(Changed<GameManager> changed)
    {
        changed.Behaviour.UpdateRanking();
    }

    public void UpdateRanking()
    {
        var names = playersClearRanking.Select(player => player.Name).ToArray();
        uiManager.UpdateRanking(names);
    }

    public void ActiveGoalSphere()
    {
        var animatorController = animatorControllers[animatorControllersIndex];
        uiManager.ActiveGoalSphereRoleModel(true, animatorController);
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
