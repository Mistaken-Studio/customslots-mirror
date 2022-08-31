﻿// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Mistaken.Updater.Config;

namespace Mistaken.CustomSlots
{
    /// <inheritdoc/>
    internal class Config : IAutoUpdatableConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug should be displayed.
        /// </summary>
        [Description("If true then debug will be displayed")]
        public bool VerboseOutput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether custom slots module is enabled.
        /// </summary>
        [Description("If true then custom slots module will be enabled")]
        public bool CustomSlotsEnabled { get; set; } = false;

        /// <inheritdoc/>
        [Description("Auto Update Settings")]
        public System.Collections.Generic.Dictionary<string, string> AutoUpdateConfig { get; set; }
    }
}
