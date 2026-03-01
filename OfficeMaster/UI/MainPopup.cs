using System.Drawing;
using System.Windows.Forms;

namespace OfficeHelper
{
    class MainPopup : Form
    {
        public int SelectedOption { get; private set; }

        public MainPopup()
        {
            this.Text = "Time Tracker v1.0";
            this.Width = 320;
            this.Height = 350;

            var button1 = new Button { Text = "Work Start", Left = 20, Top = 50, Width = 80  };
            var button2 = new Button { Text = "Break Start", Left = 120, Top = 50, Width = 80 };
            var button3 = new Button { Text = "HalfdayEnd", Left = 20, Top = 100, Width = 80, BackColor = Color.FromArgb(255, 102, 102) };
            var button4 = new Button { Text = "Monthly Status", Left = 120, Top = 100, Width = 80 };
            var button5 = new Button { Text = "Today Status", Left = 20, Top = 150, Width = 80 };
            var button6 = new Button { Text = "Delete Today", Left = 120, Top = 150, Width = 80 };


            button1.Click += (s, e) => { ChangeButtonProp(button1) ; this.DialogResult = DialogResult.OK; };
            button2.Click += (s, e) => { ChangeButtonProp(button2) ; this.DialogResult = DialogResult.OK; };
            button3.Click += (s, e) => { SelectedOption = (int)TimeAggregator.Events.HalfDayEnd; this.DialogResult = DialogResult.OK;  };
            button4.Click += (s, e) => { ShowAggregateResult(TimeAggregator.AggregateType.Month); };
            button5.Click += (s, e) => { ShowAggregateResult(TimeAggregator.AggregateType.Day); };
            button6.Click += (s, e) => { EventHandler.ClearSpecificDateDbRecord(); }; 

            this.Controls.Add(button1);
            this.Controls.Add(button2);
            this.Controls.Add(button3);
            this.Controls.Add(button4);
            this.Controls.Add(button5);
            this.Controls.Add(button6);

            EventHandler.InitializeButtonProperty(button1, button2);
        }

        private void ChangeButtonProp(Button button)
        {
            if(button.Text == "Work Start" && button.BackColor == Color.LightGreen) { button.BackColor = Color.FromArgb(255, 102, 102); ; button.Text = "Work End"; SelectedOption = (int)TimeAggregator.Events.WorkStart; }
            else if (button.Text == "Work End" && button.BackColor == Color.FromArgb(255, 102, 102)) { button.BackColor = Color.LightGreen; button.Text = "Work Start"; SelectedOption = (int)TimeAggregator.Events.WorkEnd; }            
            else if (button.Text == "Break Start" && button.BackColor == Color.LightGreen) { button.BackColor = Color.FromArgb(255, 102, 102); ; button.Text = "Break End"; SelectedOption = (int)TimeAggregator.Events.BreakStart; }
            else if (button.Text == "Break End" && button.BackColor == Color.FromArgb(255, 102, 102)) { button.BackColor = Color.LightGreen; button.Text = "Break Start"; SelectedOption = (int)TimeAggregator.Events.BreakEnd; }
       
        }
        //public static string ShowEmailForm()
        //{
        //    using (Form form = new Form { Text = "Registration", Width = 300, Height = 150 })
        //    {
        //        Label lbl = new Label { Text = "Enter Email:", Left = 10, Top = 20 };
        //        TextBox txt = new TextBox { Left = 10, Top = 40, Width = 250 };
        //        Button btn = new Button { Text = "Submit", Left = 10, Top = 70, DialogResult = DialogResult.OK };

        //        form.Controls.AddRange(new Control[] { lbl, txt, btn });


        //        if( form.ShowDialog() == DialogResult.OK && txt.Text != null)
        //        {
        //            if(txt.Text.Contains("@gmail.com"))
        //            {
        //                NotificationAPI.SendOtpEmail(txt.Text, "officemaster");
        //                return ShowOtpForm() ? txt.Text : "";
        //            }

        //        }
        //        return "";
        //    }
        //}

        //public static bool ShowOtpForm()
        //{
        //    using (Form form = new Form { Text = "OTP Verification", Width = 300, Height = 150 })
        //    {
        //        Label lbl = new Label { Text = "Enter OTP:", Left = 10, Top = 20 };
        //        TextBox txt = new TextBox { Left = 10, Top = 40, Width = 250 };
        //        Button btn = new Button { Text = "Verify", Left = 10, Top = 70, DialogResult = DialogResult.OK };

        //        form.Controls.AddRange(new Control[] { lbl, txt, btn });
        //        // In real logic, compare txt.Text with your sent OTP
        //        if(form.ShowDialog() == DialogResult.OK && txt.Text != null)
        //        {
        //            return NotificationAPI.VerifyOTP(txt.Text) ? true : false;
        //        }
        //        return false;
        //    }
        //}
        public static void DailyMonthlyStatusTableReport(TimeAggregator.AggregateType type, TimeAggregatorReport timeAggregatorReport)
        {
            string titleName = type == TimeAggregator.AggregateType.Day ? "Today Status" : "Monthly Status";
            using(Form form = new Form() { Text = titleName , Width = 320 , Height = 350})
            {
                var grid = new PropertyGrid() { Dock = DockStyle.Fill};
                if(timeAggregatorReport == null) { grid.SelectedObject = new ErrorModel("no enough record"); }
                else
                {
                    grid.SelectedObject = timeAggregatorReport;
                }
                form.Controls.Add(grid);
                if(form.ShowDialog() == DialogResult.OK) { }
            }
        }

        public static void ShowAggregateResult(TimeAggregator.AggregateType aggregateType) => DailyMonthlyStatusTableReport(aggregateType, EventHandler.FetchAggregatedData(aggregateType));
    }
}
