using Fusion;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles prop disguise mechanics - props can disguise as everyday Nairobi objects.
/// Adapted from Python prototype with multiplayer support via Photon Fusion.
/// </summary>
public class PropDisguiseSystem : NetworkBehaviour
{
    [SerializeField] private List<string> availableDisguises = new List<string>
    {
        "Chair",
        "Wheelchair", 
        "Cabinet",
        "Bin",
        "Market Stall",
        "Helmet",
        "Drum",
        "Crate"
    };

    [Networked] public string CurrentDisguise { get; set; }
    [Networked] public bool IsDisguised { get; set; }

    private Material originalMaterial;
    private Material disguiseMaterial;

    private void OnEnable()
    {
        if (HasInputAuthority)
        {
            ChangeDisguise();
        }
    }

    /// <summary>
    /// Changes the current disguise to a random one from the available list.
    /// </summary>
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ChangeDisguise()
    {
        CurrentDisguise = availableDisguises[Random.Range(0, availableDisguises.Count)];
    }

    /// <summary>
    /// Public method for input handling.
    /// </summary>
    public void ChangeDisguise()
    {
        if (HasInputAuthority)
        {
            RPC_ChangeDisguise();
        }
    }

    /// <summary>
    /// Toggles disguise state - affects detection mechanics.
    /// </summary>
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ToggleDisguise()
    {
        IsDisguised = !IsDisguised;
    }

    public void ToggleDisguise()
    {
        if (HasInputAuthority)
        {
            RPC_ToggleDisguise();
        }
    }

    /// <summary>
    /// Get the visual representation of the current disguise.
    /// </summary>
    public string GetDisguiseInfo()
    {
        return $"Disguise: {CurrentDisguise} | Hidden: {IsDisguised}";
    }
}
