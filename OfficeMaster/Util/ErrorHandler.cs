using OfficeHelper;
using System;
using System.Windows.Forms;

public class ErrorHandler
{
    public static void RecordError(string ex) // send to me using api
    {
        //NotificationAPI.SendErrorMail(ex);
        var errorDescription = Util.Encrypt( ex , Environment.GetEnvironmentVariable("ENCRYPTKEY"));
        ErrorModel model = new ErrorModel(errorDescription);
        model.errorRecordTime = DateTime.Now;
        using (Form form = new Form() { Text = "Error", Width = 320, Height = 350 })
        {
            var grid = new PropertyGrid() { Dock = DockStyle.Fill };
            grid.Dock = DockStyle.Fill;
            grid.SelectedObject = model;
            form.Controls.Add(grid);
            if (form.ShowDialog() == DialogResult.OK) { }
        }
        Application.Exit();
    }

    //public static void ShowError()
    //{
    //}
}
