using CustomInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Config")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minX = -5.8f;
    [SerializeField] private float maxX = 5.8f;

    [Header("Components")] 
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private Transform skinHolder;

    private bool _isDead;
    private float _screenWidth;
    private float _screenHeight;

    private void Awake()
    {
        
    }

    void Start()
    {
        SetSkin(Data.PlayerData.CurrentSkin);
        
        Observer.EquipPlayerSkin += SetSkin;
    }

    private void OnDestroy()
    {
        Observer.EquipPlayerSkin -= SetSkin;
    }
    
    void Update()
    {
        if (_isDead) return;
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        
        Vector3 newPosition = transform.position;
        newPosition.x += move;
        
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        
        transform.position = newPosition;

    }
    
    public void MoveLeft()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    public void MoveRight()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Nuke"))
        {
            GameManager.Instance.OnLoseGame();
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        gameObject.SetActive(false);
    }

    private void SetSkin(string itemIdentity)
    {
        var item = ItemController.Instance.GetItemData(itemIdentity);
        var skinPrefab = item.skinPrefab;
        skinHolder.Clear();
        Instantiate(skinPrefab, skinHolder);
    }

    [Button]
    public void RandomSkin()
    {
        var item = ItemController.Instance.GetRandomItemData();
        var skinPrefab = item.skinPrefab;
        
        skinHolder.Clear();
        Instantiate(skinPrefab, skinHolder);
        
        Data.PlayerData.CurrentSkin = item.identity;
    }
}
