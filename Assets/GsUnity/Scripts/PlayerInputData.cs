using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

enum InputButtons
{
    Jump = 0,
    Emote1 = 1,
}

public struct PlayerInputData : INetworkInput
{
    public Vector3 moveDirection;
    public NetworkButtons buttons;
}
