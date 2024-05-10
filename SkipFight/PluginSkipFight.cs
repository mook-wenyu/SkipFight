using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using JSONClass;
using UnityEngine;
using UnityEngine.UI;


namespace SkipFight
{
    [BepInPlugin("com.remook.SkipFight", "跳过战斗", "1.0.1")]
    public class PluginSkipFight : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin com.remook.SkipFight is loaded!");
        }

        private void Start()
        {
            Harmony.CreateAndPatchAll(typeof(PluginSkipFight), null);
        }

        // 战斗面板管理类 初始化函数
        [HarmonyPatch(typeof(FpUIMag), nameof(FpUIMag.Init))]
        [HarmonyPostfix]
        public static void Postfix_FpUIMag_Init(FpUIMag __instance, ref Text ___npcName)
        {
            // 
            ___npcName.text = string.Format("{0} - {1}", jsonData.instance.AvatarRandomJsonData[__instance.npcId.ToString()]["Name"].Str, Tools.instance.Code64ToString(jsonData.instance.LevelUpDataJsonData[jsonData.instance.AvatarJsonData[__instance.npcId.ToString()]["Level"].I.ToString()]["Name"].str));
            // 
            string tips = GetTips();
            if (tips == "无")
            {
                __instance.DisableSkipFight.gameObject.SetActive(false);
                __instance.SkipFight.gameObject.SetActive(true);
                __instance.SkipFight.mouseUpEvent.RemoveAllListeners();

                __instance.SkipFight.mouseUpEvent.AddListener(delegate
                {
                    ResManager.inst.LoadPrefab("VictoryPanel").Inst();
                    UINPCJiaoHu.AllShouldHide = false;
                    UnityEngine.Object.Destroy(__instance.gameObject);
                });
            }
            else
            {
                __instance.DisableSkipFight.gameObject.SetActive(true);
                __instance.SkipFight.gameObject.SetActive(false);
                __instance.TipsText.text = tips;
            }

        }

        // 获取跳过战斗类型的提示
        private static string GetTips()
        {
            if (Tools.instance.monstarMag.FightType != Fungus.StartFight.FightEnumType.Normal)
            {
                return "该战斗类型无法跳过";
            }
            return "无";
        }

    }

}
