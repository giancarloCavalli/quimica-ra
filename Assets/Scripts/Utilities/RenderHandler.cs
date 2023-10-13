using TMPro;
using UnityEngine;

public static class RenderHandler
{
  public static void ChangeIncludingChildren(Transform objectTransform, bool shouldRender)
  {
    if (objectTransform.TryGetComponent<Renderer>(out var renderer))
    {
      renderer.enabled = shouldRender;
    }

    foreach (var textMeshPro in objectTransform.GetComponentsInChildren<TextMeshProUGUI>())
    {
      textMeshPro.enabled = shouldRender;
    }

    foreach (Transform child in objectTransform)
    {
      ChangeIncludingChildren(child, shouldRender);
    }
  }

  public static void ChangeSiblingsIncludingChildren(Transform objectTransform, bool shouldRender)
  {
    if (objectTransform.parent == null)
    {
      return;
    }

    foreach (Transform child in objectTransform.parent)
    {
      if (child != objectTransform)
      {
        ChangeIncludingChildren(child, shouldRender);
      }
    }
  }
}
