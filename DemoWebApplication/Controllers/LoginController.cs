using System.Collections.Generic;
using DemoWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace DemoWebApplication.Controllers;
[AllowAnonymous]
public class LoginController : Controller
{
    private readonly test1Context _context;

    public LoginController(test1Context context)
    {
        _context = context;
    }

    // 顯示登入頁面
    public IActionResult Index(string? returnUrl = null) // ✅【修改】接收 returnUrl
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // 登入處理
    [HttpPost]
    public IActionResult Index(LoginViewModel model, string? returnUrl = null) // ✅【修改】接收 returnUrl
    {
        if (ModelState.IsValid)
        {
            var user = _context.users.FirstOrDefault(u => u.username == model.Username || u.email == model.Username);

            if (user != null && VerifyPassword(model.Password, user.password_hash))
            {
                // 登入成功，設定認證 Cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()), // 🔹 加入 UserId
                    new Claim(ClaimTypes.Name, user.username),
                    new Claim(ClaimTypes.Email, user.email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // 設定登入時的 Cookie
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                // ✅【修改】登入成功後，導向 ReturnUrl，否則回首頁
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home"); // 登入成功後跳轉到主頁
            }
            else
            {
                ModelState.AddModelError("", "用戶名或密碼錯誤");
            }
        }
        ViewBag.ReturnUrl = returnUrl; // ✅【新增】確保登入失敗時 returnUrl 仍然存在
        return View(model);
    }

    // 密碼驗證方法
    private bool VerifyPassword(string password, string storedHash)
    {
        // 使用 BCrypt 驗證密碼
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
    // 登出處理
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // 清除認證 Cookie，讓用戶登出
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // 登出後重定向到登入頁面
        return RedirectToAction("Index", "Home");
    }
}