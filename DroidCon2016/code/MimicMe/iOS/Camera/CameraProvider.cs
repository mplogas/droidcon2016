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
using System.IO;
using System.Threading.Tasks;

using Xamarin.Forms;
using MimicMe.iOS;

using UIKit;
using Foundation;


[assembly: Dependency(typeof(CameraProvider))]
namespace MimicMe.iOS
{
    public class CameraProvider
    {
        public Task<CameraResult> TakePictureAsync()
        {
            var tcs = new TaskCompletionSource<CameraResult>();

            Camera.TakePicture(UIApplication.SharedApplication.KeyWindow.RootViewController, (imagePickerResult) =>
                {

                    if (imagePickerResult == null)
                    {
                        tcs.TrySetResult(null);
                        return;
                    }

                    var photo = imagePickerResult.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;

                    // You can get photo meta data with using the following
                    // var meta = obj.ValueForKey(new NSString("UIImagePickerControllerMediaMetadata")) as NSDictionary;

                    var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string jpgFilename = Path.Combine(documentsDirectory, Guid.NewGuid() + ".jpg");
                    NSData imgData = photo.AsJPEG();
                    NSError err = null;

                    if (imgData.Save(jpgFilename, false, out err))
                    {
                        CameraResult result = new CameraResult();
                        result.Picture = ImageSource.FromStream(imgData.AsStream);
                        result.FilePath = jpgFilename;

                        tcs.TrySetResult(result);
                    }
                    else
                    {
                        tcs.TrySetException(new Exception(err.LocalizedDescription));
                    }
                });

            return tcs.Task;
        }
    }
}

