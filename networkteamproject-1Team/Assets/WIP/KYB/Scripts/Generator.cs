using System;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode;

public class Generator : NetworkBehaviour, IInteractable
{
    private PressAction _pressAction;
    private MeshRenderer _renderer;
    public Material completedMaterials;


    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _pressAction = GetComponent<PressAction>();
        
        _pressAction.OnPressCompleted += ChangeToCompleted;
    }

    private void ChangeToCompleted()
    {
        _renderer.material = completedMaterials;
        
        _pressAction.image.canvas.gameObject.SetActive(false);
    }

    public void InteractStart()
    {
        _pressAction.StartInteraction();
    }

    public void InteractStop()
    {
        _pressAction.StopInteraction();
    }
    
    // public void StartInteractServerRpc()
    // public void StopInteractServerRpc()
}
