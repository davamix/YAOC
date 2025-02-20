using System.Windows.Media;
using System.Windows;

namespace Yaoc.Extensions.UI;

public static class FindControls {
    public static T FindAncestor<T>(this DependencyObject dObj) where T : DependencyObject {
        var uiElement = dObj;
        while (uiElement != null) {
            uiElement = VisualTreeHelper.GetParent(uiElement as Visual ?? new UIElement())
                ?? LogicalTreeHelper.GetParent(uiElement);

            if (uiElement is T) return (T)uiElement;
        }
        return null;
    }
}
