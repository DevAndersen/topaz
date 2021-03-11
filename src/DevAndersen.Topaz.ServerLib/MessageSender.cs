using DevAndersen.Topaz.Client;
using DevAndersen.Topaz.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevAndersen.Topaz.ServerLib
{
    public class MessageSender
    {
        private readonly Server server;

        public MessageSender(Server server)
        {
            this.server = server;
        }

        #region Exit

        /// <summary>
        /// Exit the shortcut.
        /// </summary>
        public void SendExit()
        {
            server.SendMessagesWithoutRead(MessageType.Exit);
        }

        /// <summary>
        /// Exit the shortcut with <paramref name="result"/>.
        /// </summary>
        /// <param name="result"></param>
        public void SendExit(string result)
        {
            server.SendMessagesWithoutRead(new Message(MessageType.Exit, result));
        }

        #endregion

        #region Error

        public void SendError(string errorMessage)
        {
            server.SendMessagesWithoutRead(new Message(MessageType.Error, errorMessage));
        }

        #endregion

        #region Wait

        /// <summary>
        /// Make the shortcut wait <paramref name="seconds"/> seconds.
        /// <br/>
        /// Due to data transfer time and other minor and other potentially varying delays, this should not be used for precise timing.
        /// </summary>
        /// <param name="seconds"></param>
        public void SendWait(int seconds)
        {
            server.SendMessage(new Message(MessageType.Wait, seconds.ToString()));
        }

        #endregion

        #region GetText

        /// <summary>
        /// Get user input string, asking <paramref name="prompt"/>.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public string GetText(string prompt)
        {
            return server.SendMessage(new Message(MessageType.GetText, prompt));
        }

        /// <summary>
        /// Get user input string, asking <paramref name="prompt"/>, and <paramref name="defaultValue"/> as default value.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetText(string prompt, string defaultValue)
        {
            return server.SendMessage(new Message(MessageType.GetText, prompt, defaultValue));
        }

        #endregion

        #region GetNumber

        #endregion

        #region GetDate

        #endregion

        #region GetTime

        #endregion

        #region GetDateTime

        #endregion

        #region Say

        #endregion

        #region Show

        #endregion

        #region Notification

        #endregion

        #region ShowHTML

        #endregion

        #region OpenURL

        #endregion

        #region ScanQR

        #endregion

        #region SelectOne

        public string SelectOne(IEnumerable<string> options)
        {
            return SelectOne(options.ToDictionary(x => x));
        }

        public string SelectOne(Dictionary<string, string> options)
        {
            return server.SendMessage(new Message(MessageType.SelectOne, JsonSerializer.Serialize(options)));
        }

        #endregion

        #region SelectMultiple

        public string SelectMultiple(IEnumerable<string> options)
        {
            return SelectMultiple(options.ToDictionary(x => x));
        }

        public string SelectMultiple(Dictionary<string, string> options)
        {
            return server.SendMessage(new Message(MessageType.SelectMultiple, JsonSerializer.Serialize(options)));
        }

        #endregion

        #region GetClipboard

        #endregion

        #region SetClipboard

        #endregion

        #region GetLocation

        #endregion

        #region Vibrate

        #endregion

        #region SendAudio

        #endregion

        #region StreamAudio

        #endregion

        #region GetVolume

        /// <summary>
        /// Get the audio volume level.
        /// The returned value can range from 0.0 (0% volume) to 1.0 (100% volume).
        /// </summary>
        /// <returns></returns>
        public double GetVolume()
        {
            return double.Parse(server.SendMessage(MessageType.GetVolume));
        }

        #endregion

        #region SetVolume

        /// <summary>
        /// Set the audio volume level.
        /// <paramref name="volume"/> should be a value between 0.0 (0% volume) and 1.0 (100% volume).
        /// </summary>
        /// <param name="volume"></param>
        /// <returns>The new volume level.</returns>
        public double SetVolume(double volume)
        {
            // If volume is lower than 0 or higher than 1, set it to 0.0 or 1.0, respectively.
            volume = volume switch
            {
                < 0 => 0,
                > 1 => 1,
                _ => volume
            };
            string response = server.SendMessage(new Message(MessageType.SetVolume, volume.ToString()));
            return double.Parse(response);
        }

        #endregion

        #region GetBrightness

        /// <summary>
        /// Get the screen brightness level.
        /// The returned value can range from 0.0 (0% brightness) to 1.0 (100% brightness).
        /// </summary>
        /// <returns></returns>
        public double GetBrightness()
        {
            return double.Parse(server.SendMessage(MessageType.GetBrightness));
        }

        #endregion

        #region SetBrightness

        /// <summary>
        /// Set the screen brightness level.
        /// <paramref name="brightness"/> should be a value between 0.0 (0% brightness) and 1.0 (100% brightness).
        /// </summary>
        /// <param name="brightness"></param>
        /// <returns>The new brightness level.</returns>
        public double SetBrightness(double brightness)
        {
            // If brightness is lower than 0 or higher than 1, set it to 0.0 or 1.0, respectively.
            brightness = brightness switch
            {
                < 0 => 0,
                > 1 => 1,
                _ => brightness
            };
            string response = server.SendMessage(new Message(MessageType.SetBrightness, brightness.ToString()));
            return double.Parse(response);
        }

        #endregion

        #region RunShortcut

        #endregion

        #region SendTextMessage

        #endregion

        #region SendMail

        #endregion

        #region GetBattery
        public double GetBattery()
        {
            return double.Parse(server.SendMessage(MessageType.GetBattery));
        }

        #endregion

    }
}
