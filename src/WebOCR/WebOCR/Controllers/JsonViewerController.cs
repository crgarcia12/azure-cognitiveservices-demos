using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebOCR.Models;
using WebOCR.Services;

namespace WebOCR.Controllers
{
    public class JsonViewerController : Controller
    {
        // GET: JsonViewer
        

        // GET: JsonViewer/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: JsonViewer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: JsonViewer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: JsonViewer/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: JsonViewer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: JsonViewer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: JsonViewer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}