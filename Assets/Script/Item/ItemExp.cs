using UnityEngine;

public class ItemExp : ItemBase
{
    [SerializeField] private float exp;

    protected override void OnItemCollected()
    {
        base.OnItemCollected();
        GameManager.Instance.SendEvent(EEvent.ItemAddExp, exp);
    }
}
