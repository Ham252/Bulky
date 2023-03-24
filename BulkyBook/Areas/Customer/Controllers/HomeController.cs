using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
	[Area("Customer")]

	public class HomeController : Controller
    {
		private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitofwork;

		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
		{
			_logger = logger;
			_unitofwork = unitOfWork;
		}

		public IActionResult Index()
		{
			IEnumerable<Product> productList = _unitofwork.Product.GetAll(includeProperties: "Category,CoverType");

			return View(productList);
		}

		public IActionResult Details(int productId)
		{
			ShoppingCart cartObj = new()
			{
				Count = 1,
				ProductId = productId,
				Product = _unitofwork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType"),
			};

			return View(cartObj);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCart.ApplicationUserId = claim.Value;

			ShoppingCart cartFromDb = _unitofwork.ShoppingCart.GetFirstOrDefault(
				u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

			if (cartFromDb != null)
			{
				_unitofwork.ShoppingCart.Add(shoppingCart);
			}
			else
			{
				_unitofwork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
			}
			_unitofwork.Save();
			return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}