using System.Collections;

namespace DemoWebApplication.Models;

public class UserSurveyModel : IEnumerable
{
    public Guid SurveyId { get; set; }  // 問卷 ID
    public string SurveyTitle { get; set; }  // 問卷標題
    public DateTime? AnsweredAt { get; set; }  // 問卷提交時間
    public List<QuestionAnswerModel> QuestionAnswers { get; set; }  // 問卷的所有問題和答案
    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}