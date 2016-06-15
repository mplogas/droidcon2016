//
// Player.cs
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
using Microsoft.ProjectOxford.Emotion.Contract;
using Xamarin.Forms;

namespace MimicMe
{
    public class Player : BaseNotifyPropertyChanged
    {
        private string name;
        private CameraResult cameraResult;
        private Emotion emotion;
        private double calculatedScore;
        private bool isGameLeader;


        public string Name {
            get {
                return this.name;
            }

            set {
                if (Equals (this.name, value)) return;

                this.name = value;
                OnPropertyChanged ("Name");
            }
        }
        public CameraResult CameraResult {
            get { return this.cameraResult; }
            set {
                if (Equals (this.cameraResult, value)) return;
                this.cameraResult = value;
                OnPropertyChanged ("CameraResult");
            }
        }
        public Emotion Emotion {
            get { return this.emotion; }
            set {
                if (Equals (this.emotion, value)) return;
                this.emotion = value;
                OnPropertyChanged ("Emotion");
            }
        }
        public double CalculatedScore {
            get { return this.calculatedScore; }
            set {
                if (Equals (this.calculatedScore, value)) return;
                this.calculatedScore = value;
                OnPropertyChanged ("CalculatedScore");
            }
        }
        public bool IsGameLeader {
            get { return this.isGameLeader; }
            set {
                if (Equals (this.isGameLeader, value)) return;
                this.isGameLeader = value;
                OnPropertyChanged ("IsGameLeader");
            }
        }
    }
}

