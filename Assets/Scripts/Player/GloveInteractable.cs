using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class GloveInteractable : MonoBehaviour
{
    [SerializeField] private GameObject interactableObject;

    public void ButtonInteract()
    {
        if (interactableObject != null)
        {
            ExecuteEvents.Execute(interactableObject, MenuUIManager.Instance.PointerData, ExecuteEvents.pointerClickHandler);
        }
        else Debug.LogWarning("Tried clicking at " + this.gameObject.name + " but button didnt exist");
    }
}