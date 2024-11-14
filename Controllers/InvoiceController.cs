using Core.Entities;
using Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesWebApp.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly InvoiceService _invoiceService;
        
        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        // GET: Invoice
        public ActionResult Index()
        {
            var invoices = _invoiceService.GetAll();
            return View(invoices);
        }

        // GET: Invoice/Details/5
        public ActionResult Details(int id)
        {
            var invoice = _invoiceService.GetById(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: Invoice/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Invoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _invoiceService.Add(invoice);
                return RedirectToAction("Index");
            }
            return View(invoice);
        }

        // GET: Invoice/Edit/5
        public ActionResult Edit(int id)
        {
            var invoice = _invoiceService.GetById(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
           
            return View(invoice);
        }

        // POST: Invoice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _invoiceService.Update(invoice);
                return RedirectToAction("Index");
            }
            return View(invoice);
        }

        // GET: Invoice/Delete/5
        public ActionResult Delete(int id)
        {
            var invoice = _invoiceService.GetById(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _invoiceService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}