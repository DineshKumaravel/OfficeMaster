using NHotkey;
using NHotkey.WindowsForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static OfficeHelper.TimeAggregator;

namespace OfficeHelper
{
    public class HotKeyHandler
    {

        private static readonly DbHelper dbHelper = new DbHelper();
        private static readonly CustomPopup popup = new CustomPopup();

        private static List<string> eventList = new List<string>() { "Work Start", "Work End", "Break Start", "Break End" };
        public static void Initialize()
        {
            // Register hotkeys (all use NHotkey.ModifierKeys)
            HotkeyManager.Current.AddOrReplace(
                "CtrlAltW",
                Keys.Control | Keys.Alt | Keys.W,
                true,
                CtrlAltW_Pressed
            );
            HotkeyManager.Current.AddOrReplace(
                "CtrlShiftW",
                Keys.Control | Keys.Shift | Keys.W,
                true,
                CtrlShiftW_Pressed
            );
        }

        private static void CtrlAltW_Pressed(object sender, HotkeyEventArgs e) // work start
        { 
            if (popup.ShowDialog() == DialogResult.OK && (popup.SelectedOption >= 0 && popup.SelectedOption <= 3))
            {
                EventHandler.PushEventData(eventList[popup.SelectedOption], (Events)Enum.ToObject(typeof(Events), popup.SelectedOption));
            }
            e.Handled = true;
        }

        private static void CtrlShiftW_Pressed(object sender, HotkeyEventArgs e)
        {
            //EventHandler.ClearDB(); only for dev use
            e.Handled = true;
        }
    }

}
