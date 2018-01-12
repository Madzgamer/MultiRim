using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HugsLib;
using Harmony;
using UnityEngine;
using HugsLib.Utils;
using HugsLib.Settings;
using HugsLib.Source.Settings;

namespace MultiRim
{
    public class MainBridgeClass : ModBase
    {
        private string modId;

        public override string ModIdentifier
        {
            get { return "MultiRim"; }
        }

        public MainBridgeClass Instance { get; private set; }

        public MainBridgeClass()
        {
            Instance = this;
        }

        public override void Initialize()
        {
            Logger.Message("Initialized");
        }

        public override void OnGUI()
        {
            base.OnGUI();


        }

        public override void MapLoaded(Map map)
        {
            base.MapLoaded(map);

            Logger.Message("Player home: {0}", map.IsPlayerHome);
            //Thing thing = ThingMaker.MakeThing(new ThingDef());
            //thing.stackCount = 20;
           // thing.SetFaction(Faction.OfPlayer);
            //List<Thing> things = new List<Thing>();
            //things.Add(thing);
            

            //DropPodUtility.DropThingsNear(map.Center, map, things);

        }
        /*
        public override void Tick(int currentTick)
        {
            Logger.Message("Tick:"+currentTick);
        }
        */
        // original method: public static bool ButtonText(Rect rect, string label, bool drawBackground = true, bool doMouseoverSound = false, bool active = true)
        [HarmonyPatch(typeof(Dialog_FileList), "DoWindowContents")]
        public static class FileList_DoWindowContents_Patch
        {
            [HarmonyPostfix]
            public static void DrawHelloButton(Dialog_FileList __instance, Rect inRect)
            {
                ModLogger log = new ModLogger("MultiRim");
                if (!(__instance is Dialog_SaveFileList_Load)) return;
                var buttonSize = new Vector2(220f, 140f);
                Rect rect = new Rect(0, inRect.height - buttonSize.y, buttonSize.x, buttonSize.y);
                TooltipHandler.TipRegion(rect, "Say hello to developer");
                if (Widgets.ButtonText(rect, "Hello"))
                {
                    // do stuff		
                    log.Message("You pressed the button");
                }
            }
        }

        [HarmonyPatch(typeof(Selector), "SelectUnderMouse")]
        public static class OnSomethingSelected
        {
            [HarmonyPostfix]
            public static void AnnounceSelection(Selector __instance)
            {
                ModLogger log = new ModLogger("MultiRim");
                log.Message("Something was selected");
                List<Thing> things = new List<Thing>();
                List<Zone_Stockpile> stockpiles = new List<Zone_Stockpile>();
                string output = "";
                foreach (object obj in __instance.SelectedObjects)
                {
                    if(obj is Thing)
                    {
                        things.Add((Thing)obj);
                    }
                    if(obj is Zone_Stockpile)
                    {
                        stockpiles.Add((Zone_Stockpile)obj);
                    }
                }
                /*
                if(things.Count != 0)
                {
                    foreach(Thing thing in things)
                    {
                        thing.TryAttachFire(3f);
                        output = output + thing.Label + ", ";
                    }

                    log.Message("Tried to light {0} things on fire, those things were: {1}", things.Count, output);
                }
                */
                if(stockpiles.Count != 0)
                {
                    Dictionary<string, int> contents = new Dictionary<string, int>();


                    foreach(Zone_Stockpile stockpile in stockpiles)
                    {
                        foreach(Thing thing in stockpile.AllContainedThings)
                        {

                            if (!thing.def.IsCorpse)
                            {
                                if (contents.ContainsKey(thing.LabelNoCount))
                                {
                                    contents[thing.LabelNoCount] = contents.GetValueSafe(thing.LabelNoCount) + thing.stackCount;
                                    //contents.Add(thing.LabelNoCount, contents.GetValueSafe(thing.LabelNoCount) + thing.stackCount);
                                }
                                else
                                {

                                    contents.Add(thing.LabelNoCount, thing.stackCount);
                                }
                            }
                        }

                        //output = output + stockpile.label + ", ";
                    }
                    log.Message("You selected a stockpile!!!");
                    if(contents.Count != 0)
                    {
                        foreach (KeyValuePair<string, int> kvp in contents)
                        {
                            output = output + kvp.Key + " (" + kvp.Value + ")\n";
                        }
                        log.Message("It contains: " + output);
                        Dialog_MessageBox msgBox = new Dialog_MessageBox("It contains: " + output, "OK");
                        Find.WindowStack.Add(msgBox);
                    }
                }

            }
        }


    }

}
