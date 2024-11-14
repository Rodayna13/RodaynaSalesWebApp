using Core.Entities;
using Core.UseCases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;

namespace SalesWebApp.Controllers
{
    public class ItemController : Controller
    {
        private readonly ItemService _itemService;
        private readonly CategoryService _categoryService;

        public ItemController(ItemService itemService, CategoryService categoryService)
        {
            _itemService = itemService;
            _categoryService = categoryService;
        }

        // GET: Item
        public ActionResult Index()
        {
            var items = _itemService.GetAllItems();
            return View(items);
          
        }

        // GET: Item/Details/5
        public ActionResult Details(int id)
        {
            var item = _itemService.GetItemById(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Item/Create
        public ActionResult Create()
        {
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "Id", "Name");
            return View();
        }

        // Post: Item/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public ActionResult Create(Item item)
        {
            if (ModelState.IsValid)
            {
                if (item.ImageFile != null && item.ImageFile.Length > 0)
                {
                    if (!item.AreValidImages())
                    {
                        ModelState.AddModelError("ImageFile", "Invalid image format.");
                        ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "Id", "Name", item.CategoryId);
                        return View(item);
                    }

                    foreach (var file in item.ImageFile)
                    {
                        try
                        {
                            string uploadsFolder = Server.MapPath("~/images");
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            file.SaveAs(filePath); // Use SaveAs for HttpPostedFileBase

                            item.ImagePath = "/images/" + uniqueFileName; // Adjust as needed
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("ImageFile", "Error uploading file: " + ex.Message);
                            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "Id", "Name", item.CategoryId);
                            return View(item);
                        }
                    }
                }

                _itemService.AddItem(item);
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "Id", "Name", item.CategoryId);
            return View(item);
        }



        // GET: Item/Edit/5
        public ActionResult Edit(int id)
        {
            var item = _itemService.GetItemById(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "Id", "Name", item.CategoryId);
            return View(item);
        }

        // POST: Item/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Item item, HttpPostedFileBase[] ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    foreach (var file in ImageFile)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            string uploadsFolder = Path.Combine(Server.MapPath("~/images"));
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            file.SaveAs(filePath);

                            // Update the item with the new image path (adjust as needed)
                            item.ImagePath = "/images/" + uniqueFileName;
                        }
                    }
                }

                _itemService.UpdateItem(item);
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "Id", "Name", item.CategoryId);
            return View(item);
        }



        // GET: Item/Delete/5
        public ActionResult Delete(int id)
        {
            var item = _itemService.GetItemById(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _itemService.DeleteItem(id);
            return RedirectToAction("Index");
        }



    }
}
