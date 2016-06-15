//
// CameraViewModel.cs
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
using System.Linq;
using System.ComponentModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.IO;
using System.Collections.Generic;
using Microsoft.ProjectOxford.Emotion.Contract;
using System.Collections.ObjectModel;

namespace MimicMe
{
    public class MainViewModel : BaseNotifyPropertyChanged
    {
        private Player current;
        private string error;

        private ICameraProvider cameraProvider;
        private IFilesystemRepository filesystem;
        private IEmotionProvider emotionProvider;

        public Player Current {
            get {
                return this.current;
            }
            set {
                if (Equals (value, this.current)) {
                    return;
                }
                this.current = value;
                OnPropertyChanged ("Current");
            }
        }

        public string Error {
            get {
                return this.error;
            }
            set {
                if (Equals (value, this.error)) {
                    return;
                }
                this.error = value;
                OnPropertyChanged ("Error");
            }
        }

        public ObservableCollection<Player> Game { get; set; }

        public ICommand TakePicture { get; set; }
        public ICommand NextPlayer { get; set; }
        public ICommand ResetGame { get; set; }


        public MainViewModel (ICameraProvider cameraProvider, IFilesystemRepository filesystem, IEmotionProvider emotionprovider)
        {
            if (cameraProvider == null) {
                throw new ArgumentNullException ("cameraProvider");
            }
            if (filesystem == null) {
                throw new ArgumentNullException ("filesystem");
            }
            if (emotionprovider == null) {
                throw new ArgumentNullException ("emotionprovider");
            }

            this.Current = new Player () { IsGameLeader = true };
            this.Game = new ObservableCollection<Player> ();

            TakePicture = new Command (async () => await TakePictureAsync ());
            NextPlayer = new Command (SaveAndClearPlayer);
            ResetGame = new Command (ClearGame);
            this.cameraProvider = cameraProvider;
            this.filesystem = filesystem;
            this.emotionProvider = emotionprovider;
        }

        void ClearGame ()
        {
            Current = new Player () { IsGameLeader = true };

            Game.Clear ();
            Error = string.Empty;
        }

        private void SaveAndClearPlayer ()
        {
            Game.Add (Current);
            Current = new Player ();
            Error = String.Empty;
        }

        private async Task TakePictureAsync ()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync (Permission.Storage);

            if (status != PermissionStatus.Granted) {
                var results = await CrossPermissions.Current.RequestPermissionsAsync (new [] { Permission.Storage });
                status = results [Permission.Location];
            }

            Error = String.Empty;
            Current.CameraResult = await cameraProvider.TakePictureAsync ();

            await GetOxfordScore ();
        }

        private async Task GetOxfordScore ()
        {
            if (filesystem.FileExists (Current.CameraResult.FilePath)) {
                var bytes = await filesystem.ReadFileAsync (Current.CameraResult.FilePath);

                try {
                    var result = await emotionProvider.GetEmotionResultsAsync (new MemoryStream (bytes));
                    if (result.Any ()) {
                        Current.Emotion = result.First ();

                        if (!Current.IsGameLeader) {
                            Current.CalculatedScore = Helper.CalculateEmotionMatch (Game.First (p => p.IsGameLeader == true).Emotion, Current.Emotion);
                        } else {
                            Current.CalculatedScore = 1;
                        }

                    }
                } catch (Exception e) {
                    Error = e.Message;
                }
            }
        }
    }
}

