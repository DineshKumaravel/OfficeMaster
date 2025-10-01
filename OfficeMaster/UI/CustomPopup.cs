using System;
using System;
using System.Windows.Forms;
using static OfficeHelper.TimeAggregator;

namespace OfficeHelper
{
    class CustomPopup : Form
    {
        public int SelectedOption { get; private set; }

        public CustomPopup()
        {
            this.Text = "Custom Popup";
            this.Width = 250;
            this.Height = 300;

            var button1 = new Button { Text = "Work Start", Left = 20, Top = 50, Width = 80 };
            var button2 = new Button { Text = "Work End", Left = 110, Top = 50, Width = 80 };
            var button3 = new Button { Text = "Break Start", Left = 20, Top = 100, Width = 80 };
            var button4 = new Button { Text = "Break End", Left = 110, Top = 100, Width = 80 };

            button1.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.WorkStart; this.DialogResult = DialogResult.OK; };
            button2.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.WorkEnd; this.DialogResult = DialogResult.OK; };
            button3.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.BreakStart; this.DialogResult = DialogResult.OK; };
            button4.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.BreakEnd; this.DialogResult = DialogResult.OK; };


            this.Controls.Add(button1);
            this.Controls.Add(button2);
            this.Controls.Add(button3);
            this.Controls.Add(button4);

        }
    }
}
