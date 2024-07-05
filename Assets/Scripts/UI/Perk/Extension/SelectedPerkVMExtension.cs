
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


        public static void OnResponseEquip(this SelectedPerkViewModel vm, PerkData perk, int index)
        {
            if (perk != vm.PerkData)
            {
                if (index == vm.index)
                    vm.PerkData = perk;
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
/*
 * 
 DataManager

Data[] m_data;

Data curData;
event Action<data> OnClick;


void OnClickBtn(data d){
 curData = d;
}

foreach data d in m_data

d.onclick += onclickbtn;


Data


event Action<Data> onclick;

void onclick(Data d)
{
   
}

Button(UI) OnClick()
+=

public void Onclick_Btn()
{
    onclick(this);
}

 */