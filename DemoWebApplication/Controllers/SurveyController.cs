using Microsoft.AspNetCore.Mvc;
using DemoWebApplication.Models;
using System.Linq;
using System.Security.Claims;

namespace DemoWebApplication.Controllers
{
    public class SurveyController : Controller
    {
        private readonly test1Context _context;

        public SurveyController(test1Context context)
        {
            _context = context;
        }

        // 顯示所有用戶回答過的問卷
        public IActionResult Index()
        {
            // 取得當前登入用戶的 ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim.Value);

            // 查詢用戶所回答的所有問卷，並按回答時間排序，顯示問卷編號
            var surveys = _context.answers
                .Where(a => a.user_id == userId)
                .GroupBy(a => a.survey_id)
                .Select(g => new UserSurveyModel
                {
                    SurveyId = g.Key,
                    SurveyTitle = "Survey " + g.Key,  // 這個可以隱藏，並改為顯示順序
                    AnsweredAt = g.Min(a => a.answered_at), // 取得最早的回答時間
                    QuestionAnswers = g.Select(a => new QuestionAnswerModel
                    {
                        QuestionText = a.question.question_text,
                        Answer = a.answer,
                        AnsweredAt = a.answered_at
                    }).ToList()
                })
                .OrderBy(s => s.AnsweredAt)  // 按照回答時間排序，最早的顯示在最前
                .ToList();

            return View(surveys);
        }

        // 顯示某個問卷的詳細資訊
        public IActionResult Detail(Guid surveyId)
        {
            // 取得當前登入用戶的 ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim.Value);

            // 查詢指定問卷的詳細資料
            var survey = _context.answers
                .Where(a => a.survey_id == surveyId && a.user_id == userId)
                .GroupBy(a => a.survey_id)
                .Select(g => new UserSurveyModel
                {
                    SurveyId = g.Key,
                    SurveyTitle = "Survey " + g.Key,  // 這個可以隱藏，並改為顯示順序
                    AnsweredAt = g.Min(a => a.answered_at),  // 取得最早的回答時間
                    QuestionAnswers = g.Select(a => new QuestionAnswerModel
                    {
                        QuestionText = a.question.question_text,
                        Answer = a.answer,
                        AnsweredAt = a.answered_at
                    }).ToList()
                })
                .FirstOrDefault();

            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }
    }
}