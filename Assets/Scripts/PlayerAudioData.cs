using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/AudioData", fileName = "New Audio Sheet")]
public class PlayerAudioData : ScriptableObject
{
    [Header("Game SFX")]
    public FMODUnity.EventReference loopsImpact;

    public string loopsImpact_FloatImpactStrength = "ImpactStrength";
    [Space] // adds an empty line

    public FMODUnity.EventReference tankImpact;

    public string tankImpact_FloatImpactStrength = "ImpactStrength";
    [Space] // adds an empty line

    public FMODUnity.EventReference water_whoosh;
    [Space] // adds an empty line

    [Header("UI")]

    public FMODUnity.EventReference loopsOnHook;

    public string loopsOnHook_IntloopsHooked = "Count";
    [Space] // adds an empty line

    public FMODUnity.EventReference loopsOnHookFinal;
    [Space] // adds an empty line

    public FMODUnity.EventReference bubbleTransition;
    [Space] // adds an empty line

    public FMODUnity.EventReference menu_buttonPress;
    [Space] // adds an empty line

    public FMODUnity.EventReference pointsToTotal;
    [Space] // adds an empty line

    public FMODUnity.EventReference upgradeBuy;
    [Space] // adds an empty line

    public FMODUnity.EventReference upgradeFail;
    [Space] // adds an empty line

    public FMODUnity.EventReference menu_upgradeSlideIn;
    [Space] // adds an empty line

    public FMODUnity.EventReference menu_upgradeSlideOut;
    [Space] // adds an empty line

    [Header("Music")]
    public FMODUnity.EventReference musicLoop;
    [Space] // adds an empty line

    [Header("Ambience")]
    public FMODUnity.EventReference waterAmbience;

}