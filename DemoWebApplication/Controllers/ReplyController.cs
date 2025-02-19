using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            survey_id = surveyId,
            weight = (q.difficulty_level == "High" ? 3 :
                      q.difficulty_level == "Medium" ? 2 :
                      q.difficulty_level == "Low" ? 1 : 0)
        }).ToList();

        return Json(questions);
    }

    // 提交答案
    [HttpPost]
    public JsonResult SubmitAnswers([FromBody] AnswerSubmissionModel model)
    {
        try
        {
            // 檢查所有的 Claims
            // var allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            //取得目前登入的使用者 ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            
            int userId = int.Parse(userIdClaim.Value); // 將 UserId 轉為整數
            List<answers> answerList = new List<answers>();
            foreach (var ans in model.Answers)
            {
                var question = _context.questions.FirstOrDefault(q => q.question_id == ans.QuestionId);
                int weight = 0;
                if (question != null)
                {
                    // 將 difficulty_level ("高"/"中"/"低") 轉換為數值權重
                    weight = (question.difficulty_level == "High" ? 3 :
                              question.difficulty_level == "Medium" ? 2 :
                              question.difficulty_level == "Low" ? 1 : 0);
                }
                // **轉換 Answer**：YES -> 1, "NO" -> 0
                int answerValue = ans.Answer.ToUpper() == "YES" ? 1 : 0;

                // 計算分數：權重 (3, 2, 1) * (YES=1, NO=0)
                int score = weight * answerValue;
                
                answerList.Add(new answers
                {
                    question_id = ans.QuestionId,
                    answer = ans.Answer,
                    user_id = userId,
                    survey_id = ans.SurveyId,
                    answered_at = DateTime.Now,
                    score = score
                });
            }
            _context.answers.AddRange(answerList);
            _context.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}