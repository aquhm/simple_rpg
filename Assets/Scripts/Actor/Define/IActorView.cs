﻿using UnityEngine;

namespace Client.Actor
{
    public interface IActorView
    {
        CharacterController CharacterController { get; }
        Animator Animator { get; }
        Transform Transform { get; }
        ActorSettings Settings { get; }
        ActorEquipmentReferences EquipmentRefs { get; }
    }
}
