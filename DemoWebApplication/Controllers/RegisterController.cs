using System;
using DemoWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BCrypt.Net;

namespace DemoWebApplication.Controllers;

public class RegisterController : Controller
{
    private readonly test1Context _context;

    public RegisterController(test1Context context)
    {
        _context = context;
    }

    // 顯示註冊頁面
    public IActionResult Index()
    {
        return View();
    }

    // 註冊處理
    [HttpPost]
    public IActionResult Index(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // 檢查用戶名是否已存在
            if (_context.users.Any(u => u.username == model.Username))
            {
                ModelState.AddModelError("Username", "此用戶名已被註冊");
                return View(model);
            }

            // 檢查電子郵件是否已存在
            if (_context.users.Any(u => u.email == model.Email))
            {
                ModelState.AddModelError("Email", "此電子郵件已被註冊");
                return View(model);
            }

            // 確認密碼是否一致
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "密碼不一致");
                return View(model);
            }

            // 密碼加密（使用 BCrypt）
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // 創建新用戶
            var newUser = new users
            {
                username = model.Username,
                email = model.Email,
                password_hash = hashedPassword,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            _context.users.Add(newUser);
            _context.SaveChanges();

            // 註冊成功後，跳轉到登入頁面
            return RedirectToAction("Index", "Login");
        }

        // 表單資料無效，重新顯示註冊頁面
        return View(model);
    }
}