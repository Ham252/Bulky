using BulkyBook.Migrations;
using BulkyBook.DataAccess;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitofwork;

        public CoverTypeController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;

        }

        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverTypeList = _unitofwork.CoverType.GetAll();
            return View(objCoverTypeList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType obj)
        {
            if (ModelState.IsValid)
            {
                _unitofwork.CoverType.Add(obj);
                _unitofwork.Save();
                TempData["success"] = "CoverType Created Successfully";
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
            var CoverTypeFromDbFirst = _unitofwork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if (CoverTypeFromDbFirst == null)
            {
                return NotFound();
            }
            return View(CoverTypeFromDbFirst);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType obj)
        {
            if (ModelState.IsValid)
            {
                _unitofwork.CoverType.Update(obj);
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
            var CoverTypeFromDbFirst = _unitofwork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if (CoverTypeFromDbFirst == null)
            {
                return NotFound();
            }
            return View(CoverTypeFromDbFirst);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitofwork.CoverType.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitofwork.CoverType.Remove(obj);
            _unitofwork.Save();
            TempData["success"] = "CoverType Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
