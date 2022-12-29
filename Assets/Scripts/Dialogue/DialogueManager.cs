using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.InputSystem;


public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private Story currentStory;
    public bool dialogueIsPlaying;

    private bool talkPressed;
    private PlayerActionsScript playerActionsScript;


    private void OnEnable()
    {
        InitPlayerInput();
        ConfigPlayerInput();
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
        dialoguePanel = GameObject.Find("DialoguePanel");
        dialogueText = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();

        
        if (instance != null)
        {
            Debug.LogWarning("More than one DialogueManager found");
        }
        instance = this;
        talkPressed = false;
        
    }

    public static DialogueManager GetInstance() {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    public void EnterDialogueMode(TextAsset inkJSON) {
        Debug.Log("enter Dialogue");
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        ContinueStory();
    }

    public void ExitDialogueMode() {
        Debug.Log("exit Dialogue");
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogueIsPlaying) {
            return;
        }

        if (talkPressed) {
            talkPressed = false;
            ContinueStory();
        }
    }

    public void ContinueStory() 
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    public void Talk(InputAction.CallbackContext context)
    {
        talkPressed = true;
        // if (!currentStory.canContinue) {
        //     ExitDialogueMode();
        // }
    }
}
