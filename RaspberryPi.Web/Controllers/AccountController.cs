using Microsoft.AspNetCore.Mvc;

namespace RaspberryPi.Web.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext _context;
        SignInManager<IdentityUser> _signInManager;
        public AccountController(ApplicationDbContext dbContext, SignInManager<IdentityUser> signInManager)
        {
            _context = dbContext;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username.IsNullOrWhiteSpace() || password.IsNullOrWhiteSpace())
            {
                ViewBag.Error = "ورودی ها خالی است";
                return View();
            }

            var user = _context.Users.Where(x => x.UserName == username).FirstOrDefault();
            if (user == null)
            {
                ViewBag.Error = "کاربر یافت نشد";
                return Redirect("/");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return View();
            }
            else if (!result.Succeeded)
            {
                ViewBag.Error = "رمز عبور وارد شده نادرست است";
                return View();                
            }
            else if (result.IsNotAllowed)
            {
                ViewBag.Error = "حساب کاربری شما غیر فعال شده است";
                return View();
            }
            else if (result.IsLockedOut)
            {
                ViewBag.Error = "بدلیل ورود بیش از حد رمز اشتباه این اکانت به صورت موقت مسدود گردید. لطفا کمی بعد مجددا تلاش کنید";
                return View();                
            }
            ViewBag.Error = "خطا";
            return View();
        }
    }
}
