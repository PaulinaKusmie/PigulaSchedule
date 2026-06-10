
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.PlatformConfiguration;
using PigulaSchedule.Model;
using Plugin.Maui.OCR;
using SQLite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
#if ANDROID
using Android.Graphics;
#endif


namespace PigulaSchedule
{
    class AddSchedule
    {
        private string ocrResult = string.Empty;

        string dbPath = System.IO.Path.Combine(
            FileSystem.AppDataDirectory,
            "pigulaApp.db3");



        //public async Task AddScheduleAsync()
        //{
        //    var photo = await MediaPicker.Default.PickPhotoAsync();

        //    if (photo == null)
        //        return;

        //    using var stream = await photo.OpenReadAsync();

        //    using var ms = new MemoryStream();
        //    await stream.CopyToAsync(ms);

        //    var result = await OcrPlugin.Default
        //        .RecognizeTextAsync(ms.ToArray());

        //    ocrResult = result.AllText;

        //    var resultSchedule = ScheduleParser.Parse(ocrResult);

        //    await SaveData(resultSchedule);
        //}

        public async Task AddScheduleAsync()
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo == null)
                return;

            using var stream = await photo.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var imageBytes = ms.ToArray();

#if ANDROID
    imageBytes = ConvertToBlackAndWhite(imageBytes);
#endif

            var result = await OcrPlugin.Default.RecognizeTextAsync(imageBytes);
            ocrResult = result.AllText;

            var resultSchedule = ScheduleParser.Parse(ocrResult);
            await SaveData(resultSchedule);
        }

#if ANDROID
            private byte[] ConvertToBlackAndWhite(byte[] imageBytes)
            {
                var originalBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                
                var bwBitmap = Android.Graphics.Bitmap.CreateBitmap(
                    originalBitmap!.Width, 
                    originalBitmap.Height, 
                    Android.Graphics.Bitmap.Config.Argb8888!);
                
                var canvas = new Android.Graphics.Canvas(bwBitmap!);
                var paint = new Android.Graphics.Paint();
                var colorMatrix = new ColorMatrix();
                colorMatrix.SetSaturation(0);
                paint.SetColorFilter(new ColorMatrixColorFilter(colorMatrix));
                canvas.DrawBitmap(originalBitmap, 0, 0, paint);
                
                using var ms = new MemoryStream();
                bwBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg!, 100, ms);
                return ms.ToArray();
            }
#endif


        private async System.Threading.Tasks.Task SaveData(List<ShiftDay> jsonPath)
        {
            var database = new SQLiteAsyncConnection(dbPath);

            if (jsonPath == null)
                return;

            await database.CreateTableAsync<ShiftDay>();

            await database.InsertAllAsync(jsonPath);
        }


        public async System.Threading.Tasks.Task DeleteData()
        {
            var database = new SQLiteAsyncConnection(dbPath);
            await database.DeleteAllAsync<ShiftDay>();
        }
    }


//#if ANDROID
//    public class MlKitSuccessListener<T> : Java.Lang.Object, IOnSuccessListener
//        where T : Java.Lang.Object
//    {
//        private readonly Action<T> _action;
//        public MlKitSuccessListener(Action<T> action) => _action = action;
//        public void OnSuccess(Java.Lang.Object result) => _action((T)result);
//    }

//    public class MlKitFailureListener : Java.Lang.Object, IOnFailureListener
//    {
//        private readonly Action<Java.Lang.Exception> _action;
//        public MlKitFailureListener(Action<Java.Lang.Exception> action) => _action = action;
//        public void OnFailure(Java.Lang.Exception e) => _action(e);
//    }
//#endif


}
