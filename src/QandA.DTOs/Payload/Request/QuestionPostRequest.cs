namespace QandA.DTOs.Payload.Request;

public record class QuestionPostRequest
(
    string Title,
    string Content
);