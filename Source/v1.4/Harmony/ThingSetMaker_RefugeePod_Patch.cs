﻿using Verse;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using MechHumanlikes;

namespace ATReforged
{
    public class ThingSetMaker_RefugeePod_Patch
    {
        // Factionless refugees for androids or refugees for android factions should be androids. Android refugees should do a full restart on crash.
        [HarmonyPatch(typeof(ThingSetMaker_RefugeePod), "Generate")]
        public class Generate_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ThingSetMakerParams parms, ref List<Thing> outThings)
            {
                for (int i = outThings.Count - 1; i >= 0; i--)
                {
                    Thing thing = outThings[i];
                    if (thing is Pawn pawn)
                    {
                        if (!MHC_Utils.IsConsideredMechanical(pawn) && (pawn.Faction != null && pawn.Faction.def.GetModExtension<ATR_FactionAndroidExtension>()?.membersShouldBeAndroids == true || pawn.Faction == null && Faction.OfPlayer.def.GetModExtension<ATR_FactionAndroidExtension>()?.membersShouldBeAndroids == true))
                        {
                            if (pawn.Faction != null)
                            {
                                pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawn.Faction.def.basicMemberKind, pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, canGeneratePawnRelations: false, allowFood: true));
                            }
                            else
                            {
                                pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(Faction.OfPlayer.def.basicMemberKind, null, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, canGeneratePawnRelations: false, allowFood: true));
                            }
                            HealthUtility.DamageUntilDowned(pawn);
                            outThings.Replace(thing, pawn);
                        }

                        if (MHC_Utils.IsConsideredMechanical(pawn))
                        {
                            pawn.health.AddHediff(MHC_HediffDefOf.MHC_Restarting);
                        }
                    }
                }
            }
        }
    }
}