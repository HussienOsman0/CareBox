using CareBox.BLL.Services.FileServices.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.FileServices
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            // 1. تحديد مسار المجلد (wwwroot/uploads/folderName)
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folderName);

            // 2. التأكد إن المجلد موجود
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // 3. تغيير اسم الملف لاسم فريد (عشان لو ملفين بنفس الاسم ميمسحوش بعض)
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            // 4. حفظ الملف
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 5. إرجاع المسار النسبي (عشان يتخزن في الداتابيز)
            // مثال: /uploads/providers/image.jpg
            return $"/uploads/{folderName}/{fileName}";
        }

        public void DeleteFile(string filePath)
        {
            // كود للمسح لو احتجناه بعدين
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
