using System.Collections;
using System.Collections.Generic;


public enum Transition
{
    NullTransition,
    SawPlayer,
    LostPlayer,
    Waiting,
    Dying,
    PlayerFar,
    Dodging,
    Attack,
    Thinking,
    SmallStepsToBack,
    SmallStepsToAttack,
    SmallStepsToTaunt,
    StayStill,
    TauntToAttack,
    ToGunAttack,
    TauntToGunAttack,
    BeingGrabbed,
    ToBlock,
    WalksAllTheWay,
    GoingBackward,
    Attack3Hits,
    GoingToTaunt,
    GoingToHeadButt,
    RunToStop,
    isStunned,
    isBlockStunned,
    Escape,
    Destroy,
    WakeUp
}