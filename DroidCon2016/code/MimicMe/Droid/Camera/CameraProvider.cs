//
// CameraProvider.cs
//
// Author:
//       Marc Plogas <marc.plogas@microsoft.com>
//
// Copyright (c) 2016 microsoft
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Threading.Tasks;
using MimicMe.Droid;
using Xamarin.Forms;

using Java.IO;

using Android.App;
using Android.Content;
using Android.Provider;

using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Content.PM;
using System.Collections.Generic;
using Android.Graphics;

[assembly: Dependency (typeof (CameraProvider))]
namespace MimicMe.Droid
{
    public class CameraProvider : ICameraProvider
    {
        private static File file;
        private static TaskCompletionSource<CameraResult> tcs;

        public Task<CameraResult> TakePictureAsync ()
        {
            var activity = (Activity)Forms.Context;

            if (HasCameraApp ()) {
                File pictureDirectory;
                Intent intent = new Intent (MediaStore.ActionImageCapture);

                pictureDirectory = CreateDirectoryForPictures ("MimicMe");
                file = new File (pictureDirectory, String.Format ("photo_{0}.jpg", Guid.NewGuid ()));

                intent.PutExtra (MediaStore.ExtraOutput, Uri.FromFile (file));

                activity.StartActivityForResult (intent, 0);

                tcs = new TaskCompletionSource<CameraResult> ();

                return tcs.Task;
            } else {
                throw new Exception ("no camera app installed");
            }

        }

        public static async Task OnResult (Result resultCode)
        {
            if (resultCode == Result.Canceled) {
                tcs.TrySetResult (null);
                return;
            }

            if (resultCode != Result.Ok) {
                tcs.TrySetException (new Exception ("Unexpected error"));
                return;
            }


            await PostProcessPicture (file.Path);
            var result = new CameraResult ();
            result.Picture = ImageSource.FromFile (file.Path);
            result.FilePath = file.Path;

            tcs.TrySetResult (result);
        }

        private bool HasCameraApp ()
        {
            //Intent intent = new Intent (MediaStore.ActionImageCapture);
            //IList<ResolveInfo> availableActivities = Forms.Context.PackageManager.QueryIntentActivities (intent, PackageInfoFlags.MatchDefaultOnly);
            //return availableActivities != null && availableActivities.Count > 0;

            return true;
        }

        private File CreateDirectoryForPictures (string subfolder)
        {
            var directory = new File (Android.OS.Environment.GetExternalStoragePublicDirectory (Android.OS.Environment.DirectoryPictures), subfolder);
            if (!directory.Exists ()) {
                directory.Mkdirs ();
            }

            return directory;
        }


        private static async Task PostProcessPicture (string path)
        {
            Bitmap bitmap = await Task.Run (() => {
                //Get the bitmap with the right rotation
                var tmp = BitmapHelper.GetAndRotateBitmap (path);
                //Resize the picture to be under 4MB (Emotion API limitation and better for Android memory)
                return Bitmap.CreateScaledBitmap (tmp, 2000, (int)(2000 * tmp.Height / tmp.Width), false);
            });

            using (var stream = new System.IO.MemoryStream ()) {
                await bitmap.CompressAsync (Bitmap.CompressFormat.Jpeg, 90, stream);
                stream.Seek (0, System.IO.SeekOrigin.Begin);

                //overwrite the existing image
                using (var writer = System.IO.File.Create (path)) {
                    await writer.WriteAsync (stream.ToArray (), 0, (int)stream.Length);
                }
            }
        }
    }
}

