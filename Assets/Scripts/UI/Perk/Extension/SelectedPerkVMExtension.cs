
namespace PerkVMExtension
{
    public static class SelectedPerkVMExtension
    {
        public static void RegisterOnEnable(this SelectedPerkViewModel vm)
        {
            PerkSettingModel.Instance.RegisterEventOnEquip(vm.OnResponseEquip);
            PerkSettingModel.Instance.RegisterEventOnSelect(vm.OnResponseSelect);
        }


        public static void UnRegisterOnDisable(this SelectedPerkViewModel vm)
        {
            PerkSettingModel.Instance.UnRegisterEventOnSelect(vm.OnResponseSelect);
            PerkSettingModel.Instance.UnRegisterEventOnEquip(vm.OnResponseEquip);
            PerkSettingModel.Instance.selectedPerkList.Clear();
        }


        public static void OnResponseEquip(this SelectedPerkViewModel vm, PerksScriptableObject perk, int index)
        {
            if (perk != vm.PerkSCO)
            {
                if (index == vm.index)
                    vm.PerkSCO = perk;
            }
        }

        public static void OnResponseSelect(this SelectedPerkViewModel vm, int index)
        {
            if (vm.index == index)
                vm.IsSelected = true;
            else
                vm.IsSelected = false;
        }
    }
}
