using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemptyTool.ES_MessageSystem;

public class HeroController : MonoBehaviour
{
    public static HeroController instance;

    // health system
    public int maxHealth = 5;
    public int health { get { return currentHealth; } } // get and set method for health
    int currentHealth;

    // hero speed
    public float heroSpeed = 3.0f;

    // invincible system
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    // animation system
    Animator animator;
    public Vector2 lookDirection = new Vector2(0, -1);

    // physic system
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    // Inventory system
    public Inventory inventory;
    public bool inventory_open = false;
    [SerializeField] UI_Inventory uiInventory;

    public bool dialogue_open = false;
    public bool isTransporting = false;

    #region Game Loop
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        // adjust frame number
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        // getComponets
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        //currentHealth = 1;

        Inventory.instance = new Inventory();
        inventory = Inventory.instance;
        if (uiInventory)
            uiInventory.SetInventory(inventory);

    }

    // Update is called once per frame
    void Update()
    {
        // input detection
        GetInput();
        // communication with animator
        ManageAmination();
        // invincible timer
        ManageInvinciale();
        // raycasting to detect object
        TriggerDialog();
        ShowInventory();
    }

    // Update is called via physics system
    void FixedUpdate()
    {
        Move();
    }

    #endregion

    //functions
    #region Private functions
    private void GetInput()
    {
        if (!inventory_open && !ES_MessageSystem.instance.IsDoingTextTask && !isTransporting)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        else
            horizontal = vertical = 0;
    }

    private void Move()
    {
        if (ES_MessageSystem.instance.IsDoingTextTask || inventory_open || isTransporting) return;
        Vector2 position = rigidbody2d.position;
        position.x += heroSpeed * horizontal * Time.deltaTime;
        position.y += heroSpeed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    private void TriggerDialog()
    {
        if (!inventory_open && !isTransporting && Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 0.5f, LayerMask.GetMask("npc"));
            if (hit.collider != null)
            {
                NewDialogueTrigger dialogueTrigger = hit.collider.GetComponent<NewDialogueTrigger>();
                if (dialogueTrigger != null)
                {
                    dialogueTrigger.TriggerDialogue();
                }
            }
        }
    }

    public void ShowInventory()
    {
        if (!ES_MessageSystem.instance.IsDoingTextTask && !isTransporting && Input.GetKeyDown(KeyCode.X))
        {
            if (inventory_open)
            {
                uiInventory.gameObject.SetActive(false);
                inventory_open = false;
            }
            else
            {
                uiInventory.SelectFirstButton();
                uiInventory.gameObject.SetActive(true);
                inventory_open = true;
            }
        }
    }

    private void ManageAmination()
    {
        Vector2 move = new Vector2(horizontal, vertical);
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
    }

    private void ManageInvinciale()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
    }

    #endregion

    #region Public functions
    public void ChangeHealth(int amount)
    {
        if(amount < 0)
        {
            if (isInvincible) return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    #endregion

    #region Events
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("portal"))
        {
            Portal p = collision.gameObject.GetComponent<Portal>();
            p.Transport(transform);
        }
    }
    
    #endregion
}
