using Core.Entities;
using Core.UseCases;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesWebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Category
        public ActionResult Index()
        {
            ViewBag.Message = TempData["Message"];
            var categories = _categoryService.GetAllCategories();
            return View(categories);
        }

        // GET: Category/Details/5
        public ActionResult Details(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryService.AddCategory(category);
                TempData["Message"] = "Category created successfully.";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryService.UpdateCategory(category);
                TempData["Message"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Category/Delete/5
        public ActionResult Delete(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _categoryService.DeleteCategory(id);
            TempData["Message"] = "Category deleted successfully.";
            return RedirectToAction("Index");
        }

        // GET: Category/UploadExcel
        public ActionResult UploadExcel()
        {
            return View(); // Renders the UploadExcel.cshtml view
        }

        // POST: Category/UploadExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadExcel(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("File", "Please upload a file.");
                return View();
            }

            try
            {
                // Set EPPlus License context to non-commercial
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var categories = new List<Category>();

                using (var package = new ExcelPackage(file.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var name = worksheet.Cells[row, 1].Text;

                        // Validate that the name is not empty
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            ModelState.AddModelError("File", $"Row {row}: Category name is required.");
                            continue;
                        }

                        var category = new Category { Name = name };
                        categories.Add(category);
                    }
                }

                // Add categories to the database
                foreach (var category in categories)
                {
                    _categoryService.AddCategory(category);
                }

                TempData["Message"] = "Categories uploaded successfully from Excel.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("File", $"Error uploading file: {ex.Message}");
                return View();
            }
        }
    }
}
