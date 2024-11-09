using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    [SerializeField] private GameObject TextDmg;
    [SerializeField] private GameObject TextHeal;
    [SerializeField] private GameObject PhysicDefence;
    [SerializeField] private GameObject MagicDefence;
    [SerializeField] private GameObject DmgGroup;
    [SerializeField] private GameObject HealGroup;

    [SerializeField] private Image fill;
    [SerializeField] private Image redFill;

    [SerializeField] private Animator anim;
    
    private int _currentHealth;
    private int _maxHealth = 100;

    private Coroutine _showTimer;
    public void SetMaxHealth(int newHealth)
    {
        _maxHealth = newHealth;
        _currentHealth = newHealth;
        fill.fillAmount = 1;
        redFill.fillAmount = 0;
    }

    public void TakeDamage(int dmg, DamageType type, Effect fx)
    {
        if(_showTimer != null)
            StopCoroutine(_showTimer);
        
        RectTransform empty = new GameObject().AddComponent<RectTransform>();
        empty.SetParent(DmgGroup.transform);
        empty.anchorMin = new Vector2(0,0);
        empty.anchorMax = new Vector2(0,0);
        empty.localScale = new Vector3(1, 1, 1);
        
        var obj = Instantiate(TextDmg, new Vector3(0,0,0), Quaternion.identity, empty.transform).GetComponent<RectTransform>();
        obj.anchoredPosition3D = new Vector3(0, 0, 0);
        obj.anchorMin = new Vector2(0.5f,0.5f);
        obj.anchorMax = new Vector2(0.5f,0.5f);
        empty.anchoredPosition3D = new Vector3(Random.Range(0, 1040), Random.Range(0, 450), 0);
        empty.localRotation = Quaternion.Euler(0, 0, 0);
        
        var text = obj.GetComponentInChildren<TMP_Text>();
        text.text = "-" + dmg;

        text.color = fx switch
        {
            Effect.Pyro => new Color(0.922f, 0.345f, 0.204f),
            Effect.Electro => new Color(0.149f, 0, 0.82f),
            Effect.Freeze => new Color(0.122f, 0.678f, 1f),
            Effect.Alchemist => new Color(0.008f, 0.851f, 0.149f),
            _ => text.color
        };
        _currentHealth -= dmg;
        fill.fillAmount = (float)_currentHealth/_maxHealth;
        redFill.fillAmount = 1 - fill.fillAmount;
        if (gameObject.activeSelf)
        {
            _showTimer = StartCoroutine(ShowAndHide());
            StartCoroutine(LowerOpacity(text));
        }

        Destroy(empty.gameObject, 2f);

        
    }

    public void Defence(DamageType type)
    {
        RectTransform empty = new GameObject().AddComponent<RectTransform>();
        empty.SetParent(DmgGroup.transform);
        empty.anchorMin = new Vector2(0,0);
        empty.anchorMax = new Vector2(0,0);
        empty.localScale = new Vector3(1, 1, 1);
        
        var text = Instantiate(type == DamageType.Physical ? PhysicDefence : MagicDefence, Vector3.zero, Quaternion.identity, empty.transform).GetComponent<TMP_Text>();
        text.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
        text.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f,0.5f);
        text.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f,0.5f);
        empty.anchoredPosition3D = new Vector3(Random.Range(0, 1040), Random.Range(0, 450), 0);
        empty.localRotation = Quaternion.Euler(0, 0, 0);
        
        _showTimer = StartCoroutine(ShowAndHide());
        StartCoroutine(LowerOpacity(text));
        Destroy(empty.gameObject, 2f);
    }

    public void Heal(int value)
    {
        RectTransform empty = new GameObject().AddComponent<RectTransform>();
        empty.SetParent(HealGroup.transform);
        empty.anchorMin = new Vector2(0,0);
        empty.anchorMax = new Vector2(0,0);
        empty.localScale = new Vector3(1, 1, 1);
        
        var obj = Instantiate(TextHeal, Vector3.zero, Quaternion.identity, empty.transform).GetComponent<RectTransform>();
        obj.anchoredPosition3D = new Vector3(0, 0, 0);
        obj.anchorMin = new Vector2(0.5f,0.5f);
        obj.anchorMax = new Vector2(0.5f,0.5f);
        empty.anchoredPosition3D = new Vector3(Random.Range(0, 1040), Random.Range(0, 450), 0);
        empty.localRotation = Quaternion.Euler(0, 0, 0);

        var text = obj.GetComponentInChildren<TMP_Text>();
        text.text = "+" + value;
        _currentHealth += value;
        fill.fillAmount = (float)_currentHealth/_maxHealth;
        redFill.fillAmount = 1 - fill.fillAmount;
        
        _showTimer = StartCoroutine(ShowAndHide());
        StartCoroutine(LowerOpacity(text));
        Destroy(empty.gameObject, 2f);
    }


    private IEnumerator ShowAndHide()
    {
        anim.SetBool("Show", true);
        yield return new WaitForSeconds(3f);
        anim.SetBool("Show", false);
    }
    private IEnumerator LowerOpacity(TMP_Text text)
    {
        yield return new WaitForSeconds(0.25f);
        float time = 0f;
        while (time < 1.5f)
        {
            text.alpha -= 0.0667f;
            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
        }
    }
}
