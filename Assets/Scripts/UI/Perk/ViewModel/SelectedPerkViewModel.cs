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

    private PerkData m_perkData;
    public PerkData PerkData
    {
        get { return m_perkData; }
        set
        {
            if(m_perkData != value)
            {
                m_perkData = value;
                OnPropChanged(nameof(PerkData));
            }
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
