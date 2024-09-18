using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private Image hpForegroundImage;

    private float maxHP;

    private void Awake()
    {
        GameManager.Instance.AddEvent(EEvent.GameStart, OnGameStart);
        GameManager.Instance.AddEvent(EEvent.ChangeHP, OnChangeHP);
    }

    private void OnGameStart(object param)
    {
        maxHP = ((GameData)param).maxHP;
        hpForegroundImage.fillAmount = 1f;
    }

    private void OnChangeHP(object param)
    {
        float hp = (float)param;
        hpForegroundImage.fillAmount = hp / maxHP;
    }
}
