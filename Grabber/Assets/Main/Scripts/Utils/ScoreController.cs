using TTP.Toys;
using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private int _score;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Toy toy))
        {
            if(toy.IsDestroying) return;
            if(toy.IsInBusket) return;
            
            Debug.Log($"OnTriggerEnter {toy.name}");
            toy.IsInBusket = true;
            _score += toy.Score;
            textField.text = _score.ToString();
        }
    }
}
 