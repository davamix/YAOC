using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Yaoc.Behaviours
{
    public class ScrollBarAutoBehavior:Behavior<ScrollViewer>
    {

        protected override void OnAttached() {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching() {
            base.OnDetaching();

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            var scrollViewer = sender as ScrollViewer;

            if (scrollViewer != null) {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
        }

        // https://stackoverflow.com/a/25765336
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            ScrollViewer sv = sender as ScrollViewer;
            bool AutoScrollToEnd = true;
            if (sv.Tag != null) {
                AutoScrollToEnd = (bool)sv.Tag;
            }
            if (e.ExtentHeightChange == 0)// user scroll
            {
                AutoScrollToEnd = sv.ScrollableHeight == sv.VerticalOffset;
            } else// content change
              {
                if (AutoScrollToEnd) {
                    sv.ScrollToEnd();
                }
            }
            sv.Tag = AutoScrollToEnd;
            return;
        }
    }
}
