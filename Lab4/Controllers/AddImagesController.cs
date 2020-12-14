using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Controllers
{
    public class AddImagesController : Controller
    {
        private readonly SchoolCommunityContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName = "answerimages";

        public AddImagesController(SchoolCommunityContext context, BlobServiceClient blobServiceClient)
         {
             _context = context;
             _blobServiceClient = blobServiceClient;
         }

        /*public AddImagesController(SchoolCommunityContext context)
        {
            _context = context;
           // _blobServiceClient = blobServiceClient;
        }*/

        public async Task<IActionResult> Index(string? ID)
        {

            

            var context = await _context.AddImages.ToListAsync();
                        return View(context);
            /* var viewModel = new AddImageViewModel();
             viewModel.AddImages = await _context.AddImages
                   .Include(i => i.FileName)
                   .Include(i => i.Url)
                   //.ThenInclude(i => i.)
                   .AsNoTracking()
                   .OrderBy(i => i.FileName)
                   .ToListAsync();

             if (ID != null)
             {
                 ViewData["AddImageId"] = ID;
                 viewModel.Community = viewModel.AddImages.Where(
                     x => x.AddImageId == ID).Single().AddImages;
             }

             return View(viewModel);
           /* var viewModel = new AddImageViewModel();
            viewModel.AddImages = await _context.AddImages
                  .Include(i => i.FileName)
                  //.ThenInclude(i => i.)
                  .AsNoTracking()
                  .OrderBy(i => i.FileName)
                  .ToListAsync();

            if (ID != null)
            {
                ViewData["AddImageId"] = ID;
                viewModel. = viewModel.AddImages.Where(
                    x => x.AddImageId == ID).Single().FileName;
            }

            return View(viewModel);*/
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile answerImage)
        {
            BlobContainerClient containerClient;
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }

            try
            {
                if (answerImage != null) {//avoids breaking on null user input 
                var blockBlob = containerClient.GetBlobClient(answerImage.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }
                using (var memoryStream = new MemoryStream())
                {
                    await answerImage.CopyToAsync(memoryStream);

                    memoryStream.Position = 0;
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }
                var image = new AddImage();
                image.Url = blockBlob.Uri.AbsoluteUri;
                image.FileName = answerImage.FileName;
                _context.AddImages.Add(image);
                _context.SaveChanges();
                }
                else{}
            }
            catch (RequestFailedException)
            {
                View("Error");
            }
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var image = await _context.AddImages
                .FirstOrDefaultAsync(m => m.AddImageId == id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.AddImages
                .FindAsync(id);
            BlobContainerClient containerClient;
            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            try
            {
                var blockBlob = containerClient.GetBlobClient(image.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }
                _context.AddImages.Remove(image);
                await _context.SaveChangesAsync();
            }
            catch (RequestFailedException)
            {
                return View("Error");
            }
            return RedirectToAction("Index");
        }




    }
}