using HarmonyLib;
using Microsoft.VisualBasic;
using SandSailorStudio.Attributes;
using SandSailorStudio.Inventory;
using SSSGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static askaplus.bepinex.mod.Plugin;

namespace askaplus.bepinex.mod
{
    internal static class AskaRecipes
    {

        internal static void CreateRecipes()
        {
            if (!configRecipesEnable.Value) return;



            //Resin from sacks
            RecipeCreateStruct ResinFromSack = new RecipeCreateStruct();
            ResinFromSack.RecipeName = "Aska+ Resin";
            ResinFromSack.Ingredients = [new() { quantity = 1, itemInfo = Helpers.itemInfoSO["Item_Misc_CrawlerSack"] }];

            ResinFromSack.BlueprintConditionsRules = new();
            ResinFromSack.BlueprintConditionsRules.Add(Helpers.Dict_BCR["CaveEntranceL2_Rule"]);

            ResinFromSack.Quantity = 25;
            ResinFromSack.Result = Helpers.itemInfoSO["Item_Wood_Resin"];
            ResinFromSack.Category = Helpers.Dict_ICI["Categ_Blueprints_Materials"];
            ResinFromSack.Interaction = Helpers.Dict_CI["VirtualCraftingStation"];
            ResinFromSack.ItemInfoListTargets = ["WorkshopBlueprints_T2" , "WorkshopBlueprints_T1"];
            ResinFromSack.Description = "ASKA+ Found in fir trees";
            ResinFromSack.Lore = String.Empty;
            ResinFromSack.Name = "Resin";
            AddRecipe(ResinFromSack);

            //FireWood from Resin, Stick, Bark
            RecipeCreateStruct Firewood = new RecipeCreateStruct();
            Firewood.RecipeName = "Aska+ Firewood";
            Firewood.Ingredients = new List<ItemInfoQuantity>([
                new(){ quantity = 1, itemInfo = Helpers.itemInfoSO["Item_Wood_Sticks"] }, 
                new(){ quantity = 1, itemInfo = Helpers.itemInfoSO["Item_Wood_Bark"] },
                new(){ quantity = 1, itemInfo = Helpers.itemInfoSO["Item_Wood_Resin"] }
                ]);

            Firewood.BlueprintConditionsRules = new();
            Firewood.BlueprintConditionsRules.Add(Helpers.Dict_BCR["CarpenterL1_Rule"]);

            Firewood.Quantity = 1;
            Firewood.Result = Helpers.itemInfoSO["Item_Wood_Firewood"];
            Firewood.Category = Helpers.Dict_ICI["Categ_Blueprints_Materials"];
            Firewood.ItemInfoListTargets = ["WorkshopBlueprints_T2", "WorkshopBlueprints_T1"];
            Firewood.Description = "ASKA+ Found in Logs";
            Firewood.Lore = "General purpose fuel obtained by harvesting logs and removing tree stumps.";
            Firewood.Name = "Firewood";
           
            AddRecipe(Firewood);


            //Compost
            RecipeCreateStruct Compost1 = new RecipeCreateStruct();
            Compost1.RecipeName = "Aska+ Compost 1";
            Compost1.Ingredients = new List<ItemInfoQuantity>([
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Materials_LeatherScraps"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Misc_BoneFragments"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Seeds_BeetrootSeeds"] }
                ]);

            Compost1.BlueprintConditionsRules = new();
            Compost1.BlueprintConditionsRules.Add(Helpers.Dict_BCR["Farm_Rule"]);

            Compost1.Quantity = 1;
            Compost1.Result = Helpers.itemInfoSO["Item_Junk_Compost"];
            Compost1.Category = Helpers.Dict_ICI["Categ_Blueprints_Materials"];
            Compost1.ItemInfoListTargets = ["WorkshopBlueprints_T2", "WorkshopBlueprints_T1"];
            Compost1.Description = "ASKA+ It is just a junk, but pLants will love it";
            Compost1.Lore = "General purpose compost.";
            Compost1.Name = "Compost";
            AddRecipe(Compost1);

            Compost1.Ingredients = new List<ItemInfoQuantity>([
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Materials_LeatherScraps"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Misc_BoneFragments"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Seeds_BerriesSeeds"] }
                ]);
            AddRecipe(Compost1);
            Compost1.Ingredients = new List<ItemInfoQuantity>([
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Materials_LeatherScraps"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Misc_BoneFragments"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Seeds_CabbageSeeds"] }
                ]);
            AddRecipe(Compost1);
            Compost1.Ingredients = new List<ItemInfoQuantity>([
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Materials_LeatherScraps"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Misc_BoneFragments"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Seeds_CarrotSeeds"] }
                ]);
            AddRecipe(Compost1);
            Compost1.Ingredients = new List<ItemInfoQuantity>([
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Materials_LeatherScraps"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Misc_BoneFragments"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Seeds_FlaxSeeds"] }
                ]);
            AddRecipe(Compost1);
            Compost1.Ingredients = new List<ItemInfoQuantity>([
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Materials_LeatherScraps"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Misc_BoneFragments"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Seeds_GarlicSeeds"] }
                ]);
            AddRecipe(Compost1);
            Compost1.Ingredients = new List<ItemInfoQuantity>([
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Materials_LeatherScraps"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Misc_BoneFragments"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Seeds_OnionSeeds"] }
                ]);
            AddRecipe(Compost1);
            Compost1.Ingredients = new List<ItemInfoQuantity>([
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Materials_LeatherScraps"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Misc_BoneFragments"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Seeds_ReedsSeeds"] }
                ]);
            AddRecipe(Compost1);
            Compost1.Ingredients = new List<ItemInfoQuantity>([
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Materials_LeatherScraps"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Misc_BoneFragments"] },
                new(){ quantity = 5, itemInfo = Helpers.itemInfoSO["Item_Seeds_SpruceSeeds"] }
                ]);
            AddRecipe(Compost1);


            //Resin from sacks
            ResinFromSack = new RecipeCreateStruct();
            ResinFromSack.RecipeName = "Aska+ Resin";
            ResinFromSack.Ingredients = [new() { quantity = 2, itemInfo = Helpers.itemInfoSO["Item_Wood_Bark"] }];

            ResinFromSack.BlueprintConditionsRules = new();
            ResinFromSack.BlueprintConditionsRules.Add(Helpers.Dict_BCR["WoodCutterL0_Rule"]);
            ResinFromSack.BlueprintConditionsRules.Add(Helpers.Dict_BCR["WorkshopL0_Rule"]);

            ResinFromSack.Quantity = 1;
            ResinFromSack.Result = Helpers.itemInfoSO["Item_Wood_Resin"];
            ResinFromSack.Category = Helpers.Dict_ICI["Categ_Blueprints_Materials"];
            ResinFromSack.Interaction = Helpers.Dict_CI["VirtualCraftingStation"];
            ResinFromSack.ItemInfoListTargets = ["WorkshopBlueprints_T2", "WorkshopBlueprints_T1", "WorkshopBlueprints_T0"];
            ResinFromSack.Description = "ASKA+ Found in fir trees";
            ResinFromSack.Lore = String.Empty;
            ResinFromSack.Name = "Resin";
            AddRecipe(ResinFromSack);
            //TODO MODIFY Workshop to be able drop firewood to crates

        }

        internal struct RecipeCreateStruct
        {
            internal string RecipeName;
            internal List<ItemInfoQuantity> Ingredients;
            internal Il2CppSystem.Collections.Generic.List<BlueprintConditionsRule> BlueprintConditionsRules;
            internal int Quantity;
            internal ItemCategoryInfo Category;
            internal ItemInfo Result;
            internal CraftInteraction Interaction;
            internal string Description;
            internal string Lore;
            internal string Name;
            internal List<string> ItemInfoListTargets;
        }

        internal static void AddRecipe (RecipeCreateStruct data)
        {
            CraftBlueprintInfo test = ScriptableObject.CreateInstance<CraftBlueprintInfo>();
           

            test.availableInTrialVersion = false;
            Plugin.Log.LogMessage($"Adding BRC");
            test.blueprintConditionsRules = data.BlueprintConditionsRules;
            test.craftVolume = 1f;
            test.quantity = data.Quantity;
            Plugin.Log.LogMessage($"Adding ICI");

            test.category = data.Category;
            test.parts = data.Ingredients.ToArray();
            test.cost = new ItemInfoQuantity();
            test.result = data.Result;
            test.interaction = data.Interaction;
            test.icon = test.result.icon;
            test.Localized = false;
            test.localizedDescription = data.Description;
            test.localizedLore = data.Lore;
            test.localizedName = data.Name;
            test.stackSize = 1;
            test.spawnHeight = 1;
            test._cachedComponents = new Il2CppSystem.Collections.Generic.List<ItemInfo>();
            test._cachedComponentsTable = new Il2CppSystem.Collections.Generic.Dictionary<ItemInfo, Il2CppSystem.ValueTuple<int, int>>();
            test.components = Array.Empty<ItemInfoChance>();
            test.attributes = Array.Empty<AttributeData>();
            test.networkedInventoryAttributes = Array.Empty<AttributeConfig>();
            test.processes = Array.Empty<ItemProcess>();
            test.unique = true;
            test.name = data.RecipeName;
            test.previewImage = test.result.previewImage;
            test.storageClass = Helpers.Dict_ISC["VirtualItem"];
            test.spawnObject = test.result.spawnObject;
            test.id = test.GetHashCode();


            foreach (var item in data.ItemInfoListTargets)
            {
                Helpers.Dict_BlueprintsList[item].itemInfoList.Add(test);
            }

          
        }
        

        internal static void OnSettingsMenu(Transform parent)
        {
            Helpers.CreateCategory(parent, "Recipes mod");
            Helpers.CreateSwitch(parent, "*! Enable Aska+ recipes", configRecipesEnable);
        }
    }

}
