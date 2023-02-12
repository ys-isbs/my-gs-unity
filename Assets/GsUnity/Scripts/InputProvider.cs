using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour, INetworkRunnerCallbacks
{
    PlayerInput input;
    InputAction moveInput;
    InputAction jumpInput;
    InputAction emote1Input;
    Vector3 moveDirection;
    bool jumpInputPressed;
    bool emote1InputPressed;
    PlayerInputData inputData = new PlayerInputData();
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        moveInput = input.actions["Move"];
        jumpInput = input.actions["Jump"];
        emote1Input = input.actions["Emote1"];
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInputValue = moveInput.ReadValue<Vector2>();
        Vector3 camForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
        Vector3 verticalValue = moveInputValue.y * camForward;
        Vector3 holizontalValue = moveInputValue.x * cam.transform.right;
        moveDirection = verticalValue + holizontalValue;

        if (jumpInput.WasPressedThisFrame())
        {
            jumpInputPressed = true;
        }

        if (emote1Input.WasPressedThisFrame())
        {
            emote1InputPressed = true;
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        inputData.moveDirection = moveDirection;
        inputData.buttons.Set(InputButtons.Jump, jumpInputPressed);
        inputData.buttons.Set(InputButtons.Emote1, emote1InputPressed);
        input.Set(inputData);
        inputData = default;
        jumpInputPressed = false;
        emote1InputPressed = false;
    }

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

}
