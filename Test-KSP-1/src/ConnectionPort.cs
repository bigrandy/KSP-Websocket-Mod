using System;
using System.Collections;
using WebSocket4Net;
using UnityEngine;

namespace TestKSP1

{
    public class ReceivedDataArgs {
        public ReceivedDataArgs(byte [] b) {
            ByteBuff = b;
        }
        public byte[] ByteBuff { get; private set; }
    }
    public class ConnectionPort
    {
        internal bool IsOpen = false;
        private readonly string targetURL;
        private readonly uint baudRate;
        public int BytesToRead { get; internal set; }
		public string PortName { get; internal set; }
        public uint ReceivedBytesThreshold { get; set; }

        public delegate void ReceivedEventHandler(object sender, ReceivedDataArgs e);
        public event ReceivedEventHandler ReceivedEvent;
        static private int STATIC_BUFF_SIZE = 1024 * 1024; //Fixed 1Mb byte buffer for input.
        private byte[] byteBuff = new byte[STATIC_BUFF_SIZE];
        private uint byteBuffOffset = 0;

        private WebSocket _outputSocket;

		private ConnectionPort()
        {
            throw new NotSupportedException("Default Constructor doesn't make sense, you shouldn't be here.");
        }

        public ConnectionPort(string targetURL, uint baudRate)
        {
            this.targetURL = targetURL;
            this.baudRate = baudRate;
            this.PortName = "Socket.IO Out to : " + targetURL;
            try
            {
                Debug.Log("[ConnectionPort] Creating output socket to: " + targetURL);
                //using this constructor so that our UA is set to KSPSerialWebsocket

                _outputSocket = new WebSocket(targetURL);

                _outputSocket.Opened       += ((a,b) => { Debug.Log("[ConnectionPort] Opened..."); this.IsOpen = true; });
                _outputSocket.Error        += ((a,b) => { Debug.Log("[ConnectionPort] Error: " + b.Exception.Message); });
                _outputSocket.Closed       += ((a,b) => { this.IsOpen = false; });
                _outputSocket.DataReceived += ((a,b) =>
                {
                    Debug.Log("[ConnectionPort] Received message for KSP: " + b.Data.ToString());
                    for (int i = 0; i < b.Data.Length; i++){
                        byteBuff[byteBuffOffset] = b.Data[i];
		                byteBuffOffset++;
		                BytesToRead++;
	                }
                    if(this.BytesToRead > ReceivedBytesThreshold ){
                        if (ReceivedEvent != null){
                            ReceivedEvent(this, new ReceivedDataArgs(byteBuff));
                        }
                    }
                });

                Debug.Log("[ConnectionPort] Done creating output socket.");
            } catch (Exception e){
                throw new InvalidOperationException("Failure to create Websocket:" + e.Message);
            }
        }


        internal void Write(byte[] packet, int v, int length)
        {
            //blast them bits
            if (!this.IsOpen)
            {
                try {
                    this.Open();
                }
                catch (Exception e){
                    // screwed
                    throw new UnableToWriteException(e.Message);
                }
            } else {
                Debug.Log("[ConnectionPort] Writing " + length + " byte message to socket...");
                _outputSocket.Send(packet, v, length);
            }
        }

        internal void Open()
        {
            Debug.Log("[ConnectionPort] Attempting to open WebSocket...");
            try
            {
                _outputSocket.Open();

            } catch (Exception e){
                throw new UnableToOpenSocketException(e.Message);
            }
		}

        internal void Close()
        {            
            _outputSocket.Close();
        }

        internal char ReadByte()
        {   
            char retval = (char)this.byteBuff[byteBuffOffset];
            byteBuffOffset--;
            this.BytesToRead--;
            return retval;
        }
    }
}
