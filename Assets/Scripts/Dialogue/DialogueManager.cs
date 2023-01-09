using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.InputSystem;

//-----------------------------------------//
// DialogueTriggerAuto
//-----------------------------------------//
// Base code from https://www.youtube.com/watch?v=vY0Sk93YUhA and heavily modified.
// Manages dialogue to ensure that the UI is displayed when it is active, and not displayed when it is not active.
// Governs correct dialogue, and only ensures 1 piece of dialogue is active at a time
// Keeps track of all Dialogue NPCs in a level

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private bool resetToLevel0;

    private List<GameObject> triggeredNPCs = new List<GameObject>();
    private DialogueTrigger[] allNPCObjectsInteract;
    private DialogueTriggerAuto[] allNPCObjects;
    private DialogueTriggerAutoDestroy[] allNPCObjectsDestroy;
    private GameObject[] allNPCs;
    private SceneSwitch sceneSwitch;

    private Story currentStory;
    public bool dialogueIsPlaying;

    private bool talkPressed;
    private PlayerActionsScript playerActionsScript;

    private void OnEnable()
    {
        sceneSwitch = GameObject.Find("LevelLoader").GetComponent<SceneSwitch>();
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

    // Finds all the NPC objects in a level and stores them in a list
    private void Awake()
    {
        allNPCObjectsInteract = FindObjectsOfType<DialogueTrigger>();
        allNPCObjects = FindObjectsOfType<DialogueTriggerAuto>();
        allNPCObjectsDestroy = FindObjectsOfType<DialogueTriggerAutoDestroy>();
        allNPCs = allNPCObjects.Select(s => s.gameObject).ToArray().Concat(allNPCObjectsDestroy.Select(s => s.gameObject)).ToArray().Concat(allNPCObjectsInteract.Select(s => s.gameObject)).ToArray();
        
        dialoguePanel = GameObject.Find("DialoguePanel");
        dialogueText = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();

        // Singleton
        if (instance != null && instance != this)
        {
            Debug.LogWarning("More than one DialogueManager found");
            Destroy(this.gameObject);
        } 
        else
        {
            instance = this;
        }
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

    // Makes the UI active and continues the story
    public void EnterDialogueMode(TextAsset inkJSON, GameObject npc) {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        ContinueStory();
    }

    // Exits the dialogue
    public void ExitDialogueMode() {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        if (resetToLevel0)
        {
            sceneSwitch.LoadLevel0();
        }
    }

    // If the talk key is pressed, then continue the story
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
    }


    // Methods related to keeping track of the NPCs in a level
    public bool IsTriggerCalled(GameObject npc)
    {
        
        return triggeredNPCs.Contains(npc);
    }

    public void SetTriggerCalled(GameObject npc)
    {
        triggeredNPCs.Add(npc);
        foreach (var n in allNPCs)
        {
            if (n != npc && n != null && n.GetComponent<DialogueTrigger>() == null) {
                n.SetActive(false);
            }
        }
    }

    public void RemoveTriggerCalled(GameObject npc)
    {
        foreach (var n in allNPCs)
        {
            if (n != null) {
                n.SetActive(true);
            }
        }
        if (npc.GetComponent<DialogueTriggerAutoDestroy>() != null) {
            Destroy(npc);
        }
        triggeredNPCs.Remove(npc);
    }

    public bool isDialoguePlaying()
    {
        return dialogueIsPlaying;
    }
}
