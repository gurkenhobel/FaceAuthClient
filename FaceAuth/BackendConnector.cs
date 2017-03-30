using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Documents;
using FaceAuth.Contract;
using FaceAuth.Model;
using FaceAuth.Properties;
using FaceAuth.Util;
using FaceAuth.ViewModel;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

//88.99.174.163

namespace FaceAuth
{
    class BackendConnector
    {
        #region singleton impl

        public static BackendConnector Instance { get; private set; } = new BackendConnector();



        protected BackendConnector()
        {
        }

        #endregion

        private WebSocket _server;
        private bool _closing;

        public event EventHandler<string> OnConnectionError;
        public event EventHandler<List<LockerObject>> OnFileCatalogUpdate;

       



        public void Init()
        {
            _closing = false;
            _server = new WebSocket(Settings.Default.ServerAddress + "/facelock");
            _server.OnError += (s, e) =>
            {
                OnConnectionError?.Invoke(this, Resources.ConnectionLostMessage);
            };


            _server.OnClose += (s, e) =>
            {
                if(!_closing)
                    OnConnectionError?.Invoke(this, Resources.ConnectionLostMessage);
            };

            _server.OnMessage += (s, e) =>
            {
                var msg = JsonConvert.DeserializeObject<Message>(e.Data);
                if (msg.MessageType == MessageType.FileCatalogUpdate)
                {
                    var updateMsg = JsonConvert.DeserializeObject<FileCatalogUpdate>(e.Data);
                    OnFileCatalogUpdate?.Invoke(this, updateMsg.files);
                }
            };


            try
            {
                _server.Connect();
            }
            catch (Exception ex)
            {
                OnConnectionError?.Invoke(this, Resources.CantConnectMessage);
            }
            if (_server.ReadyState != WebSocketState.Open)
            {
                OnConnectionError?.Invoke(this, Resources.CantConnectMessage);
            }
        }

        public void Stop()
        {
            _closing = true;
            _server?.Close(4200);
            _server = null;
        }

        public void Reset()
        {
            Stop();
            Instance = new BackendConnector();
        }

        public async Task<bool> TryRegister(User data, IEnumerable<byte[]> images, int timeoutMs = 120000)
        {
            var imagesBase64 = images.Select(Convert.ToBase64String);
            var msg = new RegisterRequestMessage { Images = imagesBase64, User = data };
            await SendMessage(msg);
            var resp = await WaitForRequestResponse<RegisterResponseMessage>(timeoutMs, MessageType.RegisterResponse);
            return resp != null && resp.success;
        }

        public async Task<AuthResponseMessage> TryAuth(Bitmap img, int timeoutMs = 30000)
        {
            var imgBytes = ImageUtil.BytesFromBitmap(img);
            var imgBase64 = Convert.ToBase64String(imgBytes);
            var msg = new AuthRequestMessage { Image = imgBase64 };
            await SendMessage(msg);
            var resp = await WaitForRequestResponse<AuthResponseMessage>(timeoutMs, MessageType.AuthResponse);
            return resp;
        }

        public async Task RequestFileCatalog()
        {
            var msg = new FileCatalogRequest();
            await SendMessage(msg);
        }

        public async Task<string> RequestFileCreate(LockerObject fileInfo, IOFile file, string password = null)
        {
            try
            {
                var dataJson = JsonConvert.SerializeObject(file);
                var dataJsonEncrypted = fileInfo.encrypted ? Encryption.Encrypt(dataJson, password): dataJson;      

                var msg = new FileCreateRequestMessage { name = fileInfo.name, data = dataJsonEncrypted, encrypted = fileInfo.encrypted};
                await SendMessage(msg);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Resources.FileTooBigMessage;
            }
        }

        public async Task<IOFile> RequestFileDownload(LockerObject fileInfo, string password = "")
        {
            IOFile res = null;
            var msg = new FileDownloadRequestMessage {filename = fileInfo.name};
            await SendMessage(msg);
            var resp = await WaitForRequestResponse<FileDownloadResponseMessage>(60000,
                MessageType.FileDownloadResponse);
            if (resp != null && resp.success)
            {
                var fileJson = fileInfo.encrypted? Encryption.Decrypt(resp.data, password): resp.data;
                res = JsonConvert.DeserializeObject<IOFile>(fileJson);
            }
            return res;
        }

        public async Task RequestFileDelete(LockerObject fileInfo)
        {
            var msg = new FileDeleteRequestMessage {filename = fileInfo.name};
            await SendMessage(msg);
        }


        private async Task<T> WaitForRequestResponse<T>(int timeoutMs, MessageType responseType) where T : Message
        {
            T result = null;
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<MessageEventArgs> callback = (s, e) =>
            {
                var msg = JsonConvert.DeserializeObject<Message>(e.Data);
                if (msg.MessageType != responseType)
                {
                    return;
                }
                var responseMsg = JsonConvert.DeserializeObject<T>(e.Data);

                result = responseMsg;
                tcs.TrySetResult(true);
            };

            _server.OnMessage += callback;

            await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));

            _server.OnMessage -= callback;

            return result;
        }

        private async Task SendMessage(Message msg)
        {
            var json = JsonConvert.SerializeObject(msg);
            if (_server.ReadyState == WebSocketState.Open)
            {
                _server.SendAsync(json, succ =>
                {
                    Console.WriteLine(succ ? $"{msg.MessageType} sent" : $"{msg.MessageType} failed: couldn't send request");   
                });
            }
            else
            {
                _server.OnOpen += (s, e) =>
                {
                    _server.SendAsync(json, succ =>
                    {
                        Console.WriteLine(succ ? $"{msg.MessageType} sent" : $"{msg.MessageType} failed: couldn't send request");
                    });
                };
            }
        }

    }
}
