using System.Collections;
using System.Linq;
using Fusion;
using UnityEngine;

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

    private bool IsLocalPlayerGoaled;
    public int selectedCharaIndex;
    public string playerName;

    [Networked(OnChanged = nameof(OnChangedRanking))]
    [Capacity(10)]
    private NetworkLinkedList<PlayerController> playersRanking => default;

    [Networked(OnChanged = nameof(OnChangedRule))]
    private NetworkBool ruleActive { get; set; }

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
        if (Runner.IsServer)
        {
            if (playersRanking.Count == 0)
            {
                animatorControllersIndex = Random.Range(0, animatorControllers.Length);
            }

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
        }
    }

    public void OnEmote(PlayerController player, string emoteName)
    {
        var networkPlayer = player.GetComponent<NetworkObject>();

        if (IsLocalPlayerGoaled && networkPlayer.HasInputAuthority)
        {
            var animatorController = animatorControllers[animatorControllersIndex];
            if (animatorController.name == emoteName)
            {
                uiManager.ActiveRewardsButtons(true);
            }
        }
    }

    public static void OnChangedRanking(Changed<GameManager> changed)
    {
        changed.Behaviour.UpdateRanking();
        changed.Behaviour.ActiveGoalSphere();
    }

    public void UpdateRanking()
    {
        var names = playersRanking.Select(player => player.Name).ToArray();
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
