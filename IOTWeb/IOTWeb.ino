const char GET_PAGE = 1;
const int SizeOfGetPage = 1;

const char ADD_CTRL = 2;
const int SizeOfAddCtrl = 26;

const char UPDATE_CTRL = 3;
const char SET_CTRL = 4;
const int SizeOfBaseCtrlData = 4;

enum RX_STATE {RX_READ, RX_GETPAGE, RX_SETCTRL};

struct AddCtrlStruct
{
  const unsigned char hdr = ADD_CTRL;
  unsigned char ID;
  unsigned char Type;
  unsigned short X;
  unsigned short Y;
  unsigned short Width;
  unsigned short Height;
  char Label[16];
};

struct UpdateCtrlStruct
{
  const unsigned char hdr = UPDATE_CTRL;
  unsigned char PageID;
  unsigned char ID;
  unsigned short Size;
  char Data[256];
};

RX_STATE mRxState = RX_READ;
int mBytesRemaining = 0;
boolean dataComplete = false;

void setup() {
  // initialize serial:
  Serial.begin(57600);
}

void loop() {
  // print the string when a newline arrives:
  if (dataComplete) {
    dataComplete = false;
  }
}

/*
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
*/

void serialEvent() {
  while (Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read();
    switch(mRxState)
    {
      case RX_READ:
      {
        switch(inChar)
        {
          case GET_PAGE: mRxState = RX_GETPAGE; mBytesRemaining = SizeOfGetPage; break;
        }
      }break;
      case RX_GETPAGE:
      {       
        mBytesRemaining=0;
        GetPage(inChar);
        mRxState = RX_READ;
      }break;
      }
    }
}


void SendCtrl(char id, char type, unsigned short x, unsigned short y, unsigned short width, unsigned short height, const char lbl[16])
{
  AddCtrlStruct acb;
  acb.ID = id;
  acb.Type = type;
  acb.X = x;
  acb.Y = y;
  acb.Width = width;
  acb.Height = height;
  memset(acb.Label,0,16);
  memcpy(acb.Label, lbl, strlen(lbl));
  Serial.write((char*)&acb,SizeOfAddCtrl+1);
}

void UpdateCtrl(char pid, char id, unsigned short psize, const char data[256])
{
  UpdateCtrlStruct ucs;
  ucs.PageID = pid;
  ucs.ID = id;
  ucs.Size = psize;
  memset(ucs.Data,0,16);
  memcpy(ucs.Data, data, psize);
  Serial.write((char*)&ucs, SizeOfBaseCtrlData+1+psize); 
}

void GetPage(char pageNum)
{
  if(pageNum == 0)
  {
    SendCtrl(0, 1, 100, 200, 100, 25, "TEST BUTTON\0");
    SendCtrl(1, 2, 100, 200, 100, 25, "Read Only Edit\0");
    UpdateCtrl(0, 1, 6, "HELLO\0");
    
  }
}


