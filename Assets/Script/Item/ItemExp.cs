using UnityEngine;

public class ItemExp : ItemBase
{
    [SerializeField] private int exp;

    protected override void OnItemCollected()
    {
        base.OnItemCollected();
        GameManager.Instance.SendEvent(EEvent.ItemAddExp, exp);
    }
}
