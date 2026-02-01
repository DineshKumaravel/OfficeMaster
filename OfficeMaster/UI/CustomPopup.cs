using System;
using System.Windows.Forms;

namespace OfficeHelper
{
    class CustomPopup : Form
    {
        public int SelectedOption { get; private set; }

        private static RichTextBox resultBox;

        public CustomPopup()
        {
            this.Text = "Time Tracker v1.0";
            this.Width = 320;
            this.Height = 350;

            resultBox = new RichTextBox
            {
                Left = 20,
                Top = 200,
                Width = 250,
                Height = 100,
                ReadOnly = true,
                BackColor = System.Drawing.Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };

            var button1 = new Button { Text = "Work Start", Left = 20, Top = 50, Width = 80 };
            var button2 = new Button { Text = "Work End", Left = 110, Top = 50, Width = 80 };
            var button3 = new Button { Text = "Break Start", Left = 20, Top = 100, Width = 80 };
            var button4 = new Button { Text = "Break End", Left = 110, Top = 100, Width = 80 };
            var button5 = new Button { Text = "Monthly TotalHours", Left = 20, Top = 150, Width = 80 };
            var button6 = new Button { Text = "Today Status", Left = 110, Top = 150, Width = 80 };
            var button7 = new Button { Text = "Clear", Left = 200, Top = 150, Width = 80 };
            var button8 = new Button { Text = "Half Day End", Left = 200, Top = 50, Width = 80 };


            button1.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.WorkStart; this.DialogResult = DialogResult.OK; };
            button2.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.WorkEnd; this.DialogResult = DialogResult.OK; };
            button3.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.BreakStart; this.DialogResult = DialogResult.OK; };
            button4.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.BreakEnd; this.DialogResult = DialogResult.OK; };
            button5.Click += (s, e) => { ShowAggregateResult(TimeAggregator.AggregateType.Month); };
            button6.Click += (s, e) => { ShowAggregateResult(TimeAggregator.AggregateType.Day); };
            button7.Click += (s, e) => { ClearTextBox(); };
            button8.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.HalfDayEnd; this.DialogResult = DialogResult.OK; };

            this.Controls.Add(button1);
            this.Controls.Add(button2);
            this.Controls.Add(button3);
            this.Controls.Add(button4);
            this.Controls.Add(button5);
            this.Controls.Add(button6);
            this.Controls.Add(button7);
            this.Controls.Add(button8);
            this.Controls.Add(resultBox);

        }

        public static void ShowAggregateResult(TimeAggregator.AggregateType aggregateType)
        {
            resultBox.Text = EventHandler.FetchAggregatedData(aggregateType);
        }

        public static void ShowError(string message) => resultBox.Text = message;

        public static void ClearTextBox() => resultBox.Clear();
    }
}
