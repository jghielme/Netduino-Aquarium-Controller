using System;
using Microsoft.SPOT;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using Webserver.Responses;
using Extensions;

namespace Webserver.POST
{
    /// <summary>
    /// Saves a POST-request at Setting.POST_TEMP_PATH
    /// Sends back "OK" on success
    /// </summary>
    class PostToSdWriter : IDisposable
    {
        private byte[] _buffer;
        private int _startAt;
        private Request _e;

        public PostToSdWriter(Request e, byte[] buffer, int startAt)
        {
            _buffer=buffer;
            _startAt = startAt;
            _e = e;
        }

        /// <summary>
        /// Saves content to Setting.POST_TEMP_PATH
        /// </summary>
        /// <param name="e">The request which should be handeld</param>
        /// <returns>True if 200_OK was sent, otherwise false</returns>
        public bool Receive()
        {
            Debug.Print(Debug.GC(true).ToString());
            Debug.Print(Debug.GC(true).ToString());

            int availableBytes = Convert.ToInt32(_e.Headers["Content-Length"].ToString().TrimEnd('\r'));
			
            try
            {
                FileStream fs = new FileStream(Settings.PostTempPath, FileMode.Create, FileAccess.Write);
                Debug.Print(Debug.GC(true).ToString());
                Debug.Print(Debug.GC(true).ToString());
                
                fs.Write(_buffer,_startAt,_buffer.Length-_startAt);
                availableBytes -= (_buffer.Length-_startAt);

                _buffer = new byte[availableBytes > Settings.MaxRequestSize ? Settings.MaxRequestSize : availableBytes];

                while (availableBytes > 0)
                {
                    if(availableBytes < Settings.MaxRequestSize)
                        _buffer = new byte[availableBytes];

                   // while (_e.Client.Available < _buffer.Length)
                   //     Thread.Sleep(1);

                    _e.Client.Receive(_buffer, _buffer.Length, SocketFlags.None);
			        fs.Write(_buffer, 0, _buffer.Length);
                    availableBytes -= Settings.MaxRequestSize;
                }

                fs.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                Debug.Print("Error writing POST-data");
                return false;
            }

            return true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _buffer = new byte[0];            
        }

        #endregion
    }
}
