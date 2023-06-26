using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;

namespace BulkyWebV01.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        // private readonly ApplicationDbContext _db;
        // creating obj of the database.


        private readonly IWebHostEnvironment _webHostEnvironment;


        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
           


        }

        //taking data from the database with the help of database object
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();



            return View(objCompanyList);
        }
        //Read/Display data from the database which is brought by database object
        // accroding to video ::
        //upsert means update and insert

        public IActionResult Upsert(int? id)
        {

            //Generalyy this code is for fetch the categoris in Company .
            // but we use View Model for fetch the category type in create Company.


         

            if (id == null || id == 0)
            {
                // Create
                return View(new Company());
            }
            else
            {
                //update functionality
               Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }


        }

        //insert data
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {


            if (ModelState.IsValid)
            {
                



                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj);
                }

                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }


                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index", "Company");
            }
            else
            {

            


                return View(CompanyObj);
            }
        }

        // edit data

        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    // there are three ways to get data for edit 

        //    Company? CompanyFromDb = _unitOfWork.Company.Get(u => u.Id == id);
        //    // Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
        //    //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

        //    if (CompanyFromDb == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(CompanyFromDb);
        //}


        //// update data

        //[HttpPost]
        //public IActionResult Edit(Company obj)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Company.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Company Updated successfully";
        //        return RedirectToAction("Index", "Company");
        //    }
        //    return View();

        //}


        // delete data

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }


        //    Company? CompanyFromDb = _unitOfWork.Company.Get(u => u.Id == id);


        //    if (CompanyFromDb == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(CompanyFromDb);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    Company? obj = _unitOfWork.Company.Get(u => u.Id == id);

        //    if (obj == null)
        //    {
        //        return NotFound();

        //    }

        //    _unitOfWork.Company.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Company Deleted successfully";
        //    return RedirectToAction("Index", "Company");

        //}




        #region API CALLS

        [HttpGet]

        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            
            return Json(new {data=objCompanyList}); 

        }

        // Delete Data 

        [HttpDelete]

        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u=>u.Id == id);
            if(CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "error while deleting" });
            }

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete SuccessFully" });

        }



        #endregion

    }
}
