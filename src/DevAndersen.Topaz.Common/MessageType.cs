using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevAndersen.Topaz.Client
{
    public enum MessageType
    {
        Exit,
        Error,
        Wait,
        GetText,
        GetNumber,
        GetDate,
        GetTime,
        GetDateTime,
        Say,
        Show,
        Notification,
        ShowHTML,
        OpenURL,
        ScanQR,
        SelectOne,
        SelectMultiple,
        GetClipboard,
        SetClipboard,
        GetLocation,
        Vibrate,
        SendAudio,
        StreamAudio,
        GetVolume,
        SetVolume,
        GetBrightness,
        SetBrightness,
        RunShortcut,
        SendTextMessage,
        SendMail,
        GetBattery
    }
}
