
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
        }


        public static void OnResponseEquip(this SelectedPerkViewModel vm, PerksScriptableObject perk)
        {
            if (perk != null)
            {
                if (perk != vm.PerkSCO)
                {
                    vm.PerkSCO = perk;
                }
            }
        }

        public static void OnResponseSelect(this SelectedPerkViewModel vm, int index)
        {
            vm.IsSelected = true;
        }
    }
}
