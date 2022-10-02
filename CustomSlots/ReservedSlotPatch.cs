// -----------------------------------------------------------------------
// <copyright file="ReservedSlotPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Mistaken.CustomSlots
{
    [HarmonyPatch(typeof(ReservedSlot), nameof(ReservedSlot.HasReservedSlot), new Type[] { typeof(string) })]
    internal static class ReservedSlotPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Ret);
        }
    }
}
