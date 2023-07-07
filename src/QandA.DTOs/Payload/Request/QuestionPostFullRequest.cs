namespace QandA.DTOs.Payload.Request;

public class QuestionPostFullRequest
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string UserName { get; set; }
    public string UserId { get; set; }
    public DateTime Created { get; set; }

    public QuestionPostFullRequest() { }
};