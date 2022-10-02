// -----------------------------------------------------------------------
// <copyright file="CustomSlotsHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using LiteNetLib.Utils;
using Mistaken.API.Diagnostics;
using UnityEngine;

namespace Mistaken.CustomSlots
{
    /// <inheritdoc/>
    public class CustomSlotsHandler : Module
    {
        /// <summary>
        /// Players with dynamic reserved slots.
        /// </summary>
        public static readonly HashSet<string> DynamicReservedSlots = new HashSet<string>();

        /// <summary>
        /// Gets real number of slots.
        /// </summary>
        public static int RealSlots => CustomNetworkManager.slots + ConnectedDynamicSlots.Count;

        /// <inheritdoc/>
        public override bool IsBasic => true;

        /// <inheritdoc/>
        public override string Name => "CustomSlots";

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.PreAuthenticating += this.Player_PreAuthenticating;
            Exiled.Events.Handlers.Player.Left += Player_Left;
            Exiled.Events.Handlers.Server.RestartingRound += Server_RestartingRound;

            Server_RestartingRound();
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.PreAuthenticating -= this.Player_PreAuthenticating;
            Exiled.Events.Handlers.Player.Left -= Player_Left;
            Exiled.Events.Handlers.Server.RestartingRound -= Server_RestartingRound;
        }

        internal CustomSlotsHandler(PluginHandler plugin)
            : base(plugin)
        {
        }

        private static readonly HashSet<string> ConnectedDynamicSlots = new HashSet<string>();
        private static int _reservedSlotsCount = 5;

        private static void Server_RestartingRound()
        {
            ConnectedDynamicSlots.Clear();

            _reservedSlotsCount = Mathf.Max(GameCore.ConfigFile.ServerConfig.GetInt("reserved_slots", ReservedSlot.Users.Count), 0);
            CustomNetworkManager.reservedSlots = 99;
        }

        private static void Player_Left(Exiled.Events.EventArgs.LeftEventArgs ev)
        {
            ConnectedDynamicSlots.Remove(ev.Player.UserId);
        }

        private void Player_PreAuthenticating(Exiled.Events.EventArgs.PreAuthenticatingEventArgs ev)
        {
            this.Log.Debug("Connecting player", PluginHandler.Instance.Config.VerboseOutput);

            if (ev.ServerFull)
            {
                this.Log.Warn("Server is full, can't grant reserved slots");

                // return;
            }

            if (ReservedSlot.Users.Contains(ev.UserId))
                this.Log.Debug("Connecting player with static reserved slot", PluginHandler.Instance.Config.VerboseOutput);
            if (DynamicReservedSlots.Contains(ev.UserId))
                this.Log.Debug("Connecting player with dynamic reserved slot", PluginHandler.Instance.Config.VerboseOutput);

            // Override
            if (((CentralAuthPreauthFlags)ev.Flags).HasFlagFast(CentralAuthPreauthFlags.ReservedSlot))
            {
                this.Log.Debug("Player has active ReservedSlot flag", PluginHandler.Instance.Config.VerboseOutput);
                return;
            }

            // Has Dynamic Reserved Slot
            if (DynamicReservedSlots.Contains(ev.UserId))
                ConnectedDynamicSlots.Add(ev.UserId);

            // Server Not full
            else if (Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.ConnectedPeersCount < RealSlots)
                return;

            // Has Reserved Slot
            else if (ReservedSlot.Users.Contains(ev.UserId) || DynamicReservedSlots.Contains(ev.UserId))
            {
                if (Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.ConnectedPeersCount < RealSlots + _reservedSlotsCount)
                    return;

                var reason = $"Server is full!   {Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.ConnectedPeersCount}/{RealSlots + _reservedSlotsCount} (With reserved slots)";
                var writer = new NetDataWriter();
                writer.Put((byte)10);
                writer.Put(reason);
                ev.Request.Reject(writer);
                ev.Disallow();
                Exiled.API.Features.Log.Info($"Rejecting {ev.UserId} with reason: {reason}");
            }

            // No reserved Slot
            else
            {
                var reason = $"Server is full!   {Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorCore.Host.ConnectedPeersCount}/{RealSlots}";
                var writer = new NetDataWriter();
                writer.Put((byte)10);
                writer.Put(reason);
                ev.Request.Reject(writer);
                ev.Disallow();
                Exiled.API.Features.Log.Info($"Rejecting {ev.UserId} with reason: {reason}");
            }
        }
    }
}
