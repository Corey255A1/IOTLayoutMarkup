using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
namespace IOTClientWPF
{

    public delegate void AddCtrlCallback(PacketHeaders.AddCtrl ac);
    public delegate void UpdateCtrlCallback(PacketHeaders.CtrlData ac);
    class SerialInterface
    {
        SerialPort mCommPort;
        ReadStates mReadState = ReadStates.READ;
        bool bGotUpdateSize = false;
        int mBytesToRead = 0;
        List<Byte> mByteBuffer = new List<byte>();

        public AddCtrlCallback AddCtrlEvent;
        public UpdateCtrlCallback UpdateCtrlEvent;

        public SerialInterface()
        {
            mCommPort = new SerialPort();
            mCommPort.PortName = "COM5";
            mCommPort.BaudRate = 57600;
            mCommPort.Parity = Parity.None;
            mCommPort.DataBits = 8;
            mCommPort.StopBits = StopBits.One;
            mCommPort.Handshake = Handshake.None;

            mCommPort.Open();
            mCommPort.DataReceived += MCommPort_DataReceived;
        }

        public void GetPage(byte id)
        {
            mCommPort.Write(new PacketHeaders.GetPage(id).GetBytes(),0,PacketHeaders.SizeOfGetPageStruct+1);
        }

        public void SetControl(byte pid, byte id, byte[] data)
        {
            byte[] b = new PacketHeaders.CtrlData(pid, id, data).GetBytes();
            mCommPort.Write(b, 0, b.Count());
        }

        private void MCommPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                while (mCommPort.BytesToRead > 0)
                {
                    byte b = (byte)mCommPort.ReadByte();
                    Console.Write(b);
                    switch (mReadState)
                    {
                        case ReadStates.READ:
                            {
                                mByteBuffer.Clear();
                                switch (b)
                                {
                                    case PacketHeaders.ADD_CTRL: mReadState = ReadStates.ADD_CTRL; mBytesToRead = PacketHeaders.SizeOfAddCtrlStruct; break;
                                    case PacketHeaders.UPDATE_CTRL: mReadState = ReadStates.UPDATE_CTRL; mBytesToRead = PacketHeaders.SizeOfBaseCtrlDataStruct; bGotUpdateSize = false; break;
                                }
                            }break;
                        case ReadStates.ADD_CTRL:
                            {
                                mByteBuffer.Add(b);
                                if(--mBytesToRead == 0)
                                {
                                    Console.WriteLine("GOT AN ADD CTRL");
                                    mReadState = ReadStates.READ;
                                    AddCtrlEvent?.Invoke(new PacketHeaders.AddCtrl(mByteBuffer));
                                }
                            }
                            break;
                        case ReadStates.UPDATE_CTRL:
                            {
                                mByteBuffer.Add(b);
                                if (--mBytesToRead == 0)
                                {
                                    if (!bGotUpdateSize)
                                    {
                                        int size = (Int16)(((Int16)mByteBuffer[3] << 8) | mByteBuffer[2]);
                                        mBytesToRead = size;
                                        bGotUpdateSize = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("GOT AN UPDATE CTRL");
                                        mReadState = ReadStates.READ;
                                        UpdateCtrlEvent?.Invoke(new PacketHeaders.CtrlData(mByteBuffer));
                                    }                                    
                                }
                            }
                            break;

                    }

                } 
            }
        }
    }
}
