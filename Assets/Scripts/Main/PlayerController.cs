using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ParticleSystem bloodSplatterVFX;

    public AudioClip[] knifeSFX;
    
    private Rigidbody playerRb;
    private GameManager gameManager;
    private AudioSource knifeAudio;

    private float yRot;
    private float yRotBounds = 70.0f;
    [SerializeField] float aimSpeed;
    [SerializeField] float launchSpeed;
    [SerializeField] float spinSpeed;

    private Vector3 murderLightOffset = new Vector3(0, 8, 0.3f);
    private Vector3 sfxOffset = new Vector3(0, 0, -1f);

    // Enum corresponds to order audio clips appear in AudioClip array 
    enum AudioFiles {knifeDraw, knifeThrow, knifeHit, knifeBounce, knifeDrop };

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        knifeAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive)
        {
            // Aim and launch player knife
            if (gameManager.isAiming)
            {
                // Rotate knife
                yRot += Input.GetAxis("Horizontal") * Time.deltaTime * aimSpeed;
                yRot = Mathf.Clamp(yRot, -yRotBounds, yRotBounds);
                transform.rotation = Quaternion.Euler(transform.rotation.x, yRot, transform.rotation.z);

                // Launch knife
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    gameManager.isAiming = false;
                    playerRb.AddForce(transform.forward * launchSpeed, ForceMode.Impulse);
                    knifeAudio.PlayOneShot(knifeSFX[(int)AudioFiles.knifeThrow], 0.5f);

                    // Change spin direction based on angle of launch
                    if (yRot >= 0)
                    {
                        playerRb.AddTorque(Vector3.up * spinSpeed);
                    }
                    else if (yRot < 0)
                    {
                        playerRb.AddTorque(Vector3.up * -spinSpeed);
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        // Have spotlight follow the knife after viewing time is up
        if (!gameManager.isViewing)
        {
            gameManager.murderLight.transform.position = transform.position + murderLightOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If knife hits target
        if (other.gameObject.CompareTag("Target"))
        {
            knifeAudio.PlayOneShot(knifeSFX[(int)AudioFiles.knifeHit], 0.5f);
            var tempLoc = other.transform.position + sfxOffset;
            bloodSplatterVFX.gameObject.transform.position = tempLoc;
            bloodSplatterVFX.Play();
            other.gameObject.transform.Translate(0, -10, 0);
            Destroy(other.gameObject, 0.5f);
        }
        // If knife goes out of bounds, reset
        else if (other.gameObject.CompareTag("ResetKnife"))
        {
            knifeAudio.PlayOneShot(knifeSFX[(int)AudioFiles.knifeDrop], 0.5f);
            gameManager.attemptNum++;
            gameManager.uiManager.UpdateAttemptsText(gameManager.attemptNum);
            gameManager.isAiming = true;
            ResetKnife();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Play bounce audio
        if (collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Barrier"))
        {
            knifeAudio.PlayOneShot(knifeSFX[(int)AudioFiles.knifeBounce], 0.2f);
        }
    }

    // Reset the position of the knife for relaunch
    public void ResetKnife()
    {
        bloodSplatterVFX.Stop();
        knifeAudio.PlayOneShot(knifeSFX[(int)AudioFiles.knifeDraw], 0.5f);
        transform.position = new Vector3(0, 0.5f, -5.5f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
    }
}
