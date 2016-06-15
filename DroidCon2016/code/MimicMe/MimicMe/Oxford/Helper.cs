//
// Helper.cs
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

namespace MimicMe
{
    public class Helper
    {
        public static double CalculateEmotionMatch (Emotion emotion1, Emotion emotion2)
        {
            double difference = 0;

            difference += Math.Abs (emotion1.Scores.Anger - emotion2.Scores.Anger);
            difference += Math.Abs (emotion1.Scores.Contempt - emotion2.Scores.Contempt);
            difference += Math.Abs (emotion1.Scores.Disgust - emotion2.Scores.Disgust);
            difference += Math.Abs (emotion1.Scores.Fear - emotion2.Scores.Fear);
            difference += Math.Abs (emotion1.Scores.Happiness - emotion2.Scores.Happiness);
            difference += Math.Abs (emotion1.Scores.Neutral - emotion2.Scores.Neutral);
            difference += Math.Abs (emotion1.Scores.Sadness - emotion2.Scores.Sadness);
            difference += Math.Abs (emotion1.Scores.Surprise - emotion2.Scores.Surprise);

            return 1.0 - (difference / 8);
        }
    }
}

