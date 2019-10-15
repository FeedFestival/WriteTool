using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public static class ElementsService
    {
        public static string GetButtonHotkey(ElementType? elementType)
        {
            if (elementType.HasValue)
            {
                switch (elementType.Value)
                {
                    case ElementType.SceneHeading:
                        return " [<b><color=red>S</color></b>]";
                    case ElementType.Action:
                        return " [<b><color=red>A</color></b>]";
                    case ElementType.Character:
                        return " [<b><color=red>C</color></b>]";
                    case ElementType.Dialog:
                        return " [<b><color=red>D</color></b>]";
                    case ElementType.Picture:
                        return " [<b><color=red>P</color></b>]";
                    default:
                        break;
                }
            }
            return string.Empty;
        }

        public static ElementType GetPreviousElementType(
            List<IPrefabComponent> elementsPool,
            List<Element> elements,
            int editableIndex,
            bool isLastElement
        )
        {
            // Debug.Log("isLastElement: " + isLastElement);
            // Debug.Log("currentIndex: " + currentIndex);
            // Debug.Log("_editableIndex: " + _editableIndex);

            ElementType previousElementType = ElementType.Action;
            if (isLastElement)
            {
                if (elements.Count > 0)
                {
                    previousElementType = elements[elements.Count - 1].ElementType;
                }
            }
            else
            {
                if (elementsPool.Count > 0)
                {
                    previousElementType = (ElementType)((elementsPool[editableIndex] as IElementComponent).TypeId);
                }
            }
            return previousElementType;
        }

        public static bool FilterNewElements(ElementType elementType, ElementType lastElementType)
        {
            switch (lastElementType)
            {
                case ElementType.SceneHeading:
                    if (elementType == ElementType.Dialog || elementType == ElementType.SceneHeading)
                    {
                        return false;
                    }
                    return true;
                case ElementType.Action:
                    if (elementType == ElementType.Dialog || elementType == ElementType.Action)
                        return false;
                    return true;
                case ElementType.Character:
                    if (elementType == ElementType.Dialog)
                        return true;
                    return false;
                case ElementType.Dialog:
                    if (elementType == ElementType.Dialog)
                        return false;
                    return true;
                default:
                    return false;
            }
        }

        public static void RecalculateIndexes(List<IPrefabComponent> elementsPool, List<Element> elements)
        {
            for (var i = 0; i < elementsPool.Count; i++)
            {
                var index = elements.FindIndex(e => { return e.UniqueId() == elementsPool[i].UniqueId; });
                elements[index].Index = i;
                elementsPool[i].UniqueId = elements[index].UniqueId();
                elementsPool[i].GameObject.name = ElementsService.GetElementName(elements[index]);
            }
        }

        public static string GetDefaultText(ElementType elementType)
        {
            if (elementType == ElementType.SceneHeading)
                return "INT.";
            return "";
        }

        public static string GetElementName(Element element)
        {
            return element.Index + "_[" + element.Id + "]_" + element.ElementType.ToString() + " (" + element.UniqueId() + ")";
        }
    }
}
