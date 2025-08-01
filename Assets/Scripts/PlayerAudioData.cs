using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/AudioData", fileName = "New Audio Sheet")]
public class PlayerAudioData : ScriptableObject
{
    [Header("Game SFX")]
    public FMODUnity.EventReference loopsImpact;
    [Space] // adds an empty line

    public FMODUnity.EventReference spurtButton_press;
    [Space] // adds an empty line

    public FMODUnity.EventReference water_whoosh;
    [Space] // adds an empty line

    [Header("UI")]

    public FMODUnity.EventReference loopsOnHook;

    public string loopsOnHook_IntloopsHooked = "Count";
    [Space] // adds an empty line

    public FMODUnity.EventReference menu_buttonPress;
    [Space] // adds an empty line


    [Header("Ambience")]
    public FMODUnity.EventReference waterAmbience;

}