using TMPro;
using UnityEngine;
using System.Linq;

public static class RenderHelper
{
  public static void ChangeSelfIncludingChildren(Transform objectTransform, bool shouldRender)
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
      ChangeSelfIncludingChildren(child, shouldRender);
    }
  }

  public static void ChangeSelfIncludingChildrenIgnoringTags(Transform objectTransform, bool shouldRender, string[] tagsToIgnore)
  {
    if (tagsToIgnore.Contains(objectTransform.tag))
    {
      return;
    }

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
      ChangeSelfIncludingChildrenIgnoringTags(child, shouldRender, tagsToIgnore);
    }
  }

  public static void ChangeSiblingsIncludingChildren(Transform objectTransform, bool shouldRender, string tagToBeIgnored = "")
  {
    if (objectTransform.parent == null)
    {
      return;
    }

    foreach (Transform child in objectTransform.parent)
    {
      if (child != objectTransform)
      {
        if (tagToBeIgnored != "" && child.CompareTag(tagToBeIgnored))
        {
          continue;
        }
        ChangeSelfIncludingChildren(child, shouldRender);
      }
    }
  }

  public static void ChangeSelfAndSiblingsIncludingChildren(Transform objectTransform, bool shouldRender)
  {
    ChangeSelfIncludingChildren(objectTransform, shouldRender);
    ChangeSiblingsIncludingChildren(objectTransform, shouldRender);
  }

  public static void ChangeChildrenIncludingChildren(Transform objectTransform, bool shouldRender)
  {
    foreach (Transform child in objectTransform)
    {
      ChangeSelfIncludingChildren(child, shouldRender);
    }
  }

  public static bool GetRendererEnabledValue(Transform objectTransform)
  {
    if (objectTransform.TryGetComponent<Renderer>(out var renderer))
    {
      return renderer.enabled;
    }

    return false;
  }

  public static void ChangeChildrenIgnoringTags(Transform objectTransform, bool shouldRender, string[] tagsToIgnore)
  {
    foreach (Transform child in objectTransform)
    {
      ChangeSelfIncludingChildrenIgnoringTags(child, shouldRender, tagsToIgnore);
    }
  }
}
