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
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        // private readonly ApplicationDbContext _db;
        // creating obj of the database.


        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;


        }

        //taking data from the database with the help of database object
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();



            return View(objProductList);
        }
        //Read/Display data from the database which is brought by database object
        // accroding to video ::
        //upsert means update and insert

        public IActionResult Upsert(int? id)
        {

            //Generalyy this code is for fetch the categoris in product .
            // but we use View Model for fetch the category type in create product.


            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()

                }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                // Create
                return View(productVM);
            }
            else
            {
                //update functionality
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties:"ProductImages" );
                return View(productVM);
            }


        }

        //insert data
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile?> files)
        {


            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }

                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }


                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (files != null)
                {
                    foreach(IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        string productPath = @"images\products\product-" +productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if(!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                     using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                     file.CopyTo(fileStream);
                    }


                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" +  productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if(productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);

                       
                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();


                    //if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    //{
                    //    // delete the old image
                    //    var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                    //    if (System.IO.File.Exists(oldImagePath))
                    //    {
                    //        System.IO.File.Delete(oldImagePath);
                    //    }

                    //}

                    //using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    //{
                    //    file.CopyTo(fileStream);
                    //}

                    //productVM.Product.ImageUrl = @"\images\product\" + fileName;

                }

              
                TempData["success"] = "Product created/Updated successfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {

                productVM.CategoryList = _unitOfWork.Category
                     .GetAll().Select(u => new SelectListItem
                     {
                         Text = u.Name,
                         Value = u.Id.ToString()

                     });


                return View(productVM);
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

        //    Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        //    // Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
        //    //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(productFromDb);
        //}


        //// update data

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product Updated successfully";
        //        return RedirectToAction("Index", "Product");
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


        //    Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);


        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(productFromDb);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    Product? obj = _unitOfWork.Product.Get(u => u.Id == id);

        //    if (obj == null)
        //    {
        //        return NotFound();

        //    }

        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product Deleted successfully";
        //    return RedirectToAction("Index", "Product");

        //}


        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                                   Path.Combine(_webHostEnvironment.WebRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }




        #region API CALLS

        [HttpGet]

        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            
            return Json(new {data=objProductList}); 

        }

        // Delete Data 

        [HttpDelete]

        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u=>u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "error while deleting" });
            }

           
            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);                                                                
                }
                Directory.Delete(finalPath);
            }
                
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete SuccessFully" });

        }



        #endregion

    }
}
