using HarmonyLib;
using Kitchen;
using System;
using System.Collections.Generic;

namespace KitchenConsolidatedPreferences
{
    #region Register KL Submenus in PS
    [HarmonyPatch(typeof(PreferenceSystem.Menus.PreferenceSystemMenu<MainMenuAction>), "CreateSubmenus")]
    class PreferenceSystem_PreferenceSystemMenu_CreateSubmenus_Patch1
    {
        [HarmonyPostfix]
        static void Postfix(ref Dictionary<Type, Menu<MainMenuAction>> menus, ref PreferenceSystem.Menus.PreferenceSystemMenu<MainMenuAction> __instance)
        {
            Mod.LogInfo("PreferenceSystem_PreferenceSystemMenu_CreateSubmenus_Patch1");

            if (__instance.GetType().GetGenericArguments()[0] == typeof(MainMenuAction))
                KitchenLib.Utils.EventUtils.InvokeEvent(nameof(KitchenLib.Event.Events.PreferenceMenu_MainMenu_CreateSubmenusEvent), KitchenLib.Event.Events.PreferenceMenu_MainMenu_CreateSubmenusEvent?.GetInvocationList(), null, new KitchenLib.Event.PreferenceMenu_CreateSubmenusArgs<MainMenuAction>(__instance, menus, __instance.Container, __instance.ModuleList));
        }
    }

    [HarmonyPatch(typeof(PreferenceSystem.Menus.PreferenceSystemMenu<PauseMenuAction>), "CreateSubmenus")]
    class PreferenceSystem_PreferenceSystemMenu_CreateSubmenus_Patch2
    {
        [HarmonyPostfix]
        static void Postfix(ref Dictionary<Type, Menu<PauseMenuAction>> menus, ref PreferenceSystem.Menus.PreferenceSystemMenu<PauseMenuAction> __instance)
        {
            Mod.LogInfo("PreferenceSystem_PreferenceSystemMenu_CreateSubmenus_Patch2");

            if (__instance.GetType().GetGenericArguments()[0] == typeof(PauseMenuAction))
                KitchenLib.Utils.EventUtils.InvokeEvent(nameof(KitchenLib.Event.Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent), KitchenLib.Event.Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent?.GetInvocationList(), null, new KitchenLib.Event.PreferenceMenu_CreateSubmenusArgs<PauseMenuAction>(__instance, menus, __instance.Container, __instance.ModuleList));
        }
    }

    [HarmonyPatch(typeof(KitchenLib.ModsPreferencesMenu<MainMenuAction>), "CreateSubmenus")]
    class KitchenLib_ModsPreferencesMenu_CreateSubmenus_Patch1
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            Mod.LogInfo("KitchenLib_ModsPreferencesMenu_CreateSubmenus_Patch1");

            return false;
        }
    }

    [HarmonyPatch(typeof(KitchenLib.ModsPreferencesMenu<PauseMenuAction>), "CreateSubmenus")]
    class KitchenLib_ModsPreferencesMenu_CreateSubmenus_Patch2
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            Mod.LogInfo("KitchenLib_ModsPreferencesMenu_CreateSubmenus_Patch2");

            return false;
        }
    }
    #endregion

    #region Add Buttons for KL Submenus in PS
    [HarmonyPatch(typeof(KitchenLib.ModsPreferencesMenu<MainMenuAction>), "RegisterMenu")]
    class KitchenLib_ModsPreferencesMenu_RegisterMenu_Patch1
    {
        [HarmonyPrefix]
        static bool Prefix(string name, Type type, Type generic)
        {
            Mod.LogInfo($"KitchenLib_ModsPreferencesMenu_RegisterMenu_Patch1 {name}");

            PreferenceSystem.Menus.PreferenceSystemMenu<MainMenuAction>.RegisterMenu(name, type, generic);

            return false;
        }
    }

    [HarmonyPatch(typeof(KitchenLib.ModsPreferencesMenu<PauseMenuAction>), "RegisterMenu")]
    class KitchenLib_ModsPreferencesMenu_RegisterMenu_Patch2
    {
        [HarmonyPrefix]
        static bool Prefix(string name, Type type, Type generic)
        {
            Mod.LogInfo($"KitchenLib_ModsPreferencesMenu_RegisterMenu_Patch2 {name}");

            PreferenceSystem.Menus.PreferenceSystemMenu<PauseMenuAction>.RegisterMenu(name, type, generic);

            return false;
        }
    }
    #endregion

    #region Replace KL Button with PS Button
    [HarmonyPatch(typeof(KitchenLib.Main), "OnPostActivate")]
    class KitchenLib_Main_SetupMenus_Patch
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            // TODO: make this actually work 

            Mod.LogInfo("KitchenLib_Main_SetupMenus_Patch");

            KitchenLib.ModsPreferencesMenu<PauseMenuAction>.RegisterMenu("KitchenLib 2", typeof(KitchenLib.UI.PreferenceMenu<PauseMenuAction>), typeof(PauseMenuAction));
            KitchenLib.ModsPreferencesMenu<MainMenuAction>.RegisterMenu("KitchenLib 2", typeof(KitchenLib.UI.PreferenceMenu<MainMenuAction>), typeof(MainMenuAction));

            //Setting Up For Main Menu
            KitchenLib.Event.Events.StartMainMenu_SetupEvent += (s, args) =>
            {
                args.addSubmenuButton.Invoke(args.instance, new object[] { "Mods 2", typeof(KitchenLib.ModsMenu<MainMenuAction>), false });
                args.addSubmenuButton.Invoke(args.instance, new object[] { "Mod Preferences 2", typeof(KitchenLib.ModsPreferencesMenu<MainMenuAction>), false });
            };
            KitchenLib.Event.Events.MainMenuView_SetupMenusEvent += (s, args) =>
            {
                args.addMenu.Invoke(args.instance, new object[] { typeof(KitchenLib.UI.RevisedMainMenu), new KitchenLib.UI.RevisedMainMenu(args.instance.ButtonContainer, args.module_list) });
                args.addMenu.Invoke(args.instance, new object[] { typeof(KitchenLib.ModsMenu<MainMenuAction>), new KitchenLib.ModsMenu<MainMenuAction>(args.instance.ButtonContainer, args.module_list) });
                args.addMenu.Invoke(args.instance, new object[] { typeof(KitchenLib.ModsPreferencesMenu<MainMenuAction>), new KitchenLib.ModsPreferencesMenu<MainMenuAction>(args.instance.ButtonContainer, args.module_list) });
                args.addMenu.Invoke(args.instance, new object[] { typeof(KitchenLib.UI.PlateUp.DeveloperOptions<MainMenuAction>), new KitchenLib.UI.PlateUp.DeveloperOptions<MainMenuAction>(args.instance.ButtonContainer, args.module_list) });
                args.addMenu.Invoke(args.instance, new object[] { typeof(KitchenLib.UI.PlateUp.UserOptions<MainMenuAction>), new KitchenLib.UI.PlateUp.UserOptions<MainMenuAction>(args.instance.ButtonContainer, args.module_list) });
            };

            //Setting Up For Pause Menu
            KitchenLib.Event.Events.MainMenu_SetupEvent += (s, args) =>
            {
                args.addSubmenuButton.Invoke(args.instance, new object[] { "Mods 2", typeof(KitchenLib.ModsMenu<PauseMenuAction>), false });
                args.addSubmenuButton.Invoke(args.instance, new object[] { "Mod Preferences 2", typeof(KitchenLib.ModsPreferencesMenu<PauseMenuAction>), false });
            };
            KitchenLib.Event.Events.PlayerPauseView_SetupMenusEvent += (s, args) =>
            {
                args.addMenu.Invoke(args.instance, new object[] { typeof(KitchenLib.ModsMenu<PauseMenuAction>), new KitchenLib.ModsMenu<PauseMenuAction>(args.instance.ButtonContainer, args.module_list) });
                //args.addMenu.Invoke(args.instance, new object[] { typeof(KitchenLib.ModsPreferencesMenu<PauseMenuAction>), new KitchenLib.ModsPreferencesMenu<PauseMenuAction>(args.instance.ButtonContainer, args.module_list) });
                args.addMenu.Invoke(args.instance, new object[] { typeof(KitchenLib.UI.PlateUp.ModSyncMenu), new KitchenLib.UI.PlateUp.ModSyncMenu(args.instance.ButtonContainer, args.module_list) });
                args.addMenu.Invoke(args.instance, new object[] { typeof(KitchenLib.UI.PlateUp.DeveloperOptions<PauseMenuAction>), new KitchenLib.UI.PlateUp.DeveloperOptions<PauseMenuAction>(args.instance.ButtonContainer, args.module_list) });
                args.addMenu.Invoke(args.instance, new object[] { typeof(KitchenLib.UI.PlateUp.UserOptions<PauseMenuAction>), new KitchenLib.UI.PlateUp.UserOptions<PauseMenuAction>(args.instance.ButtonContainer, args.module_list) });
            };

            KitchenLib.Event.Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(KitchenLib.UI.PreferenceMenu<PauseMenuAction>), new KitchenLib.UI.PreferenceMenu<PauseMenuAction>(args.Container, args.Module_list));
            };

            KitchenLib.Event.Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(KitchenLib.UI.PreferenceMenu<MainMenuAction>), new KitchenLib.UI.PreferenceMenu<MainMenuAction>(args.Container, args.Module_list));
            };

            return false;
        }
    }
    #endregion

    #region Remove PS Button
    #endregion
}
