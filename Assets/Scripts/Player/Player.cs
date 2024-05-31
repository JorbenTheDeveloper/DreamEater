using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private float currentHP;
    public float MaxHP;

    public float maxGrow = 4;
    public float Size => transform.localScale.x;
    public float StartingSize = 1.2f;

    private PlayerMovement playerMovement;
    private PlayerShoot playerShoot;
    public float sizeReduction = 0.1f;

    public float CurrentHP => currentHP;
    public CinemachineVirtualCamera cinemachineCamera;
    public CinemachineCameraShake CinemachineCameraShake;
    public UnityEngine.Color DamagedColor = UnityEngine.Color.red;

    public ParticleSystem growthParticles;
    public float particleSystemScale = 1f;
    public ParticleSystem BloodParticleFx;
    public Image VignetteImage;

    public bool IsVulnerable => playerMovement.HasExhausted();

    private UnityEngine.Color originalColor;

    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerShoot = GetComponent<PlayerShoot>();
        currentHP = MaxHP;
        transform.localScale = new Vector3(StartingSize, StartingSize, 1);

        StartCoroutine(AdjustCameraSize(1));
        
    }

    private void Update()
    {
        
    }

    public void TakeDamage(int damageValue)
    {
        CinemachineCameraShake.Shake();
        AudioManager.Instance.Play("Injury");
        spriteRenderer.color = DamagedColor;
        VignetteImage?.gameObject.SetActive(true);
		Invoke(nameof(OriginalColor), 0.2f);
        currentHP -= damageValue;
        currentHP = Mathf.Clamp(currentHP, 0, MaxHP);

        if (currentHP <= 0)
        {
            SceneManager.LoadScene("Retry");
        }
    }

    void OriginalColor()
    {
		VignetteImage?.gameObject.SetActive(false);
		spriteRenderer.color = originalColor;
    }

    public void ShowBloodParticle()
    {
        BloodParticleFx.Stop();
        BloodParticleFx.Play();
	}

    public bool CanEat(Eatable eatable)
    {
        return Size >= eatable.Size || eatable.IgnoreSize;
	}

    public void TryEat(Eatable eatable)
    {
        if (!playerMovement.IsRushing) return;

        if (CanEat(eatable))
        {
            eatable.TakeDamage();

            if (IsMaxGrow() == false)
            {
                Grow(eatable.growRate);

            }
        }
    }

    private bool IsMaxGrow()
    {
        return transform.localScale.x >= maxGrow;
    }

    public void Shrink(float value)
    {
        // Calculate the new size but do not allow it to go below 0.5
        float newSize = Mathf.Max(0.5f, Size - value);
        transform.localScale = new Vector3(newSize, newSize, 1.0f); // Assuming uniform scaling for simplicity

        // Optionally, adjust the camera size or other related attributes here
        StartCoroutine(AdjustCameraSize(1));
    }
    private void Grow(float value)
    {
        if (!IsMaxGrow())
        {
            
            float previousSize = Size;
            float newSize = Mathf.Min(Size + value, maxGrow);

            // Apply the new size to the player.
            transform.localScale = new Vector3(newSize, newSize, 1);
            StartCoroutine(AdjustCameraSize(1)); // Adjust the camera size if needed.

            // Calculate how much the player has grown and update the particle scale.
            float growthIncrement = newSize - previousSize;
            UpdateParticleSystemScale(growthIncrement);

            StartGrowthParticles(); // Ensure particles are playing.
        }
    }

    private void UpdateParticleSystemScale(float growthIncrement)
    {
        // Check if the growth is at least 0.1
        if (growthIncrement >= 0.1f)
        {
            int increments = Mathf.FloorToInt(growthIncrement / 0.1f);
            particleSystemScale += increments; // Add 1 to the scale for each 0.1 increment.
            growthParticles.transform.localScale = new Vector3(particleSystemScale, particleSystemScale, particleSystemScale);
        }
    }

    private void StartGrowthParticles()
    {
        if (growthParticles != null)
        {
            growthParticles.Play(); // Start playing the particle effect
            growthParticles.transform.localScale = Vector3.one * Size * 3;
			Invoke("StopGrowthParticles", 2f); // Schedule the stopping of the particle system after 3 seconds
        }
    }

    private void StopGrowthParticles()
    {
        if (growthParticles != null)
        {
            growthParticles.Stop(); 
        }
    }

    IEnumerator AdjustCameraSize(float totalTime)
    {
        if (cinemachineCamera != null)
        {
            float sizeRatio = transform.localScale.x / StartingSize;
            float baseOrthographicSize = 6 * (StartingSize / 1f);
            float passedTime = 0;

            while (passedTime < totalTime)
            {
                float step = Mathf.SmoothStep(cinemachineCamera.m_Lens.OrthographicSize, baseOrthographicSize * sizeRatio, passedTime / totalTime);
                cinemachineCamera.m_Lens.OrthographicSize = step;
                passedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
