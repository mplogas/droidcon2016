//
// CameraPage.xaml.cs
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MimicMe
{
    public partial class CameraView : ContentPage
    {
        public CameraView ()
        {
            InitializeComponent ();
        }

        protected override void OnBindingContextChanged ()
        {
            base.OnBindingContextChanged ();

            var viewModel = BindingContext as MainViewModel;

            //I'm too lazy to create a CustomControl with XAML binding properties for a single occurence
            var tapGestureRecognizer = new TapGestureRecognizer ();
            tapGestureRecognizer.Tapped += async (sender, e) => {
                image.Opacity = .5;
                await Task.Delay (200);
                image.Opacity = 1;
            };
            tapGestureRecognizer.Command = viewModel?.NextPlayer;
            tapGestureRecognizer.CommandParameter = image;

            image.GestureRecognizers.Add (tapGestureRecognizer);
        }
    }
}

