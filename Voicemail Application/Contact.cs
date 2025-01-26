using System;
using System.ComponentModel;
using System.Media;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using NAudio.Wave;

namespace VoicemailApplication
{
    /// <summary>
    /// The Contact
    /// </summary>
    public class Contact : INotifyPropertyChanged
    {
        /// <summary>
        /// The call btn pressed
        /// </summary>
        private bool callBtnPressed;

        /// <summary>
        /// The name
        /// </summary>
        private string name;

        /// <summary>
        /// The number
        /// </summary>
        private string number;

        /// <summary>
        /// The sound player
        /// </summary>
        private SoundPlayer player;

        /// <summary>
        /// The property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The voicemail duration
        /// </summary>
        private TimeSpan voicemailDuration;

        /// <summary>
        /// The Contact contructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <param name="player"></param>
        /// <param name="voicemaillocation"></param>
        public Contact(string name, string number, SoundPlayer player, string voicemaillocation)
        {
            WaveFileReader wf = new WaveFileReader(voicemaillocation);
            this.name = name;
            this.number = number;
            this.player = player;
            this.voicemailDuration = wf.TotalTime;
        }

        /// <summary>
        /// Gets and Sets the Call btn pressed property
        /// </summary>
        public bool CallBtnPressed
        {
            get { return this.callBtnPressed; }
            set
            {
                callBtnPressed = value;
                RaisedPropertyChanged("CallBtnPressed");
            }
        }



        /// <summary>
        /// The call BTN pressed command.
        /// </summary>
        public ICommand CallBtnPressedCommand => new RelayCommand(this.CallContact);

        /// <summary>
        /// The contact name
        /// </summary>
        public string Name => this.name;

        /// <summary>
        /// The contact number
        /// </summary>
        public string Number => this.number; 

        /// <summary>
        /// The sound player
        /// </summary>
        public SoundPlayer Player => this.player;

        /// <summary>
        /// The voicemail duration
        /// </summary>
        public TimeSpan VoicemailDuration => this.voicemailDuration;

        /// <summary>
        /// Calls the contact
        /// </summary>
        public void CallContact()
        {
            this.CallBtnPressed = true;
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisedPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
