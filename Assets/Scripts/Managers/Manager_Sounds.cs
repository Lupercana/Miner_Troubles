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
    [SerializeField] private AudioClip sfx_denied = null;
    [SerializeField] private AudioClip sfx_purchase = null;
    [SerializeField] private AudioClip sfx_basic_hit = null;
    [SerializeField] private AudioClip sfx_hammer_hit = null;
    [SerializeField] private AudioClip sfx_bomb_hit = null;
    [SerializeField] private AudioClip sfx_staff_hit = null;
    [SerializeField] private AudioClip sfx_torch_hit = null;
    [SerializeField] private AudioClip sfx_tool_change = null;
    [SerializeField] private AudioClip sfx_upverter = null;
    [SerializeField] private AudioClip sfx_worker_activate = null;
    [SerializeField] private AudioClip sfx_worker_tool_swap = null;

    [SerializeField] private bool check_visibility = true;
    [SerializeField] private float starting_volume_music = 0.5f;
    [SerializeField] private float starting_volume_sfx = 0.5f;

    private float last_play_level_up = 0f;
    private float last_play_mining_intermediate = 0f;
    private float last_play_mining_finished = 0f;
    private float last_play_upverter = 0f;

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

    public void PlayLevelUp(bool visible) { if (ShouldPlaySFX(visible, last_play_level_up, sfx_level_up.length)) { source_sfx.PlayOneShot(sfx_level_up); last_play_level_up = Time.time; } }
    public void PlayMiningIntermediate(bool visible) { if (ShouldPlaySFX(visible, last_play_mining_intermediate, sfx_mining_intermediate.length)) { source_sfx.PlayOneShot(sfx_mining_intermediate); last_play_mining_intermediate = Time.time; } }
    public void PlayMiningFinished(bool visible) { if (ShouldPlaySFX(visible, last_play_mining_finished, sfx_mining_finished.length)) { source_sfx.PlayOneShot(sfx_mining_finished); last_play_mining_finished = Time.time; } }
    public void PlayUpverter(bool visible) { if (ShouldPlaySFX(visible, last_play_upverter, sfx_upverter.length)) { source_sfx.PlayOneShot(sfx_upverter); last_play_upverter = Time.time; } }

    public void PlayPurchase(bool visible) { if (ShouldPlaySFX(visible)) { source_sfx.PlayOneShot(sfx_purchase); } }
    public void PlayDenied(bool visible) { if (ShouldPlaySFX(visible)) { source_sfx.PlayOneShot(sfx_denied); } }
    public void PlayBasicHit(bool visible) { if (ShouldPlaySFX(visible)) { source_sfx.PlayOneShot(sfx_basic_hit); } }
    public void PlayHammerHit(bool visible) { if (ShouldPlaySFX(visible)) { source_sfx.PlayOneShot(sfx_hammer_hit); } }
    public void PlayBombHit(bool visible) { if (ShouldPlaySFX(visible)) { source_sfx.PlayOneShot(sfx_bomb_hit); } }
    public void PlayStaffHit(bool visible) { if (ShouldPlaySFX(visible)) { source_sfx.PlayOneShot(sfx_staff_hit); } }
    public void PlayTorchHit(bool visible) { if (ShouldPlaySFX(visible)) { source_sfx.PlayOneShot(sfx_torch_hit); } }
    public void PlayToolChange(bool visible) { if (ShouldPlaySFX(visible)) { source_sfx.PlayOneShot(sfx_tool_change); } }
    public void PlayWorkerActivate(bool visible) { if (ShouldPlaySFX(visible)) { source_sfx.PlayOneShot(sfx_worker_activate); } }
    public void PlayWorkerToolSwap(bool visible) { if (ShouldPlaySFX(visible)) { source_sfx.PlayOneShot(sfx_worker_tool_swap); } }

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

    private bool ShouldPlaySFX(bool visible, float last_play = 0f, float clip_length = 0f)
    {
        float e_time = Time.time - last_play;
        if (clip_length != 0 && e_time <= clip_length)
        {
            return false;
        }

        return visible || !check_visibility;
    }
}
