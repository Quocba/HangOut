namespace HangOut.Domain.Payload.Base;

public class EmailMessage
{
    public string ToAddress { get; set; } 
    public string Body { get; set; }
    public string Subject { get; set; }
}