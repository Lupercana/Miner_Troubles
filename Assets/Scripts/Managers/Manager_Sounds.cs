using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Sounds : MonoBehaviour
{
    public static Manager_Sounds Instance = null;

    [SerializeField] private Slider ui_slider_music = null;
    [SerializeField] private Slider ui_slider_sfx = null;

    [SerializeField] private AudioSource source_track = null;
    [SerializeField] private AudioSource source_sfx = null;

    [SerializeField] private AudioClip track_main = null;
    [SerializeField] private AudioClip sfx_level_up = null;
    [SerializeField] private AudioClip sfx_mining_intermediate = null;
    [SerializeField] private AudioClip sfx_mining_finished = null;
    //[SerializeField] private AudioClip sfx_gate_unlock = null;
    [SerializeField] private AudioClip sfx_purchase = null;

    [SerializeField] private float starting_volume_music = 0.5f;
    [SerializeField] private float starting_volume_sfx = 0.5f;

    public float GetTrackVolume() { return source_track.volume; }
    public float GetSFXVolume() { return source_track.volume; }

    public void SetTrackVolume(float v)
    {
        source_track.volume = v;
    }

    public void SetSFXVolume(float v)
    {
        source_sfx.volume = v;
    }

    public void PlayLevelUp() { source_sfx.PlayOneShot(sfx_level_up); }
    public void PlayMiningIntermediate() { source_sfx.PlayOneShot(sfx_mining_intermediate); }
    public void PlayMiningFinished() { source_sfx.PlayOneShot(sfx_mining_finished); }
    public void PlayPurchase() { source_sfx.PlayOneShot(sfx_purchase); }

    public void StopSFX()
    {
        source_sfx.Stop();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        source_track.volume = starting_volume_music;
        source_sfx.volume = starting_volume_sfx;
        ui_slider_music.value = starting_volume_music;
        ui_slider_sfx.value = starting_volume_sfx;

        source_track.loop = true;
        source_track.clip = track_main;
        source_track.Play();
    }
}
