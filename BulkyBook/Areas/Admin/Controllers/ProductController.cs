using BulkyBook.Migrations;
using BulkyBook.DataAccess;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using BulkyBook.Models.ViewModels;
using BulkyBook.DataAccess.Repository;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.ObjectPool;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitofwork, IWebHostEnvironment hostEnvironment ) 
        {
            _unitofwork = unitofwork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitofwork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

                CoverTypeList = _unitofwork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

            };
			if (id == null || id == 0)
            {
                //create product 
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(productVM);
            }
            else
            {
                //update product

                productVM.Product = _unitofwork.Product.GetFirstOrDefault(u => u.Id == id);
                return View(productVM);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    if (obj.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using(var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                if (obj.Product.Id == 0)
                {
                    _unitofwork.Product.Add(obj.Product);

                }
                else
                {
                    _unitofwork.Product.Update(obj.Product);

                }
                _unitofwork.Save();
                TempData["Success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        } 

        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitofwork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }

		//POST
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var obj = _unitofwork.Product.GetFirstOrDefault(u => u.Id == id);
			if (obj == null)
			{
				return Json(new { success = false, message = "Error while Deleting" });
			}

			var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

			_unitofwork.Product.Remove(obj);
			_unitofwork.Save();
			return Json(new { success = true, message = "Product Deleted Successfully" });
		}
		#endregion
	}
}
