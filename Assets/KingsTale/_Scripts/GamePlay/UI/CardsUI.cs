using UnityEngine;

public class CardsUI : MonoBehaviour
{
    public GameObject[] Cards;
    public GameObject[] Infos;
    [SerializeField] private GameObject _hideCards;
    
    public void ActivateCard(GameObject _selectedCard)
    {

        foreach (var _card in Cards)
        {
            var anim = _card.GetComponent<Animator>();
            if (_selectedCard == _card && _card.GetComponent<UnitsGroupButton>().Group.Count != 0)
            {
                anim.Play("ScaleCard");
            }
            else if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "DefaultCard")
            {
                anim.Play("Disscale");
            }
        }
    }

    public void DeselectCard(GameObject _selectedCard){
        _selectedCard.GetComponent<Animator>().Play("Disscale");
    }
    public void DeselectAll(){
        foreach (var _card in Cards)
        {
            var anim = _card.GetComponent<Animator>();
            if (anim.GetCurrentAnimatorClipInfo(0).Length != 0 && anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "DefaultCard")
            {
                anim.Play("Disscale");
            }
        }
    }

    public void HideInfo()
    {
        foreach (var info in Infos)
        {
            info.SetActive(false);
        }
    }
}
