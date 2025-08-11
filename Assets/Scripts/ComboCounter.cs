using FMODUnity;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ComboCounter : MonoBehaviour
{
    public MeshRenderer hookSpike;
    public Transform hookTimer;

    public int highScore = 0;
    private bool hasNewHighScoreBeenNotified = false;


    private List<RingHandler> rings = new List<RingHandler>();

    public float comboDuration = 2.0f;
    public float ringCountDelay = 0.4f;
    private float comboTimer;
    private int currentCombo = 0;

    public int maxCombo = 10; // Maximum combo to display

    private void Start()
    {
        if(hookTimer) hookTimer.GetComponentInChildren<MeshRenderer>().enabled = false; // Show the timer visual
    }

    private void Update()
    {
        if (currentCombo > 0)
        {
            //comboText.text = "TEST COMBO x" + currentCombo;
            comboTimer -= Time.deltaTime;
            hookSpike.material.SetFloat("_FillUp", 1 - (comboTimer / comboDuration));
            if(hookTimer) hookTimer.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 360, 1 - (comboTimer / comboDuration)));
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    public void AddToCombo(RingHandler ring)
    {
        if (comboTimer <= 0f)
            currentCombo = 0;

        rings.Add(ring);
        if (hookTimer) hookTimer.GetComponentInChildren<MeshRenderer>().enabled = false; // Show the timer visual

        currentCombo++;
        comboTimer = comboDuration;

        GameManager.Instance.scoreManager.UpdateCombo(currentCombo);

        Debug.Log("Combo: " + currentCombo + " Rings: " + rings.Count);
        bool isLastRingInCombo = currentCombo == maxCombo;
        EventReference loopSound = !isLastRingInCombo ? GameManager.Instance.playerAudioData.loopsOnHook : GameManager.Instance.playerAudioData.loopsOnHookFinal;
        FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(GameManager.Instance.playerAudioData.loopsOnHook);
        if(!isLastRingInCombo) instance.setParameterByName(GameManager.Instance.playerAudioData.loopsOnHook_IntloopsHooked, currentCombo);
        instance.start();

    }

    public void RemoveFromCombo(RingHandler ring)
    {

        rings.Remove(ring);
        currentCombo--;

        GameManager.Instance.scoreManager.UpdateCombo(currentCombo);
    }


    private void ResetCombo()
    {
        GameManager.Instance.scoreManager.UpdateCombo(0); // <- pass 0 to clear combo text
        if (hookTimer) hookTimer.GetComponentInChildren<MeshRenderer>().enabled = false; // Show the timer visual

        float delayOffset = ringCountDelay;
        float currentDelay = 0.0f;
        foreach (RingHandler ring in rings)
        {
            if (ring != null)
            {
                ring.StartCounting(currentCombo, currentDelay);
                currentDelay += delayOffset;
            }
        }

        hookSpike.material.SetFloat("_FillUp", 0);
        currentCombo = 0;
        rings.Clear();
    }
}