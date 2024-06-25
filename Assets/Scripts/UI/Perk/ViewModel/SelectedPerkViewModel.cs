using System.ComponentModel;

public class SelectedPerkViewModel
{
    public int index;

    private bool m_isSelected;
    public bool IsSelected
    {
        get { return m_isSelected; }
        set
        {
            if (m_isSelected != value)
            {
                m_isSelected = value;
                OnPropChanged(nameof(IsSelected));
            }
        }
    }

    private PerksScriptableObject m_perkSCO;
    public PerksScriptableObject PerkSCO
    {
        get { return m_perkSCO; }
        set
        {
            if(m_perkSCO != value)
            {
                m_perkSCO = value;
                OnPropChanged(nameof(PerkSCO));
            }
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
