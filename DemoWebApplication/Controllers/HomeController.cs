using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoWebApplication.Models;

namespace DemoWebApplication.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly test1Context _context;
    public HomeController(ILogger<HomeController> logger,test1Context context)
    {
        _logger = logger;
        _context = context;
    }
    
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    public async Task<IActionResult> Questions()
    {
        var questions = await _context.questions.ToListAsync();  // 從資料庫讀取所有問題
        return View(questions);  // 把問題資料傳遞到 Questions.cshtml 視圖
    }
    
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
    
    public async Task<IActionResult> Edit(int id)
    {
        var question = await _context.questions.FindAsync(id);
        if (question == null)
        {
            return NotFound();  // 找不到問題
        }
        return View(question);  // 返回修改表單，並傳遞問題資料
    }

    // 處理修改問題表單提交 (POST 請求)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, questions question)
    {
        if (id != question.question_id)
        {
            return NotFound();  // 問題 ID 不匹配
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
                    return NotFound();  // 如果找不到問題，顯示 404 錯誤
                }
                throw;
            }
            return RedirectToAction(nameof(Questions));  // 重定向回問題列表頁面
        }
        return View(question);  // 如果有錯誤，重新顯示表單
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}