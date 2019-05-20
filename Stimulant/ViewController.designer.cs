// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Stimulant
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Xamarin.RangeSlider.RangeSliderControl ccRangeSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl modeSegmentedControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl pnumSegmentedControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel programLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel rateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton startButton { get; set; }

        [Action ("ModeNumChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ModeNumChanged (UIKit.UISegmentedControl sender);

        [Action ("pnumChange:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void pnumChange (UIKit.UISegmentedControl sender);

        [Action ("RateSliderChange:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RateSliderChange (UIKit.UISlider sender);

        [Action ("StartButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void StartButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ccRangeSlider != null) {
                ccRangeSlider.Dispose ();
                ccRangeSlider = null;
            }

            if (modeSegmentedControl != null) {
                modeSegmentedControl.Dispose ();
                modeSegmentedControl = null;
            }

            if (pnumSegmentedControl != null) {
                pnumSegmentedControl.Dispose ();
                pnumSegmentedControl = null;
            }

            if (programLabel != null) {
                programLabel.Dispose ();
                programLabel = null;
            }

            if (rateLabel != null) {
                rateLabel.Dispose ();
                rateLabel = null;
            }

            if (startButton != null) {
                startButton.Dispose ();
                startButton = null;
            }
        }
    }
}