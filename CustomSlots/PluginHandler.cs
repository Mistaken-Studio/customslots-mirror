// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace Mistaken.CustomSlots
{
    /// <inheritdoc/>
    internal class PluginHandler : Plugin<Config>
    {
        /// <inheritdoc/>
        public override string Author => "Mistaken Devs";

        /// <inheritdoc/>
        public override string Name => "CustomSlots";

        /// <inheritdoc/>
        public override string Prefix => "MCustomSlots";

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Default;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(5, 2, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            this.Harmony = new HarmonyLib.Harmony("com.mistaken.api");
            this.Harmony.PatchAll();

            new CustomSlotsHandler(this);

            API.Diagnostics.Module.OnEnable(this);

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            this.Harmony.UnpatchAll();
            API.Diagnostics.Module.OnDisable(this);

            base.OnDisabled();
        }

        internal        HarmonyLib.Harmony Harmony  { get; private set; }
        internal static PluginHandler      Instance { get; private set; }
    }
}
