using DG.Tweening;
using UnityEngine;


public class FootStep : MonoBehaviour
{
    [SerializeField]Material m_Material;
    [SerializeField] float duration = 5.0f;


    private void Awake()
    {
        
        m_Material = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(m_Material.DOVector(new Vector4(1.0f, 0.0f, 0.0f, 1.0f), "_Color", 0))
                .Append(m_Material.DOVector(new Vector4(0.0f, 0.0f, 0.0f, 0.0f), "_Color", duration));
    }
}
