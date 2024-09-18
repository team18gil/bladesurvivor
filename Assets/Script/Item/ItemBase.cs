using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemBase : MonoBehaviour
{
    [SerializeField] private bool isMagnetic = true;
    [SerializeField] private float followSpeed = 3f;
    private CharacterObject characterObject;

    private bool isFollowingCharacterObject;

    private void OnEnable()
    {
        isFollowingCharacterObject = false;
        characterObject = GameManager.Instance.CharacterObject;
    }

    private void FixedUpdate()
    {
        if (isMagnetic)
        {
            float distance = Vector2.Distance(characterObject.transform.localPosition, transform.localPosition);
            if (distance < characterObject.MagneticRange)
            {
                isFollowingCharacterObject = true;
            }

            if (isFollowingCharacterObject)
            {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition,
                    characterObject.transform.localPosition, followSpeed * Time.fixedDeltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnItemCollected();
            GameManager.Instance.SendEvent(EEvent.ItemCollected, this);
        }
    }

    protected virtual void OnItemCollected()
    {
        // do nothing
    }
}
