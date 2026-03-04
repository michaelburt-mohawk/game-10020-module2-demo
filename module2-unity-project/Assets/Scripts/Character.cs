using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [Header("Physics Parameters")]
    public float moveSpeed = 5.0f;
    public float gravity = -9.81f;
    public float throwForce = 1.0f;

    [Header("Prefabs")]
    public Rock rockPrefab;

    [Header("Input Actions")]
    public InputActionReference moveInput;
    public InputActionReference attackInput;
    public InputActionReference weaponSwitchInput;
    public InputActionReference showInventoryInput;
    public InputActionReference dropInventoryInput;

    [Header("Weapons")]
    public Shovel shovel;
    public Rock rock;
    public GameObject armRight;

    [HideInInspector]
    public UnityEvent<bool> OnInventoryShown;

    [HideInInspector]
    public UnityEvent OnItemDropped;

    CharacterController controller;
    Vector3 velocity;
    Animator animator;
    Collider shovelCollider;

    bool shortRangeAttack = true;
    bool startRockSpawn = false;
    float rockTimer = 0.0f;

    bool showInventory = false;

    void Awake()
    {
        if (OnInventoryShown == null)
            OnInventoryShown = new UnityEvent<bool>();

        if (OnItemDropped == null)
            OnItemDropped = new UnityEvent();

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        shovelCollider = shovel.GetComponent<Collider>();
        shovelCollider.enabled = false;

        shortRangeAttack = true;

        showInventoryInput.action.performed += ShowInventoryPerformed;
        showInventoryInput.action.canceled += ShowInventoryCanceled;

        dropInventoryInput.action.performed += DropInventoryPerformed;
    }

    private void DropInventoryPerformed(InputAction.CallbackContext obj)
    {
        OnItemDropped.Invoke();
    }

    private void ShowInventoryCanceled(InputAction.CallbackContext obj)
    {
        showInventory = false;
        OnInventoryShown.Invoke(showInventory);
    }

    private void ShowInventoryPerformed(InputAction.CallbackContext obj)
    {
        showInventory = true;
        OnInventoryShown.Invoke(showInventory);
    }

    private void Start()
    {
        UpdateWeapon();
    }

    private void Update()
    {
        bool inputEnabled = !showInventory;

        PlayerMotion(inputEnabled);

        bool attack = attackInput.action.WasPressedThisFrame();
        if (attack && inputEnabled)
        {
            if (shortRangeAttack)
            {
                // short range attack always works (no cooldown)
                animator.SetTrigger("StartAttack");
            }
            else
            {
                // only throw the rock if it exists
                if (rock != null)
                {
                    ThrowRock();
                    animator.SetTrigger("StartAttack");
                }
            }
        }

        bool weaponSwitch = weaponSwitchInput.action.WasPressedThisFrame();
        if (weaponSwitch && inputEnabled)
        {
            shortRangeAttack = !shortRangeAttack;
            UpdateWeapon();
        }

        // rock throwing cooldown code
        if (startRockSpawn) SpawnRockDelay();
    }

    void PlayerMotion(bool inputEnabled)
    {
        if (!inputEnabled) return;

        // the following is pretty standard character controller code
        
        // snap the player to the ground if already grounded
        // when jumping, gravity takes over
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 moveDirection = moveInput.action.ReadValue<Vector2>();

        Vector3 move = Vector3.right * moveDirection.x + Vector3.forward * moveDirection.y;
        Vector3 moveVelocity = move * moveSpeed;

        // allow gravity to impact y velocity
        velocity.y += gravity * Time.deltaTime;

        moveVelocity.y = velocity.y;
        
        // finally, Move the character
        controller.Move(moveVelocity * Time.deltaTime);


        // rotate the character using Quaternion LookRotation()
        // slerp = Spherical Linear Interpolation. smoothly interpolates between Quaternion rotations
        Vector3 horizontalVelocity = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
        if (horizontalVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                15f * Time.deltaTime
            );
        }

        // set the speed for the animator to change idle/walk states
        animator.SetFloat("Speed", horizontalVelocity.magnitude);
    }

    void ThrowRock()
    {
        if (rock != null)
        {
            Vector3 point1 = rock.transform.position;
            point1.y = 0f;
            Vector3 point2 = GetMouseHitPoint();
            point2.y = 0f;

            Vector3 direction = (point2 - point1).normalized;
            
            rock.Throw(direction, throwForce);
            rock = null;
            startRockSpawn = true;
        }
    }

    // standard screen raycast code - gets a world space position based on mouse click
    Vector3 GetMouseHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    void SpawnRockDelay()
    {
        rockTimer += Time.deltaTime;
        if (rockTimer >= 0.5f)
        {
            rockTimer = 0.0f;
            startRockSpawn = false;
            rock = Instantiate(rockPrefab, armRight.transform);

            // this fixes a timing issue where rock is spawned after 
            // weapon is switched
            if (shortRangeAttack) rock.gameObject.SetActive(false);
        }
    }

    void UpdateWeapon()
    {
        shovel.gameObject.SetActive(shortRangeAttack);
        
        // we need to check if rock is not null in case we have already thrown in
        // and it hasn't respawned yet
        if (rock != null) rock.gameObject.SetActive(!shortRangeAttack);
        
        if (shortRangeAttack)
        {
            // disable hitbox initially in case animation player did not do it for us
            // (animation didn't finish)
            shovel.EnableHitbox(0);
        }
        else
        {
            if (rock != null) rock.EnableHitbox(0);
        }
    }

    // this method is called from the animation player
    public void EnableHitbox(int value)
    {
        // only the shovel should be enabled from the swinging animation
        if (shortRangeAttack)
        {
            shovel.EnableHitbox(value);
        }
    }
}
