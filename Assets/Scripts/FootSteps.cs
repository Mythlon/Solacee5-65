
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource footstepSource;
    public AudioSource slideSource;

    [Header("Default Footsteps")]
    public AudioClip[] footstepClips; // fallback/default

    [Header("Wall Running Sound")]
    public AudioSource wallRunSource;
    public AudioClip wallRunClip;

    [Header("Wall Run Footsteps")]
    public AudioSource wallRunStepSource;
    public AudioClip[] wallRunStepClips;

    public float wallRunStepInterval = 0.3f;
    public float wallRunPitchStart = 0.95f;
    public float wallRunPitchEnd = 1.2f;
    public float wallRunVolumeStart = 0.7f;
    public float wallRunVolumeEnd = 1.0f;

    private float wallRunTimer;
    private float wallRunProgress;
    private bool wallRunActive;






    [Header("Landing")]
    public AudioClip landingClip;
    public float landingVolume = 1f;

    public void PlayLandingSound()
    {
        if (landingClip != null)
        {
            footstepSource.pitch = 1f;
            footstepSource.PlayOneShot(landingClip, landingVolume);
        }
    }

    [Header("Surface Footsteps")]
    public AudioClip[] metalClips;
    public AudioClip[] woodClips;
    public AudioClip[] grassClips;

    public AudioClip[] garbageClips;

    [Header("Slide Sound")]
    public AudioClip slideClip;

    [Header("Footstep Intervals")]
    public float walkInterval = 0.5f;
    public float runInterval = 0.35f;
    public float climbInterval = 0.6f;
    public float wallRunInterval = 0.3f;

    [Header("Surface Detection")]
    public LayerMask groundLayer; // Assign your ground layer here

    [Header("Refs")]
    public PlayerMovement movement;

    private float stepTimer;

    void Start()
    {
        stepTimer = walkInterval;

    }

    void Update()
    {
        HandleFootsteps();
        HandleSlideSound();
        HandleWallRunFootsteps();

    }

    void HandleFootsteps()
    {
        // Don’t step in air or when frozen
        if (!movement.grounded && !movement.climbing && !movement.wallrunning) return;
        if (movement.state == PlayerMovement.MovementState.freeze) return;

        bool isMoving = movement.GetVelocity().magnitude > 1f;

        if (!isMoving) return;

        float interval = walkInterval;

        switch (movement.state)
        {
            case PlayerMovement.MovementState.sprinting:
            case PlayerMovement.MovementState.sliding:
                interval = runInterval;
                break;
            case PlayerMovement.MovementState.wallrunning:
                interval = wallRunInterval;
                break;
            case PlayerMovement.MovementState.climbing:
                interval = climbInterval;
                break;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            PlayFootstepBasedOnSurface();
            stepTimer = interval;
        }
    }

    bool slidePlayed = false;

    void HandleSlideSound()
    {
        if (movement.state == PlayerMovement.MovementState.sliding)
        {
            if (!slidePlayed && slideClip != null)
            {
                slideSource.PlayOneShot(slideClip);
                slidePlayed = true;
            }
        }
        else
        {
            slidePlayed = false;
        }
    }




    void PlayFootstepBasedOnSurface()
    {
        AudioClip[] clipsToUse = null; // Start with no sound

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer))
        {
            string surfaceTag = hit.collider.tag;

            switch (surfaceTag)
            {
                case "Metal":
                    clipsToUse = metalClips;
                    break;
                case "Wood":
                    clipsToUse = woodClips;
                    break;
                case "Rock":
                    clipsToUse = grassClips;
                    break;
                case "Garbage":
                    clipsToUse = garbageClips;
                    break;
            }
        }

        // If we didn't find a matching tag or surface, skip playing
        if (clipsToUse == null || clipsToUse.Length == 0) return;

        int index = Random.Range(0, clipsToUse.Length);
        footstepSource.pitch = Random.Range(0.95f, 1.05f);
        footstepSource.PlayOneShot(clipsToUse[index]);
    }

        public void PlayLandingFootstepBasedOnSurface(float fallVelocity)
        {
            AudioClip[] clipsToUse = null;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer))
            {
                string surfaceTag = hit.collider.tag;

                switch (surfaceTag)
                {
                    case "Metal":
                        clipsToUse = metalClips;
                        break;
                    case "Wood":
                        clipsToUse = woodClips;
                        break;
                    case "Grass":
                        clipsToUse = grassClips;
                        break;
                    case "Garbage":
                        clipsToUse = garbageClips;
                        break;
                }
            }

            if (clipsToUse == null || clipsToUse.Length == 0) return;

            int index = Random.Range(0, clipsToUse.Length);

            // Calculate volume and pitch based on fall speed
            float intensity = Mathf.InverseLerp(2f, 15f, Mathf.Abs(fallVelocity)); // 2 = small jump, 15 = big drop
            float volume = Mathf.Lerp(0.8f, 2f, intensity); // Low to high volume
            float pitch = Mathf.Lerp(1.1f, 0.8f, intensity); // High to low pitch

            footstepSource.pitch = pitch;
            footstepSource.PlayOneShot(clipsToUse[index], volume);
        }

    void HandleWallRunFootsteps()
    {
        if (movement.state == PlayerMovement.MovementState.wallrunning)
        {
            wallRunActive = true;

            wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0f)
            {
                if (wallRunStepClips.Length == 0) return;

                // Get progression (optional curve)
                wallRunProgress = Mathf.Clamp01(wallRunProgress + Time.deltaTime);

                int index = Random.Range(0, wallRunStepClips.Length);
                wallRunStepSource.clip = wallRunStepClips[index];

                // Apply pitch/volume dynamics
                wallRunStepSource.pitch = Mathf.Lerp(wallRunPitchStart, wallRunPitchEnd, wallRunProgress);
                wallRunStepSource.volume = Mathf.Lerp(wallRunVolumeStart, wallRunVolumeEnd, wallRunProgress);

                wallRunStepSource.Play();

                wallRunTimer = wallRunStepInterval;
            }
        }
        else
        {
            // Just mark inactive — let last sound finish naturally
            if (wallRunActive)
            {
                wallRunActive = false;
                wallRunProgress = 0f;
                wallRunTimer = 0f;
            }
        }
    }




}
