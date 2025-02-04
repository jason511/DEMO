using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoWebApplication.Models;

namespace DemoWebApplication.Controllers;
public class QuestionController : Controller
    {
        private readonly test1Context _context;

        public QuestionController(test1Context context)
        {
            _context = context;
        }

        // 顯示所有問題的頁面
        public async Task<IActionResult> Questions()
        {
            var questions = await _context.questions.ToListAsync();  // 從資料庫讀取所有問題
            return View(questions);  // 把問題資料傳遞到 Questions.cshtml 視圖
        }

        // 顯示新增問題的頁面
        public IActionResult Create()
        {
            return View();
        }

        // 處理新增問題表單提交 (POST 請求)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(questions question)
        {
            if (ModelState.IsValid)
            {
                _context.Add(question);  // 新增問題
                await _context.SaveChangesAsync();  // 保存變更
                return RedirectToAction(nameof(Questions));  // 重定向回問題列表頁面
            }
            return View(question);  // 如果有錯誤，重新顯示表單
        }

        // 顯示編輯問題頁面 (GET 請求)
        public async Task<IActionResult> Edit(int id)
        {
            var question = await _context.questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();  // 找不到對應的問題
            }
            return View(question);  // 返回問題資料
        }

        // 處理編輯問題表單提交 (POST 請求)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, questions question)
        {
            if (id != question.question_id)
            {
                return NotFound();  // 如果 ID 不匹配，返回 404
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);  // 更新問題
                    await _context.SaveChangesAsync();  // 保存變更
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.questions.Any(e => e.question_id == id))
                    {
                        return NotFound();  // 如果問題不存在，顯示 404
                    }
                    throw;
                }
                return RedirectToAction(nameof(Questions));  // 重定向回問題列表頁面
            }
            return View(question);  // 如果表單驗證失敗，重新顯示表單
        }

        // 刪除問題 (GET 請求)
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();  // 找不到問題
            }

            _context.questions.Remove(question);  // 刪除問題
            await _context.SaveChangesAsync();  // 保存變更
            return RedirectToAction(nameof(Questions));  // 重定向回問題列表頁面
        }
    }