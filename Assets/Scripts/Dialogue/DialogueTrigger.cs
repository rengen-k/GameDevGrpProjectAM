using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;
    private PlayerActionsScript playerActionsScript;
    private bool talkPressed;
    [Header("Normal and Hard Difficulty")]
    [SerializeField] private TextAsset inkJSON;

    private AudioSource soundManager;
    public AudioClip dialogueStart;

    [Header("Easy Difficulty")]
    [SerializeField] private TextAsset inkJSONeasy;
    private GameState gameState;
    private int diff;


    private bool playerInRange;

    private void OnEnable()
    {
        InitPlayerInput();
        ConfigPlayerInput();
        try
        {
            soundManager = GameObject.FindWithTag("SoundEffects").GetComponent<AudioSource>();
        } catch {
            soundManager = GameObject.FindWithTag("LevelSoundEffects").GetComponent<AudioSource>();
        }
    }

    private void InitPlayerInput() 
    {
        playerActionsScript = new PlayerActionsScript();
        playerActionsScript.Player.Enable();
    }

    private void ConfigPlayerInput() 
    {
        playerActionsScript.Player.Talk.performed += Talk;
    }

    private void Awake()
    {
        gameState = GameObject.Find("GlobalGameState").GetComponent<GameState>();
        diff = gameState.GetDifficulty();
        playerInRange = false;
        talkPressed = false;
        visualCue.SetActive(false);
    }

    private void Update()
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying) {
            playerActionsScript.Player.Talk.Disable();
        }

        if (playerInRange && DialogueManager.GetInstance().IsTriggerCalled(gameObject))
        {
            visualCue.SetActive(true);
            if (talkPressed)
            {
                talkPressed = false;
                if (!DialogueManager.GetInstance().dialogueIsPlaying)
                {
                    if (gameState.IsEasy())
                    {
                        DialogueManager.GetInstance().EnterDialogueMode(inkJSONeasy, gameObject);
                    }
                    else
                    {
                        DialogueManager.GetInstance().EnterDialogueMode(inkJSON, gameObject);  
                    }
                }
            }
        }
        else
        {
            visualCue.SetActive(false);
            // DialogueManager.GetInstance().ExitDialogueMode();
            // if (DialogueManager.GetInstance().dialogueIsPlaying) { 
            //     DialogueManager.GetInstance().ExitDialogueMode();
            // }
        }

        if (!DialogueManager.GetInstance().dialogueIsPlaying) {
            talkPressed = false;
            StartCoroutine(DialogueCooldown());
        }
    }

    public void Talk(InputAction.CallbackContext context)
    {
        soundManager.PlayOneShot(dialogueStart);
        if (playerInRange) {
            talkPressed = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DialogueManager.GetInstance().IsTriggerCalled(gameObject)) {
            return;
        }
        DialogueManager.GetInstance().SetTriggerCalled(gameObject);
        if (other.gameObject.name == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!DialogueManager.GetInstance().IsTriggerCalled(gameObject)) {
            return;
        }
        DialogueManager.GetInstance().RemoveTriggerCalled(gameObject);
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
        }

    }
    
    private IEnumerator DialogueCooldown()
    {
        playerActionsScript.Player.Talk.Disable();
        yield return new WaitForSeconds(0.1f);
        playerActionsScript.Player.Talk.Enable();
    }
    
}
