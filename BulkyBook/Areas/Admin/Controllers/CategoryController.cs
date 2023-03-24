using BulkyBook.Migrations;
using BulkyBook.DataAccess;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]

	public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitofwork;

        public CategoryController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;

        }

        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unitofwork.Category.GetAll();
            return View(objCategoryList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CustomError", "The DisplayOrder cannot match the Name.");
            }
            if (ModelState.IsValid)
            {
                _unitofwork.Category.Add(obj);
                _unitofwork.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }


        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // var categoryFromDb = _db.Find(id); 
            var categoryFromDbFirst = _unitofwork.Category.GetFirstOrDefault(u => u.Id == id);

            if (categoryFromDbFirst == null)
            {
                return NotFound();
            }
            return View(categoryFromDbFirst);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CustomError", "The DisplayOrder cannot match the Name.");
            }
            if (ModelState.IsValid)
            {
                _unitofwork.Category.Update(obj);
                _unitofwork.Save();
                return RedirectToAction("Index");
            }

            return View(obj);
        }


        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryFromDb = _db.Categories.Find(id);
            var categoryFromDbFirst = _unitofwork.Category.GetFirstOrDefault(u => u.Id == id);

            if (categoryFromDbFirst == null)
            {
                return NotFound();
            }
            return View(categoryFromDbFirst);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitofwork.Category.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitofwork.Category.Remove(obj);
            _unitofwork.Save();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
