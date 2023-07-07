namespace QandA.DTOs.Payload.Request;

public record class AnswerPostRequest
(
    int QuestionId,
    string Content
);