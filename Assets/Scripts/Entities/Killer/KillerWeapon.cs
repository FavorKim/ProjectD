using UnityEngine;

public class KillerWeapon : MonoBehaviour
{
    BoxCollider m_col;
    private void Start()
    {
        m_col = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Survivor survivor))
        {
            AttackSurvivor(survivor);
        }

    }

    void AttackSurvivor(Survivor sur)
    {
        sur.CmdGetHit();
        m_col.enabled = false;
    }
}
