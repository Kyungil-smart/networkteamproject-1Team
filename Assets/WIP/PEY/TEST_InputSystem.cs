using UnityEngine;

public class TEST_InputSystem : MonoBehaviour
{
    public BattleInputReader input;
#if UNITY_EDITOR
    private void Reset()
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BattleInputReader");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            input = UnityEditor.AssetDatabase.LoadAssetAtPath<BattleInputReader>(path);
        }
        else
        {
            Debug.LogWarning("BattleInputReader SO를 찾을 수 없습니다");
        }
    }
#endif
    private void OnEnable()
    {
        input.Enable();
        input.onInteract += HandleInteractStart;
        input.Interact += HandleInteractPerformed;
        input.offInteract += HandleInteractCancel;
        input.onAttack += HandleAttack;
    }
    private void OnDisable()
    {
        input.onInteract -= HandleInteractStart;
        input.Interact -= HandleInteractPerformed;
        input.offInteract -= HandleInteractCancel;
        input.onAttack -= HandleAttack;
    }

    void HandleInteractStart()
    {
        Debug.Log("Interact Started!");
    }
    void HandleInteractPerformed()
    {
        Debug.Log("Interact Performed!");
    }
    void HandleInteractCancel()
    {
        Debug.Log("Interact Canceled!");
    }

    void HandleAttack()
    {
        Debug.Log("Attack!");
    }
}
