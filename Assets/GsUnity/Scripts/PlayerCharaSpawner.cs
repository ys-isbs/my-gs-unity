using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerCharaSpawner : NetworkBehaviour
{
    [SerializeField] NetworkPrefabRef[] characters;
    private GameManager gameManager;

    private NetworkObject spawnedCharacter;

    public override void Spawned()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameObject.name = $"PlayerCharaSpawner {Object.InputAuthority}";

        if (Object.HasInputAuthority)
        {
            RPC_SpawnChara(Runner.LocalPlayer, gameManager.selectedCharaIndex, gameManager.playerName);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        runner.Despawn(spawnedCharacter);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SpawnChara(PlayerRef player, int charaIndex, string playerName)
    {
        spawnedCharacter = Runner.Spawn(
            characters[charaIndex],
            gameManager.StartSpawnPoint.position,
            Quaternion.identity,
            player,
            (Runner, spawnedObj) => spawnedObj.GetComponent<PlayerController>().Name = playerName);
    }
}
