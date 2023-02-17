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
            if (playersGoalRanking.Count == 0)
            {
                animatorControllersIndex = Random.Range(0, animatorControllers.Length);
            }

            var player = networkPlayer.GetComponent<PlayerController>();
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

            var playerController = FindObjectOfType<PlayerController>();
            playerController.OnEmoteEvent += OnEmote;
        }
    }

    //public void OnEmote(PlayerController player, string emoteName)
    public void OnEmote(NetworkObject networkPlayer, string emoteName)
    {
        //var networkPlayer = player.GetComponent<NetworkObject>();
        var player = networkPlayer.GetComponent<PlayerController>();
        Debug.Log(player);
        Debug.Log(player.name);
        Debug.Log(networkPlayer);
        Debug.Log(networkPlayer.name);
        Debug.Log(IsLocalPlayerGoaled);
        Debug.Log(networkPlayer.HasInputAuthority);
        if (IsLocalPlayerGoaled && networkPlayer.HasInputAuthority)
        {
            Debug.Log("IsLocalPlayerGoaled && networkPlayer.HasInputAuthority");
            var animatorController = animatorControllers[animatorControllersIndex];
            Debug.Log(animatorController);
            Debug.Log(animatorController.name);
            Debug.Log(emoteName);
            if (animatorController.name == emoteName)
            {
                Debug.Log("animatorController.name == emoteName");
                Debug.Log(playersClearRanking);
                Debug.Log(playersClearRanking.Contains(player));

                if (!playersClearRanking.Contains(player))
                {
                    uiManager.ActiveRewardsButtons(true);
                    soundManager.PlayGoalSe();
                    if (Runner.IsServer)
                    {
                        playersClearRanking.Add(player);
                    }
                }
            }
        }
    }

    public static void OnChangedGoalRanking(Changed<GameManager> changed)
    {
        changed.Behaviour.ActiveGoalSphere();
    }

    public void ActiveGoalSphere()
    {
        if (playersGoalRanking.Count == 1)
        {
            var animatorController = animatorControllers[animatorControllersIndex];
            uiManager.ActiveGoalSphereRoleModel(true, animatorController);
        }
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
