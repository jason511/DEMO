using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace DemoWebApplication.Controllers;
[Authorize]
public class ReplyController : Controller
{
    private readonly test1Context _context;

    public ReplyController(test1Context context)
    {
        _context = context;
    }

    // 顯示問題頁面
    public IActionResult Index()
    {
        return View();
    }
    
    // 取得問題列表
    [HttpGet]
    public JsonResult GetQuestions()
    {
        // 產生新的 survey_id（例如可以使用 GUID 或時間戳等方式生成）
        var surveyId = Guid.NewGuid();  // 這裡使用 GUID 作為唯一標識符
        
        var questions = _context.questions.Select(q => new
        {
            q.question_id,
            q.question_text,
            survey_id = surveyId
        }).ToList();

        return Json(questions);
    }

    // 提交答案
    [HttpPost]
    public JsonResult SubmitAnswer([FromBody] AnswerSubmissionModel model)
    {
        try
        {
            // 🔍 Debug 檢查所有的 Claims
            // var allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            // 🔹 取得目前登入的使用者 ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            
            int userId = int.Parse(userIdClaim.Value); // 將 UserId 轉為整數
            var answer = new answers
            {
                question_id = model.QuestionId,
                answer = model.Answer,
                user_id = userId, // ✅ 自動填入目前登入的 UserId
                survey_id = model.SurveyId, // 確保這裡使用正確的 SurveyId
                answered_at = DateTime.Now,
            };
            
            _context.answers.Add(answer);
            _context.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}