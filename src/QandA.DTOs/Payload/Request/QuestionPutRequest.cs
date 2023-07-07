namespace QandA.DTOs.Payload.Request;

public record class QuestionPutRequest
(
    string Title,
    string Content
);