using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    GameObject UI;
    [SerializeField] private bool openable;
    [SerializeField] private bool islocked;
    [SerializeField] private string unlockRequirement;
    [SerializeField] private float countDown;
    [SerializeField] AudioClip[] audioClips;
    private GameObject player;
    private Animator animator;
    private float currentCountDown;
    private bool playerEnter;
    private bool previousPlayerOpen;
    // Start is called before the first frame update
    void Awake()
    {
        openable = true;
        currentCountDown = countDown;
        animator = GetComponent<Animator>();
        playerEnter = false;
        UI = GameObject.Find("Canvas");
    }
    private void Update()
    {
        if (openable)
        {
            if (playerEnter && Input.GetKeyDown(KeyCode.E))
            {
                if (islocked && GameObject.Find("PlayerInventary").GetComponent<PlayerInventary>().CheckItem(this.unlockRequirement) != null)
                {// if the door is locked by key
                    UI.GetComponent<MsgDisplayer>().SetMessage("Door Unlocked");
                    UnlockDoor();
                }
                if (!islocked)
                {
                    //Debug.Log("Door Status:" + animator.GetBool("DoorOpen"));
                    ChangeDoorStatus();
                    if (animator.GetBool("DoorOpen"))// if door is open now mean player has open the door
                    {
                        previousPlayerOpen = true;
                        currentCountDown = countDown;
                    }
                    else // player actually close the door
                    {
                        previousPlayerOpen = false;
                    }
                }
                else
                {

                    UI.GetComponent<MsgDisplayer>().SetMessage("The door is locked, I need " + unlockRequirement);
                }
            }
            else
            {
                if (previousPlayerOpen == true && animator.GetBool("DoorOpen"))//if player has open this door before.
                {
                    currentCountDown -= Time.deltaTime;
                    if (currentCountDown <= 0)
                    {
                        if (animator.GetBool("DoorOpen"))// if the door is currently open
                        {
                            ChangeDoorStatus();
                        }
                        currentCountDown = countDown;
                        previousPlayerOpen = false;
                    }
                }

            }
        }
        else
        {
            if (playerEnter && Input.GetKeyDown(KeyCode.E))
                UI.GetComponent<MsgDisplayer>().SetMessage("I cannot open it!");
        }
    }
    public void SetDoorOpenable(bool result)
    {
        this.openable = result;
    }
    public void SetDoorLock(bool result,string requirement)
    {
        if (this.openable)
        {
            this.islocked = result;
            this.unlockRequirement = requirement;
            if(islocked)
                animator.SetBool("DoorOpen", false);
        }
    }
    public void UnlockDoor()
    {
        this.islocked = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            this.player = other.transform.parent.gameObject;
            playerEnter = true;
        }
        else if(other.tag == "Killer" && !islocked)// if killer approach the door,door will automaticlly open
        {
            if(animator != null)
                animator.SetBool("DoorOpen", true);
        }
    }
    private void OnTriggerExit(Collider other)// if killer approach the door,door will automaticlly open
    {
        if (other.tag == "Player")
        {
            this.player = null;
            playerEnter = false;
        }
        else if (other.tag == "Killer")
        {
            if (animator != null)
                animator.SetBool("DoorOpen", false);
        }
    }
    public void ChangeDoorStatus()
    {
        animator.SetBool("DoorOpen", !animator.GetBool("DoorOpen"));
        if (animator.GetBool("DoorOpen"))
        {
            GetComponent<AudioSource>().clip = audioClips[0];
        }
        else
        {
            GetComponent<AudioSource>().clip = audioClips[1];
        }
        GetComponent<AudioSource>().Play();
    }
    // Update is called once per frame
    public void playSound()
    {
        if (animator.GetBool("DoorOpen"))
        {
            GetComponent<AudioSource>().clip = audioClips[0];
        }
        else
        {
            GetComponent<AudioSource>().clip = audioClips[1];
        }
        GetComponent<AudioSource>().Play();
    }

    public void PuzzleSolvedDoor()
    {
        animator.SetBool("DoorOpen", true);
    }
}
