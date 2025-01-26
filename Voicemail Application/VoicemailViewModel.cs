using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Timers;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using NAudio.Wave;
using VoicemailApplication;

namespace Voicemail_Application
{
    /// <summary>
    /// The Voicemail Viewmodel
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class VoicemailViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// The audio path
        /// </summary>
        private string audioPath = Directory.GetParent(Assembly.GetEntryAssembly().Location).ToString() + @"\Audio\";

        /// <summary>
        /// The button list
        /// </summary>
        private List<KeypadButton> buttonList = new List<KeypadButton>();

        /// <summary>
        /// The call history list
        /// </summary>
        private ObservableCollection<string> callHistoryList = new ObservableCollection<string>();

        /// <summary>
        /// The contact list
        /// </summary>
        private ObservableCollection<Contact> contactList = new ObservableCollection<Contact>();

        /// <summary>
        /// The dialed contact
        /// </summary>
        private Contact dialedContact;

        /// <summary>
        /// The in call
        /// </summary>
        private bool inCall;

        /// <summary>
        /// The number zero button
        /// </summary>
        private KeypadButton numberZeroButton = new KeypadButton("0");

        /// <summary>
        /// The number one button
        /// </summary>
        private KeypadButton numberOneButton = new KeypadButton("1");

        /// <summary>
        /// The number two button
        /// </summary>
        private KeypadButton numberTwoButton = new KeypadButton("2");

        /// <summary>
        /// The number three button
        /// </summary>
        private KeypadButton numberThreeButton = new KeypadButton("3");

        /// <summary>
        /// The number four button
        /// </summary>
        private KeypadButton numberFourButton = new KeypadButton("4");

        /// <summary>
        /// The number five button
        /// </summary>
        private KeypadButton numberFiveButton = new KeypadButton("5");

        /// <summary>
        /// The number six button
        /// </summary>
        private KeypadButton numberSixButton = new KeypadButton("6");

        /// <summary>
        /// The number seven button
        /// </summary>
        private KeypadButton numberSevenButton = new KeypadButton("7");

        /// <summary>
        /// The number eight button
        /// </summary>
        private KeypadButton numberEightButton = new KeypadButton("8");

        /// <summary>
        /// The number nine button
        /// </summary>
        private KeypadButton numberNineButton = new KeypadButton("9");

        /// <summary>
        /// The hash tag button
        /// </summary>
        private KeypadButton hashTagButton = new KeypadButton("#");

        /// <summary>
        /// The phone number
        /// </summary>
        private string phoneNumber = string.Empty;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The star button
        /// </summary>
        private KeypadButton starButton = new KeypadButton("*");

        /// <summary>
        /// The selected button
        /// </summary>
        private KeypadButton selectedButton;

        /// <summary>
        /// The successful call
        /// </summary>
        private bool successfulCall;

        /// <summary>
        /// The timer
        /// </summary>
        private Timer timer = new Timer();

        /// <summary>
        /// The tone player
        /// </summary>
        private SoundPlayer tonePlayer;

        /// <summary>
        /// The timer duration
        /// </summary>
        private TimeSpan timerDuration;

        /// <summary>
        /// The voicemail timer
        /// </summary>
        private Timer voicemailTimer = new Timer();

        /// <summary>
        /// The voicemail player
        /// </summary>
        private SoundPlayer voicemailPlayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoicemailViewModel"/> class.
        /// </summary>
        public VoicemailViewModel()
        {
            this.CreateButtonList();
            foreach (var button in this.buttonList)
            {
                button.PropertyChanged += Button_PropertyChanged;
            }

            this.contactList = new ObservableCollection<Contact> {
                new Contact("Police", "911", this.PolicePlayer, this.audioPath + "Voicemails" + "\\911-Voicemail.wav"),
                new Contact("Jack's Mechanics", "405-410-5892", JacksMechanicsPlayer, this.audioPath + "Voicemails" + "\\Jacks-Mechanics-Voicemail.wav"),
                new Contact("Johnny", "405-347-9012", JohnnyPlayer, this.audioPath + "Voicemails" + "\\911-Voicemail.wav"),
                new Contact("Pearl's Bakery", "405-511-7789", PearlsBakeryPlayer, this.audioPath + "Voicemails" + "\\911-Voicemail.wav"),
                new Contact("Rio's Pizza", "405-190-3668", RiosPizzaPlayer, this.audioPath + "Voicemails" + "\\911-Voicemail.wav")
            };

            foreach (var contact in this.contactList)
            {
                contact.PropertyChanged += Contact_PropertyChanged;
            }
        }

        /// <summary>
        /// Gets the back command.
        /// </summary>
        /// <value>
        /// The back command.
        /// </value>
        public ICommand BackCommand => new RelayCommand(this.Back);

        /// <summary>
        /// Gets the call command.
        /// </summary>
        /// <value>
        /// The call command.
        /// </value>
        public ICommand CallCommand => new RelayCommand(this.CallPressed);

        /// <summary>
        /// Gets or sets the call history list.
        /// </summary>
        /// <value>
        /// The call history list.
        /// </value>
        public ObservableCollection<string> CallHistoryList
        {
            get { return this.callHistoryList; }
            set
            {
                callHistoryList = value;
                RaisedPropertyChanged("CallHistoryList");
            }
        }

        /// <summary>
        /// Gets or sets the contact list.
        /// </summary>
        /// <value>
        /// The contact list.
        /// </value>
        public ObservableCollection<Contact> ContactList
        {
            get { return this.contactList; }
            set
            {
                contactList = value;
                RaisedPropertyChanged("ContactList");
            }
        }

        /// <summary>
        /// Gets the hash tag button.
        /// </summary>
        /// <value>
        /// The hash tag button.
        /// </value>
        public KeypadButton HashTagButton => this.hashTagButton;

        /// <summary>
        /// Gets or sets a value indicating whether [in call].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [in call]; otherwise, <c>false</c>.
        /// </value>
        public bool InCall
        {
            get { return this.inCall; }
            set
            {
                inCall = value;
                RaisedPropertyChanged("InCall");
                if (value == false)
                {
                    this.PhoneNumber = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the jacks mechanics player.
        /// </summary>
        /// <value>
        /// The jacks mechanics player.
        /// </value>
        public SoundPlayer JacksMechanicsPlayer => new SoundPlayer(this.audioPath + "Voicemails" + "\\Jacks-Mechanics-Voicemail.wav");

        /// <summary>
        /// Gets the johnny player.
        /// </summary>
        /// <value>
        /// The johnny player.
        /// </value>
        public SoundPlayer JohnnyPlayer => new SoundPlayer(this.audioPath + "Voicemails" + "\\Johnny-Voicemail.wav");

        /// <summary>
        /// Gets the not in service player.
        /// </summary>
        /// <value>
        /// The not in service player.
        /// </value>
        public SoundPlayer NotInServicePlayer => new SoundPlayer(this.audioPath + "NotInServiceSound.wav");

        /// <summary>
        /// Gets the number one button.
        /// </summary>
        /// <value>
        /// The number one button.
        /// </value>
        public KeypadButton NumberOneButton => this.numberOneButton;

        /// <summary>
        /// Gets the number zero button.
        /// </summary>
        /// <value>
        /// The number zero button.
        /// </value>
        public KeypadButton NumberZeroButton => this.numberZeroButton;

        /// <summary>
        /// Gets the number two button.
        /// </summary>
        /// <value>
        /// The number two button.
        /// </value>
        public KeypadButton NumberTwoButton => this.numberTwoButton;

        /// <summary>
        /// Gets the number three button.
        /// </summary>
        /// <value>
        /// The number three button.
        /// </value>
        public KeypadButton NumberThreeButton => this.numberThreeButton;

        /// <summary>
        /// Gets the number four button.
        /// </summary>
        /// <value>
        /// The number four button.
        /// </value>
        public KeypadButton NumberFourButton => this.numberFourButton;

        /// <summary>
        /// Gets the number five button.
        /// </summary>
        /// <value>
        /// The number five button.
        /// </value>
        public KeypadButton NumberFiveButton => this.numberFiveButton;

        /// <summary>
        /// Gets the number six button.
        /// </summary>
        /// <value>
        /// The number six button.
        /// </value>
        public KeypadButton NumberSixButton => this.numberSixButton;

        /// <summary>
        /// Gets the number seven button.
        /// </summary>
        /// <value>
        /// The number seven button.
        /// </value>
        public KeypadButton NumberSevenButton => this.numberSevenButton;

        /// <summary>
        /// Gets the number eight button.
        /// </summary>
        /// <value>
        /// The number eight button.
        /// </value>
        public KeypadButton NumberEightButton => this.numberEightButton;

        /// <summary>
        /// Gets the number nine button.
        /// </summary>
        /// <value>
        /// The number nine button.
        /// </value>
        public KeypadButton NumberNineButton => this.numberNineButton;

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber
        {
            get { return this.phoneNumber; }
            set
            {
                phoneNumber = value;
                RaisedPropertyChanged("PhoneNumber");
            }
        }

        /// <summary>
        /// Gets the phone ringing player.
        /// </summary>
        /// <value>
        /// The phone ringing player.
        /// </value>
        public SoundPlayer PhoneRingingPlayer => new SoundPlayer(this.audioPath + "PhoneRingingSound.wav");

        /// <summary>
        /// Gets the police player.
        /// </summary>
        /// <value>
        /// The police player.
        /// </value>
        public SoundPlayer PolicePlayer => new SoundPlayer(this.audioPath + "Voicemails" + "\\911-Voicemail.wav");

        /// <summary>
        /// Gets the pearls bakery player.
        /// </summary>
        /// <value>
        /// The pearls bakery player.
        /// </value>
        public SoundPlayer PearlsBakeryPlayer => new SoundPlayer(this.audioPath + "Voicemails" + "\\PearlsBakery-Voicemail.wav");

        /// <summary>
        /// Gets the rios pizza player.
        /// </summary>
        /// <value>
        /// The rios pizza player.
        /// </value>
        public SoundPlayer RiosPizzaPlayer => new SoundPlayer(this.audioPath + "Voicemails" + "\\Rios-Pizza-Voicemail.wav");

        /// <summary>
        /// Gets or sets the selected button.
        /// </summary>
        /// <value>
        /// The selected button.
        /// </value>
        public KeypadButton SelectedButton
        {
            get { return this.selectedButton; }
            set
            {
                selectedButton = value;
                RaisedPropertyChanged("SelectedButton");
            }
        }

        /// <summary>
        /// Gets or sets the tone player.
        /// </summary>
        /// <value>
        /// The tone player.
        /// </value>
        public SoundPlayer TonePlayer
        {
            get { return this.tonePlayer; }
            set { tonePlayer = value; }
        }

        /// <summary>
        /// Gets or sets the voicemail player.
        /// </summary>
        /// <value>
        /// The voicemail player.
        /// </value>
        public SoundPlayer VoicemailPlayer
        {
            get { return this.voicemailPlayer; }
            set { voicemailPlayer = value; }
        }

        /// <summary>
        /// Gets the star button.
        /// </summary>
        /// <value>
        /// The star button.
        /// </value>
        public KeypadButton StarButton => this.starButton;

        private void AddNumber(string number)
        {
            if (this.PhoneNumber.Length < 12 & !InCall)
            {
                this.PhoneNumber = this.PhoneNumber + number;
                this.CheckNumber();
            }
        }

        /// <summary>
        /// Backs this instance.
        /// </summary>
        private void Back()
        {
            if (this.PhoneNumber.Length > 0 & !InCall)
            {
                this.PhoneNumber = this.PhoneNumber.Remove(this.PhoneNumber.Length - 1);
            }
        }

        /// <summary>
        /// Calls the pressed.
        /// </summary>
        private void CallPressed()
        {
            if (!this.InCall & this.PhoneNumber.Length > 0)
            {
                if (this.contactList.Any(x => x.Number == this.PhoneNumber))
                {
                    this.successfulCall = true;
                    this.dialedContact = this.contactList.First(x => x.Number == this.PhoneNumber);
                    this.voicemailTimer = new System.Timers.Timer(this.dialedContact.VoicemailDuration.TotalMilliseconds);
                    this.PhoneNumber = dialedContact.Name;
                    this.VoicemailPlayer = dialedContact.Player;
                    this.callHistoryList.Add(dialedContact.Name + " (" + dialedContact.Number + ") : " + DateTime.Now);
                    var path = this.audioPath + "PhoneRingingSound.wav";
                    WaveFileReader wf = new WaveFileReader(path);
                    this.timerDuration = wf.TotalTime - new TimeSpan(280000000);
                    this.InCall = true;
                    this.TonePlayer = this.PhoneRingingPlayer;
                    this.PlayToneSound();
                }
                else
                {
                    this.callHistoryList.Add(this.PhoneNumber + ": " + DateTime.Now);
                    this.successfulCall = false;
                    var path = this.audioPath + "NotInServiceSound.wav";
                    WaveFileReader wf = new WaveFileReader(path);
                    this.timerDuration = wf.TotalTime;
                    this.InCall = true;
                    this.TonePlayer = this.NotInServicePlayer;
                    this.PlayToneSound();
                }
            }
            else 
            {
                this.timer.Stop();
                this.StopSound();
                this.voicemailTimer.Stop();
                this.StopVoicemailSound();
                this.PhoneNumber = string.Empty;
                this.InCall = false;
            }

        }

        /// <summary>
        /// Checks the number.
        /// </summary>
        private void CheckNumber()
        {
            if (this.PhoneNumber.Length > 3 & this.PhoneNumber.Length < 8 )
            {
                if (this.PhoneNumber[3] != '-')
                {
                    this.PhoneNumber = this.PhoneNumber.Insert(3, "-");
                }
            }
            else if (this.PhoneNumber.Length > 7 )
            {
                if (this.PhoneNumber[7] != '-')
                {
                    this.PhoneNumber = this.PhoneNumber.Insert(7, "-");
                }
            }
        }

        /// <summary>
        /// Creates the button list.
        /// </summary>
        private void CreateButtonList()
        {
            this.buttonList.Add(this.numberZeroButton);
            this.buttonList.Add(this.numberOneButton);
            this.buttonList.Add(this.numberTwoButton);
            this.buttonList.Add(this.numberThreeButton);
            this.buttonList.Add(this.numberEightButton);
            this.buttonList.Add(this.numberFourButton);
            this.buttonList.Add(this.numberFiveButton);
            this.buttonList.Add(this.numberSixButton);
            this.buttonList.Add(this.numberSevenButton);
            this.buttonList.Add(this.numberEightButton);
            this.buttonList.Add(this.numberNineButton);
            this.buttonList.Add(this.hashTagButton);
            this.buttonList.Add(this.starButton);
        }

        /// <summary>
        /// Noes the service timer elapsed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void NoServiceTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            this.timer.Stop();
            this.StopSound();
            this.InCall = false;
        }

        /// <summary>
        /// Plays the tone sound.
        /// </summary>
        public void PlayToneSound()
        {
            this.TonePlayer.Load();
            this.TonePlayer.Play();
            this.StartToneTimer();
        }

        /// <summary>
        /// Plays the voicemail sound.
        /// </summary>
        public void PlayVoicemailSound()
        {
            this.VoicemailPlayer.Load();
            this.VoicemailPlayer.Play();
            this.StartVoicemailTimer();
        }

        /// <summary>
        /// Starts the tone timer.
        /// </summary>
        public void StartToneTimer()
        {
            this.timer = new System.Timers.Timer(this.timerDuration.TotalMilliseconds);
            this.timer.Interval = this.timerDuration.TotalMilliseconds;
            this.timer.Start();
            if (successfulCall)
            {
                this.timer.Elapsed += Timer_Elapsed;
            }
            else
            {
                this.timer.Elapsed += NoServiceTimer_Elapsed;
            }
        }

        /// <summary>
        /// Starts the voicemail timer.
        /// </summary>
        public void StartVoicemailTimer()
        {
            this.voicemailTimer.Start();
            this.voicemailTimer.Elapsed += this.VoicemailTimer_Elapsed;
        }

        /// <summary>
        /// Stops the sound.
        /// </summary>
        public void StopSound()
        {
            this.TonePlayer?.Stop();
            this.TonePlayer = new SoundPlayer();
        }

        /// <summary>
        /// Stops the voicemail sound.
        /// </summary>
        public void StopVoicemailSound()
        {
            this.VoicemailPlayer?.Stop();
            this.VoicemailPlayer = new SoundPlayer();
            this.InCall = false;
        }

        /// <summary>
        /// Contacts the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Contact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var contact = sender as Contact;
            if (contact.CallBtnPressed)
            {
                if (!this.InCall)
                {
                    this.PhoneNumber = contact.Number;
                    this.CallPressed();
                }
                contact.CallBtnPressed = false;
            }
        }

        /// <summary>
        /// Buttons the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Button_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var button = sender as KeypadButton;
            if (button.IsSelected)
            {
                this.AddNumber(button.Value);
                button.IsSelected = false;
            }
        }

        /// <summary>
        /// Timers the elapsed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            this.timer.Stop();
            this.StopSound();
            this.PlayVoicemailSound();
        }

        /// <summary>
        /// Voicemails the timer elapsed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void VoicemailTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            this.voicemailTimer.Stop();
            this.StopVoicemailSound();
        }

        /// <summary>
        /// Raiseds the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisedPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
