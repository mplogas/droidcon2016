//
// FilesystemRepository.cs
//
// Author:
//       Marc Plogas <marc.plogas@microsoft.com>
//
// Copyright (c) 2016 Marc Plogas @ Microsoft
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
using MimicMe.Droid;
using Xamarin.Forms;
using Environment = Android.OS.Environment;

[assembly: Dependency (typeof (FilesystemRepository))]
namespace MimicMe.Droid
{
    public class FilesystemRepository : IFilesystemRepository
    {

        public bool FileExists (string filePath)
        {
            //return File.Exists(Path.Combine (Environment.GetExternalStoragePublicDirectory (Android.OS.Environment.DirectoryPictures), fileName);
            return File.Exists (filePath);
        }

        public async Task<byte []> ReadFileAsync (string filePath)
        {
            if (!FileExists (filePath)) throw new ArgumentException ("file does not exists!");

            byte [] result;

            using (var filestream = File.Open (filePath, FileMode.Open)) {
                result = new byte [filestream.Length];
                await filestream.ReadAsync (result, 0, (int)filestream.Length);
            }

            return result;
        }

        public async Task SaveFileAsync (byte [] buffer, string filePath)
        {
            using (var filestream = File.Open (filePath, FileMode.OpenOrCreate, FileAccess.Write)) {
                await filestream.WriteAsync (buffer, 0, buffer.Length);
            }
        }
    }
}

