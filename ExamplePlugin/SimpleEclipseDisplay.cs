using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine;
using SurvivorIconController = On.RoR2.UI.SurvivorIconController;

namespace SimpleEclipseDisplay
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.rune580.riskofoptions")]
    public class SimpleEclipseDisplay : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Bobot";
        public const string PluginName = "SimpleEclipseDisplay";
        public const string PluginVersion = "1.0.0";


        //public static ConfigEntry<int> minEclipseLevel { get; set; }
        public static ConfigEntry<Color> eclipseDisplayColour { get; set; }
        public static ConfigEntry<bool> eclipseDisplayOnlyOnEclipseMenus { get; set; }

        public void Awake()
        {

            eclipseDisplayColour = Config.Bind("Display", "Display Text Colour", Color.white, new ConfigDescription("Colour of the eclipse display number."));
            ModSettingsManager.AddOption(new ColorOption(eclipseDisplayColour));

            eclipseDisplayOnlyOnEclipseMenus = Config.Bind("Display", "Only Display On Eclipse Menus", true, new ConfigDescription("Whether or not the eclipse level is displayed on all character select menus or just eclipse."));
            ModSettingsManager.AddOption(new CheckBoxOption(eclipseDisplayOnlyOnEclipseMenus));

            ModSettingsManager.SetModDescription("Displays Eclipse Level On Character Icon.");

            SurvivorIconController.Rebuild += delegate (SurvivorIconController.orig_Rebuild orig, RoR2.UI.SurvivorIconController self)
            {
                orig(self);
                
                if ((eclipseDisplayOnlyOnEclipseMenus.Value && PreGameController.GameModeConVar.instance.GetString() == "EclipseRun") || !eclipseDisplayOnlyOnEclipseMenus.Value)
                {
                    var eclipseLevelDisplay = self.transform.Find("EclipseLevelDisplay");

                    if (eclipseLevelDisplay == null)
                    {
                        var eLevelText = new GameObject("EclipseLevelDisplay");

                        var text = eLevelText.AddComponent<HGTextMeshProUGUI>();
                        text.text = "-1";
                        //text.color = new Color(0.5f, 0.7f, 0.9f); truly is a shame this colour looked way nicer but was awful for readability :(
                        text.color = Color.white;
                        text.color = eclipseDisplayColour.Value;
                        text.autoSizeTextContainer = true;
                        text.outlineWidth = 0.2f;
                        text.outlineColor = Color.black;

                        eclipseLevelDisplay = Instantiate(eLevelText, self.transform).transform;
                        //eclipseLevelDisplay.localPosition = new Vector3(95, -22, 0);
                        eclipseLevelDisplay.localScale = Vector3.one;

                        var displayText = eclipseLevelDisplay.GetComponent<HGTextMeshProUGUI>();
                        displayText.alignment = TextAlignmentOptions.BottomRight;
                        displayText.fontSize = 28;

                        var displayTransform = eclipseLevelDisplay.GetComponent<RectTransform>();
                        displayTransform.anchoredPosition = Vector2.zero;
                        displayTransform.offsetMax = Vector2.zero;
                        displayTransform.offsetMin = Vector2.zero;
                        displayTransform.anchorMax = Vector2.one;
                        displayTransform.anchorMin = Vector2.zero;
                        displayTransform.sizeDelta = Vector2.one * 8;
                        displayTransform.anchoredPosition = new Vector2(-6, 4);


                        Destroy(eLevelText);
                    }

                    var eLevel = EclipseRun.GetLocalUserSurvivorCompletedEclipseLevel(self.GetLocalUser(), self.survivorDef);

                    var textInst = eclipseLevelDisplay.gameObject.GetComponent<HGTextMeshProUGUI>();
                    textInst.text = eLevel == 8 ? "DONE" : (eLevel + 1).ToString();
                }
            };


            
            
            /*minEclipseLevel = Config.Bind("Min Eclipse", "Min Eclipse Level", 5, new ConfigDescription("homosexuality", new AcceptableValueRange<int>(0, 8)));
            ModSettingsManager.AddOption(new IntSliderOption(minEclipseLevel));

            IL.RoR2.EclipseRun.OverrideRuleChoices += (il) =>
            {
                ILCursor c = new(il);

                c.GotoNext(x => x.MatchCallOrCallvirt<EclipseRun>(nameof(EclipseRun.GetNetworkUserSurvivorCompletedEclipseLevel)));

                c.Index += 4;

                c.Emit(OpCodes.Ldloc, 5);
                c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Func<int, int>>((num2) => Math.Max(minEclipseLevel.Value, num2));
                c.Emit(OpCodes.Stloc, 5);

                Debug.Log(il.ToString());
            };*/
        }
    }
}
