using System;

public class ErrorModel
{
    public DateTime errorRecordTime {  get; set; }
    public string errorMessage { get; set; }

    public ErrorModel(string msg)
    {
        errorMessage = msg;
    }

    //public static ErrorModel Transform(string msg)
    //{
    //    return new ErrorModel() { errorMessage = msg};

    //}
}
