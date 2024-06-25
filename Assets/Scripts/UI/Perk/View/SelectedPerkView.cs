using System.ComponentModel;
using UnityEngine;
using PerkVMExtension;

public class SelectedPerkView : MonoBehaviour
{
    public int index;
    [SerializeField] private GameObject SelectedMarker;
    [SerializeField] Perk perk;

    private SelectedPerkViewModel vm;

    private void OnEnable()
    {
        if (vm == null)
        {
            vm = new();
            vm.PropertyChanged += OnPropChanged;
            vm.RegisterOnEnable();
        }
    }
    private void OnDisable()
    {
        if (vm != null)
        {
            vm.UnRegisterOnDisable();
            vm.PropertyChanged -= OnPropChanged;
            vm = null;

        }
    }

    public void SetMarkerActive(bool onOff)
    {
        SelectedMarker.SetActive(onOff);
    }

    private void OnPropChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(vm.IsSelected):
                SetMarkerActive(vm.IsSelected);
                break;

            case nameof(vm.PerkSCO):
                perk.Init(vm.PerkSCO);
                break;
        }
    }

    public void OnClick_Selected()
    {
        PerkSettingModel.Instance.SelectPerk(this.index);
    }
}
