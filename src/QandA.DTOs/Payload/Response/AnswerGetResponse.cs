namespace QandA.DTOs.Payload.Response;

public class AnswerGetResponse
{
    public int AnswerId { get; set; }
    public string Content { get; set; }
    public string UserName { get; set; }
    public string UserId { get; set; }
    public DateTime Created { get; set; }

    public AnswerGetResponse() { }
};
