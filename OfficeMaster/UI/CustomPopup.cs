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
            var button5 = new Button { Text = "Monthly TotalHours", Left = 110, Top = 100, Width = 80 };
            //var button6 = new Button { Text = "Monthly OfficeHours", Left = 110, Top = 100, Width = 80 };
            //var button7 = new Button { Text = "Monthly BreakHours", Left = 110, Top = 100, Width = 80 };
            //var button8 = new Button { Text = "Monthly CompensatedHours", Left = 110, Top = 100, Width = 80 };




            button1.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.WorkStart; this.DialogResult = DialogResult.OK; };
            button2.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.WorkEnd; this.DialogResult = DialogResult.OK; };
            button3.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.BreakStart; this.DialogResult = DialogResult.OK; };
            button4.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.BreakEnd; this.DialogResult = DialogResult.OK; };
            button5.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.CalculatedHours; this.DialogResult = DialogResult.OK; };
            //button6.Click += (s, e) => { SelectedOption = (int)TimeAggregator.MontlyAggregate.OfficeHours; this.DialogResult = DialogResult.OK; };
            //button7.Click += (s, e) => { SelectedOption = (int)TimeAggregator.MontlyAggregate.BreakHours; this.DialogResult = DialogResult.OK; };
            //button8.Click += (s, e) => { SelectedOption = (int)TimeAggregator.MontlyAggregate.CompensateHours; this.DialogResult = DialogResult.OK; };




            this.Controls.Add(button1);
            this.Controls.Add(button2);
            this.Controls.Add(button3);
            this.Controls.Add(button4);
            this.Controls.Add(button5);
            //this.Controls.Add(button6);
            //this.Controls.Add(button7);
            //this.Controls.Add(button8);

        }
    }
}
