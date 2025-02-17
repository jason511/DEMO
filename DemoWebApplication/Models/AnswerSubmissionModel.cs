using System;

namespace DemoWebApplication.Models;

    // 提交答案的資料模型
    public class AnswerSubmissionModel
    {
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        
        public Guid SurveyId { get; set; }
    }
